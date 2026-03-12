FROM --platform=linux/amd64 python:3.12-slim

RUN apt-get update && apt-get install -y --no-install-recommends \
    curl \
    ca-certificates \
    libfontconfig1 \
    libgraphite2-3 \
    libharfbuzz0b \
    libicu76 \
    libssl3t64 \
    fonts-noto-core \
    fonts-noto-cjk \
    fonts-noto-extra \
    && rm -rf /var/lib/apt/lists/* \
    && fc-cache -f -v 2>/dev/null || true

# Install tectonic (only x86_64 binaries are published)
RUN curl --proto '=https' --tlsv1.2 -fsSL https://drop-sh.fullyjustified.net | sh \
    && mv ./tectonic /usr/local/bin/tectonic

WORKDIR /service

COPY src/Services/Latex/server.py .

RUN pip install --no-cache-dir fastapi uvicorn httpx python-multipart

CMD ["uvicorn", "server:app", "--host", "0.0.0.0", "--port", "8080"]