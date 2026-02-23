# Player Prefab Setup Guide

## Quick Setup

### 1. Create Player GameObject
In Unity Editor:
1. **Create a 3D Cube**: Hierarchy → 3D Object → Cube
2. **Name it**: "Player"
3. **Position**: (0, 0, 0)

### 2. Add Network Components
Select the Player GameObject, then add these components:

**Required Components:**
- **NetworkObject** (Add Component → Netcode → NetworkObject)
- **PlayerMovement** (Drag from Assets/Scripts/PlayerMovement.cs)

**Verify Components:**
- The cube already has:
  - `Transform`
  - `Cube (Mesh Filter)`
  - `Mesh Renderer`
  - `Box Collider`

### 3. Create Prefab
1. Drag the "Player" GameObject from Hierarchy → Project window (Assets folder)
2. This creates `Player.prefab`
3. Delete the Player from Hierarchy (prefab will spawn it)

### 4. Assign to NetworkManager
1. Select **NetworkManager** GameObject in Hierarchy
2. In Inspector → **NetworkManager** component
3. Find **Player Prefabs** section
4. Click **+** to add a slot
5. Drag **Player.prefab** into the slot (Default Player Prefab)

### 5. Test
**Play in Editor:**
- Click "Host" or "Client"
- Player cube should spawn
- Red cube = you (owner)
- Use WASD or Arrow Keys to move

**Test with Docker Server:**
```powershell
# Rebuild with changes
docker-compose down
docker-compose build
docker-compose up -d

# In Unity Editor: Click "Client"
# Your player should spawn
```

---

## Optional: Better Player Visual

### Add a Camera Follow (for better view)
1. Create: GameObject → Camera → Name: "PlayerCamera"
2. Make it a child of Player prefab
3. Position: (0, 5, -10)
4. Rotation: (20, 0, 0)

### Different Colors for Each Player
Create a script to randomize colors:

```csharp
// PlayerVisual.cs
using Unity.Netcode;
using UnityEngine;

public class PlayerVisual : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        // Owner gets red, others get random colors
        if (IsOwner)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            GetComponent<Renderer>().material.color = Random.ColorHSV();
        }
    }
}
```

Add this script to your Player prefab.

---

## Troubleshooting

### "No player spawned!" warning in UI
- **Cause**: No player prefab assigned to NetworkManager
- **Fix**: Follow steps above to assign Player.prefab

### Player spawns but can't move
- **Cause**: PlayerMovement script not on prefab, or NetworkObject missing
- **Fix**: Verify both components are on the prefab

### Multiple players spawn
- **Cause**: Player GameObject left in scene + prefab spawning
- **Fix**: Remove all Player objects from Hierarchy, only use prefab

### Can't see other players
- **Cause**: Prefab not properly networked
- **Fix**: Ensure prefab has NetworkObject component with "Spawn With Observer" checked
