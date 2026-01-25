# Hooked on Kharon - Code Reference

**Purpose:** Quick reference for existing code, APIs, and conventions. Check this before writing new code to avoid referencing non-existent classes or methods.

**Last Updated:** January 24, 2026 (Added FreeMovementController, Central Hub scene)

---

## Namespaces

| Namespace | Purpose | Status |
|-----------|---------|--------|
| `HOK.Core` | Game state, managers, shared utilities | Active |
| `HOK.Ferry` | Raft movement, soul transport | Active |
| `HOK.Fishing` | Cast, reel, catch systems | Planned |
| `HOK.Companion` | Scorch behavior | Planned |
| `HOK.Progression` | Unlocks, currency, codex | Planned |
| `HOK.UI` | Menus, HUD, codex display | Planned |
| `HOK.Audio` | Music, SFX managers | Planned |
| `HOK.Data` | ScriptableObject definitions | Planned |
| `HOK.Editor` | Editor utilities, tools | Active |

---

## Scripts

### HOK.Core

#### GameState.cs
**Path:** `Assets/HOK/Scripts/Core/GameState.cs`
**Type:** Enum

```csharp
public enum GameState
{
    OffDuty = 0,    // Kharon is fishing in peace
    Fishing = 1,    // Actively fishing
    Ferrying = 2,   // Transporting souls
    InMenu = 3      // UI menu is open
}
```

---

#### GameManager.cs
**Path:** `Assets/HOK/Scripts/Core/GameManager.cs`
**Type:** MonoBehaviour (Singleton, DontDestroyOnLoad)

**Dependencies:**
- `Obvious.Soap.IntVariable` - CurrentGameState
- `Obvious.Soap.ScriptableEventInt` - OnGameStateChanged

**Public API:**
| Member | Type | Description |
|--------|------|-------------|
| `Instance` | static GameManager | Singleton instance |
| `CurrentState` | GameState (property) | Current game state from SOAP variable |
| `SetState(GameState)` | void | Changes state, raises event |

**Usage:**
```csharp
// Get current state
GameState state = GameManager.Instance.CurrentState;

// Change state
GameManager.Instance.SetState(GameState.Ferrying);
```

---

#### FollowTarget.cs
**Path:** `Assets/HOK/Scripts/Core/FollowTarget.cs`
**Type:** MonoBehaviour
**Attributes:** `[ExecuteAlways]`

**Serialized Fields:**
| Field | Type | Default | Description |
|-------|------|---------|-------------|
| `target` | Transform | null | Transform to follow |
| `offset` | Vector3 | (0, 0.5, 0) | Position offset from target |
| `followRotation` | bool | false | Match target rotation |

**Public API:**
| Method | Description |
|--------|-------------|
| `SetTarget(Transform)` | Sets the follow target |
| `SetOffset(Vector3)` | Sets the position offset |

**Usage:**
```csharp
FollowTarget follower = GetComponent<FollowTarget>();
follower.SetTarget(raftTransform);
follower.SetOffset(new Vector3(0f, 0.95f, 0f));
```

**Notes:**
- Runs in both Edit and Play mode due to `[ExecuteAlways]`
- Updates position in `LateUpdate()` to run after target moves
- Used for Kharon/Scorch to follow raft without inheriting scale

---

### HOK.Ferry

#### RaftController.cs
**Path:** `Assets/HOK/Scripts/Ferry/RaftController.cs`
**Type:** MonoBehaviour

**Dependencies:**
- `Dreamteck.Splines.SplineComputer` - River path
- `UnityEngine.InputSystem.InputValue` - Input handling
- `Obvious.Soap.ScriptableEventNoParam` - Movement and junction events (optional)
- `HOK.Ferry.SplineJunction` - Junction points for branching

**Serialized Fields:**
| Field | Type | Default | Description |
|-------|------|---------|-------------|
| `maxSpeed` | float | 5f | Maximum movement speed |
| `acceleration` | float | 3f | Acceleration rate |
| `deceleration` | float | 5f | Deceleration rate |
| `spline` | SplineComputer | null | River spline to follow |
| `junctions` | List\<SplineJunction\> | empty | Junctions on current spline (auto-populated) |
| `onRaftStartedMoving` | ScriptableEventNoParam | null | Event when movement begins |
| `onRaftStoppedMoving` | ScriptableEventNoParam | null | Event when movement stops |
| `onJunctionAvailable` | ScriptableEventNoParam | null | Event when entering junction range |
| `onJunctionTaken` | ScriptableEventNoParam | null | Event when branching to new spline |

**Public API:**
| Member | Type | Description |
|--------|------|-------------|
| `IsMoving` | bool (property) | True if raft is moving |
| `CurrentSpeed` | float (property) | Current movement speed |
| `CurrentSpline` | SplineComputer (property) | The spline being followed |
| `ActiveJunction` | SplineJunction (property) | Currently available junction (if any) |
| `IsJunctionAvailable` | bool (property) | True if a junction can be taken |
| `OnMove(InputValue)` | void | Called by PlayerInput (SendMessages) |
| `SetMoveInput(Vector2)` | void | Direct input control |
| `GetSplinePercent()` | double | Current position on spline (0-1) |
| `SetSplinePercent(double)` | void | Teleport to spline position |
| `SetSpline(SplineComputer)` | void | Assign new spline, refresh junctions |
| `SetSpline(SplineComputer, double)` | void | Assign new spline at entry position |

**Input Handling:**
- Uses `PlayerInput` component with `SendMessages` behavior
- Receives `OnMove(InputValue)` from Ferry action map
- Camera at -Z looking toward +Z: +X=screen right, -X=screen left, +Z=screen top, -Z=screen bottom
- A/Left Arrow = move toward dock (left, -X, higher spline %)
- D/Right Arrow = move toward entrance (right, +X, lower spline %)
- W/Up Arrow = take junction (if available)

**Usage:**
```csharp
RaftController raft = GetComponent<RaftController>();

// Check movement
if (raft.IsMoving) { /* ... */ }

// Check for available junction
if (raft.IsJunctionAvailable)
{
    // Player can press Up to take the junction
    SplineJunction junction = raft.ActiveJunction;
}

// Teleport to middle of river
raft.SetSplinePercent(0.5);

// Switch to a different spline
raft.SetSpline(otherSpline, 0.0); // Enter at start
```

---

#### SplineJunction.cs
**Path:** `Assets/HOK/Scripts/Ferry/SplineJunction.cs`
**Type:** MonoBehaviour

**Purpose:** Marks a junction point on a spline where the raft can branch to an alternate route. Place on or under a GameObject with SplineComputer.

**Serialized Fields:**
| Field | Type | Default | Description |
|-------|------|---------|-------------|
| `junctionPercent` | float | 0.5f | Position on source spline (0-1) |
| `activationRange` | float | 0.05f | Range around junction for activation |
| `targetSpline` | SplineComputer | null | Spline to switch to |
| `targetEntryPercent` | float | 0f | Entry position on target spline |
| `requiredDirection` | int | 0 | Required travel direction (-1, 0, 1) |
| `indicatorObject` | GameObject | null | Visual indicator when available |
| `isAvailable` | bool | true | Whether junction is usable |
| `autoReturnAtStart` | bool | false | Auto-trigger when raft reaches percent 0 |

**Public API:**
| Member | Type | Description |
|--------|------|-------------|
| `SourceSpline` | SplineComputer (property) | The spline this junction belongs to |
| `JunctionPercent` | float (property) | Position on source spline |
| `ActivationRange` | float (property) | Detection range |
| `TargetSpline` | SplineComputer (property) | Destination spline |
| `TargetEntryPercent` | float (property) | Entry position on target |
| `RequiredDirection` | int (property) | Required travel direction |
| `IsAvailable` | bool (property) | Get/set availability |
| `AutoReturnAtStart` | bool (property) | Whether junction auto-triggers at spline start |
| `IsInRange(double, int)` | bool | Check if raft can use junction |
| `UpdateIndicator(bool)` | void | Show/hide visual indicator |
| `GetWorldPosition()` | Vector3 | World position of junction point |

**Scene Setup:**
1. Create SplineJunction as child of RiverSpline GameObject
2. Set `junctionPercent` to position along river (e.g., 0.7 for 70%)
3. Assign `targetSpline` to the branch spline
4. Set `targetEntryPercent` (usually 0 for start of branch)
5. Optionally assign `indicatorObject` for visual feedback
6. For dead-end branches: set `autoReturnAtStart = true` on the return junction

**Gizmos:**
- Green sphere at junction point (red if unavailable)
- Green line showing activation range
- Yellow line/sphere showing connection to target spline

---

#### FreeMovementController.cs
**Path:** `Assets/HOK/Scripts/Ferry/FreeMovementController.cs`
**Type:** MonoBehaviour

**Purpose:** Controls raft movement in free-navigation areas (Hub). Uses tank-style controls with direct world-space movement on the X-Z plane instead of spline sampling.

**Serialized Fields:**
| Field | Type | Default | Description |
|-------|------|---------|-------------|
| `maxSpeed` | float | 5f | Maximum forward/back speed |
| `acceleration` | float | 3f | Acceleration rate |
| `deceleration` | float | 5f | Deceleration rate |
| `rotationSpeed` | float | 55f | Rotation speed (degrees/sec) |
| `waterLevel` | float | 0.3f | Fixed Y position for the raft |
| `onRaftStartedMoving` | ScriptableEventNoParam | null | Event when movement begins |
| `onRaftStoppedMoving` | ScriptableEventNoParam | null | Event when movement stops |

**Public API:**
| Member | Type | Description |
|--------|------|-------------|
| `IsMoving` | bool (property) | True if raft is moving or turning |
| `CurrentSpeed` | float (property) | Current forward/back speed |
| `CurrentVelocity` | Vector3 (property) | World-space velocity vector |
| `OnMove(InputValue)` | void | Called by PlayerInput (SendMessages) |
| `SetMoveInput(Vector2)` | void | Direct input control |
| `SetPosition(Vector3)` | void | Set raft position directly |
| `SetPosition(float x, float z)` | void | Set X-Z position, Y at water level |

**Input Handling:**
- Uses `PlayerInput` component with `SendMessages` behavior
- **Tank-style controls** (different from river RaftController):
  - W/Up Arrow = move forward (in raft's facing direction)
  - S/Down Arrow = move backward
  - A/Left Arrow = rotate left
  - D/Right Arrow = rotate right
- Movement is relative to raft's facing direction, not screen

**Usage:**
```csharp
FreeMovementController raft = GetComponent<FreeMovementController>();

// Check movement
if (raft.IsMoving) { /* ... */ }

// Get velocity
Vector3 velocity = raft.CurrentVelocity;

// Teleport raft
raft.SetPosition(new Vector3(10f, 0.3f, 5f));
```

---

### HOK.Editor

#### SplineInitializer.cs
**Path:** `Assets/HOK/Scripts/Editor/SplineInitializer.cs`
**Type:** Static class (Editor only)

**Menu Items:**
| Path | Description |
|------|-------------|
| `HOK/Initialize River Spline` | Sets up selected SplineComputer with default river path |

**Public API:**
| Method | Description |
|--------|-------------|
| `InitializeRiverSpline(SplineComputer)` | Creates 5-point river along X axis |
| `InitializeMerchantBranchSpline(SplineComputer)` | Creates 4-point branch toward upper-left |

**Notes:**
- River spline: X=18 (entrance, right) to X=-17 (dock, left) at Z=2
- Branch spline: X=-5,Z=5 (junction) to X=-17,Z=14 (merchant dock)
- Y position is 0.3 (raft height above water)
- Uses BSpline type with SmoothMirrored points

---

## SOAP Assets

**Location:** `Assets/HOK/Data/`

### Variables

| Asset | Type | Path | Description |
|-------|------|------|-------------|
| `CurrentGameState` | IntVariable | `Variables/CurrentGameState.asset` | Current GameState as int |

### Events

| Asset | Type | Path | Description |
|-------|------|------|-------------|
| `OnGameStateChanged` | ScriptableEventInt | `Events/OnGameStateChanged.asset` | Fired when game state changes |

---

## Input Actions

**Asset:** `Assets/HOK/Settings/HOKInputActions.inputactions`

### Action Maps

#### Fishing
| Action | Type | Bindings | Description |
|--------|------|----------|-------------|
| `Cast` | Button | LMB, RT | Cast fishing line |
| `Reel` | Button (Hold) | LMB, RT | Reel in line |
| `Hook` | Button | Space, A | Set the hook |
| `AimDirection` | Vector2 | Mouse delta, Right Stick | Aim cast direction |

#### Ferry
| Action | Type | Bindings | Description |
|--------|------|----------|-------------|
| `Move` | Vector2 | WASD, Arrows, Left Stick | Move raft along river |
| `Interact` | Button | E, X | Interact with souls/docks |
| `AcceptPayment` | Button | F, Y | Accept soul payment |
| `RejectSoul` | Button | R, B | Reject non-paying soul |

#### UI
| Action | Type | Bindings | Description |
|--------|------|----------|-------------|
| `Navigate` | Vector2 | WASD, Arrows, D-Pad | Menu navigation |
| `Submit` | Button | Enter, Space, A | Confirm selection |
| `Cancel` | Button | Escape, B | Back/Cancel |
| `Pause` | Button | Escape, Start | Toggle pause menu |

---

## Scene Structure

### Acheron_Greybox Scene
**Path:** `Assets/HOK/Scenes/Rivers/Acheron_Greybox.unity`

**Hierarchy:**
```
Acheron_Greybox
  Main Camera (CinemachineBrain)
  Directional Light
  ---ENVIRONMENT---
    WaterPlane
    RiverBed
    Dock (main river dead end, left)
    RiverSpline (SplineComputer)
      Junction_ToMerchant
    MerchantBranchSpline (SplineComputer)
      Junction_ToRiver
    MerchantArea/
      MerchantDock
      MerchantStall
    BottomBank_Main
    MainUpperBank_Right
    MainUpperBank_Left
    UpperMerchantBank_Diagonal
    LowerMerchantBank_Diagonal
    UpperMerchantBank
    LowerMerchantBank
    MainRiverDeadEndBank
    BranchBank_DeadEndBank
  ---UNDERWATER---
  ---PLAYER---
    Raft (RaftController, PlayerInput)
    Kharon_Placeholder (FollowTarget)
    Scorch_Placeholder (FollowTarget)
  ---CAMERA---
    VCam_Navigation (CinemachineCamera, CinemachineFollow)
    VCam_Fishing (disabled)
  DontDestroyOnLoad
```

**Camera Setup:**
- VCam_Navigation follows Raft with offset (0, 10, -20)
- Camera at -Z side, looking toward +Z (Y rotation = 0)
- +X = screen right, -X = screen left, +Z = screen top, -Z = screen bottom

**Coordinate System (CORRECT):**
- Unity standard: +X=right, +Y=up, +Z=forward
- Side-scroller view: camera at -Z looking toward +Z
- River runs along X-axis: entrance (+X, right) to dock (-X, left)
- Branch forks toward +Z (top of screen)

**Character Offsets (from Raft):**
- Kharon: (0, 0.95, 0)
- Scorch: (0.8, 0.3, -0.8)

**Materials:**
- Bank_DarkGreen: All bank objects
- Dock_LightBrown: Dock, MerchantDock, MerchantStall
- Raft_Orange: Raft

---

### Central_Hub Scene
**Path:** `Assets/HOK/Scenes/Hubs/Central_Hub.unity`

**Hierarchy:**
```
Central_Hub
  Main Camera (CinemachineBrain)
  Directional Light
  ---ENVIRONMENT---
    WaterPlane
    DropOffDock (at +Z, north side)
    RiverEntrance_Acheron (at -Z, south)
    RiverEntrance_Styx (at +X, east)
    RiverEntrance_Lethe (at -X, west)
    RiverEntrance_Phlegethon (at +X, -Z, southeast)
    RiverEntrance_Cocytus (at -X, -Z, southwest)
  ---CAMERA---
    VCam_Hub (CinemachineCamera, CinemachineFollow, CinemachineRotationComposer)
  ---PLAYER---
    Raft (FreeMovementController, PlayerInput)
      Kharon_Placeholder (on cooler)
      Scorch_Placeholder (hidden next to cooler)
      Cooler_Placeholder
```

**Camera Setup:**
- VCam_Hub follows Raft with offset (0, 2.5, -8)
- Third-person behind/above view, looking at raft
- CinemachineFollow with LockToTargetWithWorldUp binding mode
- CinemachineRotationComposer for smooth look-at

**Character Layout (on Raft):**
- Cooler: local pos (-0.28, 1.25, 0.3), scale (0.3, 1.5, 0.071)
- Kharon: local pos (-0.28, 4.62, 0.3), scale (0.17, 2.6, 0.06) - standing ON cooler
- Scorch: local pos (-0.08, 1, 0.3), scale (0.16, 1, 0.0714286) - curled up next to cooler
- Height gag: Kharon appears ~7ft tall on cooler, actually ~5'7" when off

**Movement:**
- Uses FreeMovementController (tank controls)
- W/S = forward/back in raft's facing direction
- A/D = rotate left/right
- Not spline-locked like river scenes

**River Entrances (placeholders):**
- Acheron: (0, 0.5, -45) - south, active for MVP
- Styx: (45, 0.5, 0) - east
- Lethe: (-45, 0.5, 0) - west
- Phlegethon: (32, 0.5, -32) - southeast
- Cocytus: (-32, 0.5, -32) - southwest

---

## Conventions

### Coding Standards
- No `var` keyword - explicit types always
- No per-frame allocations or LINQ
- Prefer async/await (UniTask) over coroutines
- ASCII-only in code and comments

### Naming
- Namespaces: `HOK.<System>`
- Scripts: PascalCase
- Private fields: camelCase with no prefix
- Serialized fields: `[SerializeField] private Type fieldName`

### Scene Organization
- Use `---CATEGORY---` empty GameObjects for hierarchy organization
- Persistent objects go under `DontDestroyOnLoad`
- Cameras under `---CAMERA---`
- Player-related objects under `---PLAYER---`

---

## Dependencies Reference

### Dreamteck Splines
```csharp
using Dreamteck.Splines;

// Sample position on spline
SplineSample sample = splineComputer.Evaluate(percent); // percent is 0-1
Vector3 position = sample.position;
Vector3 forward = sample.forward;

// Get spline length
float length = splineComputer.CalculateLength();
```

### SOAP (Obvious.Soap)
```csharp
using Obvious.Soap;

// Variables
[SerializeField] private IntVariable myInt;
int value = myInt.Value;
myInt.Value = 5;

// Events
[SerializeField] private ScriptableEventNoParam myEvent;
myEvent.Raise();

[SerializeField] private ScriptableEventInt myIntEvent;
myIntEvent.Raise(42);
```

### Input System
```csharp
using UnityEngine.InputSystem;

// With PlayerInput component (SendMessages behavior)
public void OnMove(InputValue value)
{
    Vector2 input = value.Get<Vector2>();
}

public void OnFire(InputValue value)
{
    bool pressed = value.isPressed;
}
```

---

## File Locations Quick Reference

| Type | Location |
|------|----------|
| Scripts | `Assets/HOK/Scripts/<Namespace>/` |
| SOAP Variables | `Assets/HOK/Data/Variables/` |
| SOAP Events | `Assets/HOK/Data/Events/` |
| Input Actions | `Assets/HOK/Settings/` |
| Scenes | `Assets/HOK/Scenes/` |
| Prefabs | `Assets/HOK/Prefabs/` |
| Materials | `Assets/HOK/Art/Materials/` |

---

**End of Code Reference**
