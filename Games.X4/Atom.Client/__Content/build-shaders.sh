#!/bin/bash
set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
CONTENT_DIR="$SCRIPT_DIR"
OUTPUT_DIR="$CONTENT_DIR/bin/DesktopGL/Content"
SHADER_BUILD_DIR="$CONTENT_DIR/obj/ShaderBuild"
DOCKER_IMAGE="mgfxc-8"
DOCKERFILE="$CONTENT_DIR/Dockerfile.mgfxc"

mkdir -p "$OUTPUT_DIR"

# Collect all .fx files
FX_FILES=()
while IFS= read -r -d '' fx; do
    FX_FILES+=("$fx")
done < <(find "$CONTENT_DIR" -maxdepth 1 -name "*.fx" -type f -print0)

if [ ${#FX_FILES[@]} -eq 0 ]; then
    echo "[build-shaders] No .fx files found."
    exit 0
fi

# Check which shaders need recompilation
STALE_FILES=()
for fx in "${FX_FILES[@]}"; do
    name=$(basename "$fx" .fx)
    if [ ! -f "$OUTPUT_DIR/${name}.xnb" ] || [ "$fx" -nt "$OUTPUT_DIR/${name}.xnb" ]; then
        STALE_FILES+=("$fx")
    fi
done

if [ ${#STALE_FILES[@]} -eq 0 ]; then
    echo "[build-shaders] All ${#FX_FILES[@]} shader(s) up to date."
    exit 0
fi

echo "[build-shaders] ${#STALE_FILES[@]} of ${#FX_FILES[@]} shader(s) need compilation."

if ! command -v docker &> /dev/null; then
    echo "[build-shaders] ERROR: Docker is not installed. Docker is required to compile shaders on macOS."
    exit 1
fi

if ! docker info &> /dev/null 2>&1; then
    echo "[build-shaders] ERROR: Docker daemon is not running. Start Docker Desktop and retry."
    exit 1
fi

if [ -z "$(docker images -q "$DOCKER_IMAGE" 2>/dev/null)" ]; then
    echo "[build-shaders] ERROR: Docker image '$DOCKER_IMAGE' not found."
    echo "[build-shaders] Run the following command to build it:"
    echo ""
    echo "  bash \"$CONTENT_DIR/build-shader-image.sh\""
    echo ""
    exit 1
fi

mkdir -p "$SHADER_BUILD_DIR"

# Generate MGCB project with only stale shaders
cat > "$CONTENT_DIR/_Shaders.mgcb" << 'MGCB_HEADER'
#----------------------------- Global Properties ----------------------------#
/outputDir:obj/ShaderBuild/out
/intermediateDir:obj/ShaderBuild/intermediate
/platform:DesktopGL
/config:
/profile:Reach
/compress:False
#---------------------------------- Content ---------------------------------#
MGCB_HEADER

for fx in "${STALE_FILES[@]}"; do
    name=$(basename "$fx")
    cat >> "$CONTENT_DIR/_Shaders.mgcb" << ENTRY

#begin $name
/importer:EffectImporter
/processor:EffectProcessor
/processorParam:DebugMode=Auto
/build:$name
ENTRY
done

echo "[build-shaders] Compiling shaders via Docker..."

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

# Copy compiled outputs and verify
FAILED=0
for fx in "${STALE_FILES[@]}"; do
    name=$(basename "$fx" .fx)
    if [ -f "$SHADER_BUILD_DIR/out/${name}.xnb" ]; then
        cp -f "$SHADER_BUILD_DIR/out/${name}.xnb" "$OUTPUT_DIR/${name}.xnb"
        echo "[build-shaders]   ${name}.fx -> ${name}.xnb"
    else
        echo "[build-shaders] ERROR: ${name}.xnb was not produced!"
        FAILED=1
    fi
done

if [ "$FAILED" -eq 1 ]; then
    exit 1
fi

echo "[build-shaders] Shader compilation complete."
