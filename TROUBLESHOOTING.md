# Troubleshooting Network Connection

## Server Not Starting

### Check 1: NetworkManager in Scene
The scene MUST have a GameObject with:
- `NetworkManager` component
- `UnityTransport` component
- Both `NetworkStartUI` and `NetworkDiagnostics` scripts attached

### Check 2: Transport Configuration
In Unity Editor → NetworkManager → UnityTransport:
```
Address: 0.0.0.0 (for server to bind to all interfaces)
Port: 7777
Server Listen Address: 0.0.0.0 (or leave empty)
```

### Check 3: Rebuild After Script Changes
After modifying `NetworkStartUI.cs`:
```powershell
# In Unity Editor: Build the Linux Server
# File → Build Settings → Platform: Dedicated Server
# Build Location: Builds/LinuxServer/

# Rebuild Docker
docker-compose down
docker-compose build
docker-compose up -d
```

## Client Cannot Connect

### From Same Machine (Unity Editor)
In Unity Editor → NetworkManager → UnityTransport:
```
Address: localhost (or 127.0.0.1)
Port: 7777
```

Then click **"Client"** button or call `NetworkManager.Singleton.StartClient()`

### From Different Machine
Replace `localhost` with the Docker host's IP address:
```
Address: 192.168.x.x (your machine's local IP)
Port: 7777
```

### Firewall Check
```powershell
# Check if port is accessible
Test-NetConnection localhost -Port 7777

# Check Docker port mapping
docker ps --format "table {{.Names}}\t{{.Ports}}"
```

## Viewing Server Logs

```powershell
# Real-time logs
docker logs -f ewmmg-server

# Search for errors
docker logs ewmmg-server | Select-String "error|failed|exception" -CaseSensitive:$false

# Search for Netcode messages
docker logs ewmmg-server | Select-String "NetworkManager|Server|Transport|Diagnostics"
```

## Common Issues

### Issue: "NetworkManager.Singleton is null"
**Solution**: Add NetworkManager GameObject to your scene with NetworkManager and UnityTransport components.

### Issue: Server logs show "Auto-launching" but no "Server started successfully"
**Solution**: 
1. NetworkManager might not be in the scene
2. Check Unity Player logs in the build
3. Rebuild with updated NetworkStartUI.cs script

### Issue: Client stuck on "Connecting..."
**Solution**: 
1. Verify server is running: `docker ps`
2. Check server logs for errors
3. Verify NetworkManager Transport address is set to `localhost` or correct IP
4. Check firewall isn't blocking port 7777

### Issue: Docker container keeps restarting
**Solution**: Check logs for Unity crashes, ensure build includes all dependencies
```powershell
docker logs ewmmg-server --tail 100
```
