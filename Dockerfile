# Use Ubuntu as base image for Unity Linux server builds
FROM ubuntu:22.04

# Install dependencies for Unity headless server
RUN apt-get update && apt-get install -y \
    ca-certificates \
    libglu1-mesa \
    xvfb \
    libx11-6 \
    libxcursor1 \
    libxrandr2 \
    && rm -rf /var/lib/apt/lists/*

# Create app directory
WORKDIR /app

# Copy the Unity Linux server build
# From GitHub Actions unity-builder output: Build/StandaloneLinux64/
COPY Build/StandaloneLinux64/ ./

# Make all executables executable and create a startup script
RUN chmod +x ./*.x86_64 2>/dev/null || true && \
    chmod +x ./EWMMG* 2>/dev/null || true && \
    echo '#!/bin/bash' > /app/start.sh && \
    echo '# Clean up stale X lock files' >> /app/start.sh && \
    echo 'rm -f /tmp/.X99-lock /tmp/.X11-unix/X99' >> /app/start.sh && \
    echo '' >> /app/start.sh && \
    echo '# Start Xvfb in the background for headless display support' >> /app/start.sh && \
    echo 'Xvfb :99 -screen 0 1024x768x24 &' >> /app/start.sh && \
    echo 'XVFB_PID=$!' >> /app/start.sh && \
    echo 'export DISPLAY=:99' >> /app/start.sh && \
    echo 'sleep 2' >> /app/start.sh && \
    echo '' >> /app/start.sh && \
    echo '# Set SDL environment variables to prevent input system calls' >> /app/start.sh && \
    echo 'export SDL_VIDEODRIVER=dummy' >> /app/start.sh && \
    echo 'export SDL_AUDIODRIVER=dummy' >> /app/start.sh && \
    echo 'export SDL_MOUSE_FOCUS_CLICKTHROUGH=0' >> /app/start.sh && \
    echo '' >> /app/start.sh && \
    echo '# Cleanup function' >> /app/start.sh && \
    echo 'cleanup() {' >> /app/start.sh && \
    echo '  echo "Shutting down..."' >> /app/start.sh && \
    echo '  kill $XVFB_PID 2>/dev/null' >> /app/start.sh && \
    echo '  exit 0' >> /app/start.sh && \
    echo '}' >> /app/start.sh && \
    echo 'trap cleanup SIGTERM SIGINT' >> /app/start.sh && \
    echo '' >> /app/start.sh && \
    echo '# Launch Unity server' >> /app/start.sh && \
    echo 'if [ -f "./EWMMG.x86_64" ]; then' >> /app/start.sh && \
    echo '  exec ./EWMMG.x86_64 "$@"' >> /app/start.sh && \
    echo 'elif [ -f "./EWMMG-linux.x86_64" ]; then' >> /app/start.sh && \
    echo '  exec ./EWMMG-linux.x86_64 "$@"' >> /app/start.sh && \
    echo 'else' >> /app/start.sh && \
    echo '  echo "Error: No Unity executable found"' >> /app/start.sh && \
    echo '  exit 1' >> /app/start.sh && \
    echo 'fi' >> /app/start.sh && \
    chmod +x /app/start.sh

# Expose Netcode default port (7777) and Unity Transport default port (7777 UDP)
EXPOSE 7777/tcp
EXPOSE 7777/udp

# Set environment variables for headless operation
ENV DISPLAY=:99

# Run the Unity server with headless and server flags
# -disable-input-manager prevents input system crashes in headless mode
CMD ["/app/start.sh", "-batchmode", "-nographics", "-disable-input-manager", "-launch-as-server"]
