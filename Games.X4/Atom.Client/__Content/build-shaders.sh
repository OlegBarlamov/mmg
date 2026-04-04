#!/bin/bash
set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
CONTENT_DIR="$SCRIPT_DIR"
OUTPUT_DIR="$CONTENT_DIR/bin/DesktopGL/Content"
SHADER_BUILD_DIR="$CONTENT_DIR/obj/ShaderBuild"
DOCKER_IMAGE="mgfxc-8"
DOCKERFILE="$CONTENT_DIR/Dockerfile.mgfxc"

mkdir -p "$OUTPUT_DIR"

if ! command -v docker &> /dev/null; then
    echo "[build-shaders] Docker not found, skipping shader compilation."
    exit 0
fi

if ! docker info &> /dev/null 2>&1; then
    echo "[build-shaders] Docker daemon not running, skipping shader compilation."
    exit 0
fi

if ! docker image inspect "$DOCKER_IMAGE" &> /dev/null 2>&1; then
    echo "[build-shaders] Building Docker image '$DOCKER_IMAGE' (first time only, may take a few minutes)..."
    docker build --platform linux/amd64 -t "$DOCKER_IMAGE" -f "$DOCKERFILE" "$CONTENT_DIR"
fi

NEEDS_COMPILE=0
if [ -f "$CONTENT_DIR/StarBeam.fx" ]; then
    if [ ! -f "$OUTPUT_DIR/StarBeam.xnb" ] || [ "$CONTENT_DIR/StarBeam.fx" -nt "$OUTPUT_DIR/StarBeam.xnb" ]; then
        NEEDS_COMPILE=1
    fi
fi

if [ "$NEEDS_COMPILE" -eq 0 ]; then
    echo "[build-shaders] All shaders up to date."
    exit 0
fi

echo "[build-shaders] Compiling shaders via Docker..."

mkdir -p "$SHADER_BUILD_DIR"

cat > "$CONTENT_DIR/_Shaders.mgcb" << 'MGCB_EOF'
#----------------------------- Global Properties ----------------------------#
/outputDir:obj/ShaderBuild/out
/intermediateDir:obj/ShaderBuild/intermediate
/platform:DesktopGL
/config:
/profile:Reach
/compress:False
#---------------------------------- Content ---------------------------------#

#begin StarBeam.fx
/importer:EffectImporter
/processor:EffectProcessor
/processorParam:DebugMode=Auto
/build:StarBeam.fx

MGCB_EOF

docker run --rm --platform linux/amd64 \
    -v "$CONTENT_DIR:/content" \
    "$DOCKER_IMAGE" \
    -c '
        rm -f /tmp/.X99-lock /tmp/.X11-unix/X99
        Xvfb :99 -screen 0 1024x768x24 &
        sleep 2
        export DISPLAY=:99

        dotnet tool install -g dotnet-mgcb --version 3.8.2.1105 2>/dev/null || true
        export PATH=$PATH:/root/.dotnet/tools

        cd /content
        mgcb /@:/content/_Shaders.mgcb /workingDir:/content 2>&1
    '

rm -f "$CONTENT_DIR/_Shaders.mgcb"

if [ -f "$SHADER_BUILD_DIR/out/StarBeam.xnb" ]; then
    cp -f "$SHADER_BUILD_DIR/out/StarBeam.xnb" "$OUTPUT_DIR/StarBeam.xnb"
    echo "[build-shaders] Shader compilation complete."
else
    echo "[build-shaders] ERROR: StarBeam.xnb was not produced!"
    exit 1
fi
