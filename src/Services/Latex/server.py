from fastapi import (
    FastAPI,
    HTTPException,
    BackgroundTasks,
    UploadFile,
    File,
    Form,
)
from fastapi.responses import FileResponse
from pydantic import BaseModel
from datetime import datetime
from pathlib import Path
from urllib.parse import urlparse, unquote
import subprocess
import uuid
import os
import shutil
import re
import mimetypes
import httpx

app = FastAPI()


class LatexRequest(BaseModel):
    content: str


INCLUDEGRAPHICS_RE = re.compile(r"\\includegraphics(\[[^\]]*\])?\{([^}]+)\}")

ALLOWED_IMAGE_HOSTS = {
    "minio.hyperdatalab.site",
}

MAX_REMOTE_IMAGES = int(os.getenv("MAX_REMOTE_IMAGES", "20"))

REMOTE_IMAGE_TIMEOUT = float(os.getenv("REMOTE_IMAGE_TIMEOUT", "20"))


def cleanup(path: str):
    shutil.rmtree(path, ignore_errors=True)


def normalize_latex(content: str) -> str:
    # Collapse whitespace+newline before closing } and ] to fix pretty JSON formatting
    return re.sub(r"\s*\n\s*([}\]])", r"\1", content)


def assert_allowed_remote_url(url: str):
    parsed = urlparse(url)
    if parsed.scheme not in ("http", "https"):
        raise HTTPException(status_code=400, detail=f"Unsupported URL scheme: {url}")

    host = (parsed.hostname or "").lower()
    if not host:
        raise HTTPException(status_code=400, detail=f"Invalid URL host: {url}")

    if ALLOWED_IMAGE_HOSTS and host not in ALLOWED_IMAGE_HOSTS:
        raise HTTPException(
            status_code=400,
            detail=f"Remote host not allowed: {host}",
        )


def safe_name(value: str, default: str) -> str:
    name = re.sub(r"[^a-zA-Z0-9_-]+", "_", value).strip("_")
    return name or default


async def download_remote_image(url: str, assets_dir: Path, idx: int) -> str:
    assert_allowed_remote_url(url)

    parsed = urlparse(url)
    raw_stem = Path(unquote(parsed.path)).stem or f"img_{idx}"
    stem = safe_name(raw_stem, f"img_{idx}")

    async with httpx.AsyncClient(
        timeout=REMOTE_IMAGE_TIMEOUT,
        follow_redirects=True,
    ) as client:
        resp = await client.get(url)

    if resp.status_code >= 400:
        raise HTTPException(
            status_code=400,
            detail=f"Cannot download image ({resp.status_code}): {url}",
        )

    content_type = (resp.headers.get("content-type") or "").split(";")[0].strip().lower()
    if content_type and not content_type.startswith("image/"):
        raise HTTPException(
            status_code=400,
            detail=f"URL is not an image: {url} (content-type={content_type})",
        )

    ext = (
        mimetypes.guess_extension(content_type)
        or Path(parsed.path).suffix
        or ".img"
    )
    filename = f"{stem}_{idx}{ext}"
    dst = assets_dir / filename
    dst.write_bytes(resp.content)

    return f"assets/{filename}"


async def rewrite_remote_includegraphics(latex: str, workdir: Path) -> str:
    assets_dir = workdir / "assets"
    assets_dir.mkdir(parents=True, exist_ok=True)

    matches = list(INCLUDEGRAPHICS_RE.finditer(latex))
    if not matches:
        return latex

    out = []
    last = 0
    remote_count = 0

    for m in matches:
        out.append(latex[last:m.start()])
        opts = m.group(1) or ""
        target = (m.group(2) or "").strip()

        if target.startswith("http://") or target.startswith("https://"):
            remote_count += 1
            if remote_count > MAX_REMOTE_IMAGES:
                raise HTTPException(
                    status_code=400,
                    detail=f"Too many remote images (max={MAX_REMOTE_IMAGES})",
                )
            local_path = await download_remote_image(target, assets_dir, remote_count)
            out.append(f"\\includegraphics{opts}{{{local_path}}}")
        else:
            out.append(m.group(0))

        last = m.end()

    out.append(latex[last:])
    return "".join(out)


def run_tectonic(workdir: Path):
    result = subprocess.run(
        ["tectonic", "doc.tex"],
        cwd=str(workdir),
        capture_output=True,
        text=True,
    )
    return result


async def compile_text_to_pdf(content: str, workdir: Path):
    normalized = normalize_latex(content)
    rewritten = await rewrite_remote_includegraphics(normalized, workdir)

    tex_path = workdir / "doc.tex"
    tex_path.write_text(rewritten, encoding="utf-8")

    result = run_tectonic(workdir)
    pdf_path = workdir / "doc.pdf"

    if result.returncode != 0 or not pdf_path.exists():
        err = (result.stderr or "").strip()
        out = (result.stdout or "").strip()
        message = err or out or "Unknown compile error"
        raise HTTPException(status_code=500, detail=message)

    return pdf_path


@app.get("/health")
async def health_check():
    start = datetime.now()
    entries = {}
    end = datetime.now()
    duration = end - start
    return {
        "status": "Healthy",
        "totalDuration": str(duration),
        "entries": entries,
    }


@app.post("/compile")
async def compile_latex(body: LatexRequest, background_tasks: BackgroundTasks):
    job = str(uuid.uuid4())
    workdir = Path(f"/tmp/{job}")
    workdir.mkdir(parents=True, exist_ok=True)

    try:
        pdf = await compile_text_to_pdf(body.content, workdir)
    except Exception:
        cleanup(str(workdir))
        raise

    background_tasks.add_task(cleanup, str(workdir))
    return FileResponse(str(pdf), media_type="application/pdf")


# Optional: compile with files uploaded directly (no remote fetch required)
@app.post("/compile-multipart")
async def compile_latex_multipart(
    background_tasks: BackgroundTasks,
    content: str = Form(...),
    files: list[UploadFile] = File(default_factory=list),
):
    job = str(uuid.uuid4())
    workdir = Path(f"/tmp/{job}")
    assets_dir = workdir / "assets"
    workdir.mkdir(parents=True, exist_ok=True)
    assets_dir.mkdir(parents=True, exist_ok=True)

    try:
        # Save uploaded files to assets/
        for idx, f in enumerate(files, start=1):
            original = f.filename or f"upload_{idx}"
            stem = safe_name(Path(original).stem, f"upload_{idx}")
            ext = Path(original).suffix or ""
            dst = assets_dir / f"{stem}_{idx}{ext}"
            data = await f.read()
            dst.write_bytes(data)

        pdf = await compile_text_to_pdf(content, workdir)
    except Exception:
        cleanup(str(workdir))
        raise

    background_tasks.add_task(cleanup, str(workdir))
    return FileResponse(str(pdf), media_type="application/pdf")