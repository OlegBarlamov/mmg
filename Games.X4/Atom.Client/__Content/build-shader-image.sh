#!/bin/bash
set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
DOCKER_IMAGE="mgfxc-8"
DOCKERFILE="$SCRIPT_DIR/Dockerfile.mgfxc"

if ! command -v docker &> /dev/null; then
    echo "ERROR: Docker is not installed."
    exit 1
fi

if ! docker info &> /dev/null 2>&1; then
    echo "ERROR: Docker daemon is not running. Start Docker Desktop and retry."
    exit 1
fi

echo "Building Docker image '$DOCKER_IMAGE'..."
docker build --platform linux/amd64 -t "$DOCKER_IMAGE" -f "$DOCKERFILE" "$SCRIPT_DIR"
echo "Docker image '$DOCKER_IMAGE' built successfully."
