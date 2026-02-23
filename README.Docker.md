# Docker Deployment Guide

## Prerequisites
- Docker installed on your system
- Unity Linux Server build in `Builds/LinuxServer/`

## Building the Unity Server

### For Unity 6 (Recommended):
1. Open your project in Unity Editor
2. Go to **File → Build Settings**
3. Click **Add Open Scenes** to include your scenes
4. Switch platform to **Dedicated Server**
5. Set the build location to `Builds/LinuxServer/`
6. Click **Build**

### For Unity 2021-2023:
1. Open your project in Unity Editor
2. Go to **File → Build Settings**
3. Select **Linux** as the target platform
4. Check **Server Build** and **Headless Mode**
5. Set the build location to `Builds/LinuxServer/`
6. Click **Build**

**Note:** If you're using a regular Linux build (not Dedicated Server), the Docker container will run Xvfb (virtual display) to handle graphics calls, which adds some overhead but ensures compatibility.

## Building the Docker Image

```bash
docker build -t ewmmg-server .
```

## Running the Server

### Using Docker directly:
```bash
docker run -d \
  --name ewmmg-server \
  -p 7777:7777/tcp \
  -p 7777:7777/udp \
  ewmmg-server
```

### Using Docker Compose:
```bash
docker-compose up -d
```

## Managing the Server

### View logs:
```bash
docker logs ewmmg-server
```

### Stop the server:
```bash
docker stop ewmmg-server
```

### Restart the server:
```bash
docker restart ewmmg-server
```

### Remove the container:
```bash
docker rm -f ewmmg-server
```

## Port Configuration

The default Netcode port is **7777** (TCP/UDP). If you need to change it:

1. Update the port mapping in `docker-compose.yml` or the `docker run` command
2. Configure the port in your Unity Netcode NetworkManager

## Notes

- The server runs in headless mode (no graphics)
- The `-launch-as-server` flag automatically starts the server on launch
- Make sure your Unity build is configured for Linux dedicated server
