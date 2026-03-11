from fastapi import FastAPI, HTTPException, BackgroundTasks
from fastapi.responses import FileResponse
from pydantic import BaseModel
from datetime import datetime
import subprocess, uuid, os, shutil, re

app = FastAPI()


class LatexRequest(BaseModel):
    content: str


def cleanup(path: str):
    shutil.rmtree(path, ignore_errors=True)


@app.get("/health")
async def health_check():
    start = datetime.now()

    entries = {}

    end = datetime.now()
    duration = end - start

    return {
        "status": "Healthy",
        "totalDuration": str(duration),
        "entries": entries
    }


@app.post("/compile")
async def compile_latex(body: LatexRequest, background_tasks: BackgroundTasks):

    # Collapse whitespace+newline before closing } and ] to fix
    # content that has been auto-formatted (e.g. JSON pretty-print)
    latex = re.sub(r'\s*\n\s*([}\]])', r'\1', body.content)

    job = str(uuid.uuid4())
    path = f"/tmp/{job}"
    os.makedirs(path)

    tex = f"{path}/doc.tex"

    with open(tex, "w") as f:
        f.write(latex)

    result = subprocess.run(
        ["tectonic", "doc.tex"],
        cwd=path,
        capture_output=True,
    )

    pdf = f"{path}/doc.pdf"

    if not os.path.exists(pdf):
        cleanup(path)
        raise HTTPException(status_code=500, detail=result.stderr.decode())

    background_tasks.add_task(cleanup, path)

    return FileResponse(pdf, media_type="application/pdf")