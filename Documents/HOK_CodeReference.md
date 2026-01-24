# Hooked on Kharon - Code Reference

**Purpose:** Quick reference for existing code, APIs, and conventions. Check this before writing new code to avoid referencing non-existent classes or methods.

**Last Updated:** January 24, 2026

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
- `Obvious.Soap.ScriptableEventNoParam` - Movement events (optional)

**Serialized Fields:**
| Field | Type | Default | Description |
|-------|------|---------|-------------|
| `maxSpeed` | float | 5f | Maximum movement speed |
| `acceleration` | float | 3f | Acceleration rate |
| `deceleration` | float | 5f | Deceleration rate |
| `spline` | SplineComputer | null | River spline to follow |
| `onRaftStartedMoving` | ScriptableEventNoParam | null | Event when movement begins |
| `onRaftStoppedMoving` | ScriptableEventNoParam | null | Event when movement stops |

**Public API:**
| Member | Type | Description |
|--------|------|-------------|
| `IsMoving` | bool (property) | True if raft is moving |
| `CurrentSpeed` | float (property) | Current movement speed |
| `OnMove(InputValue)` | void | Called by PlayerInput (SendMessages) |
| `SetMoveInput(Vector2)` | void | Direct input control |
| `GetSplinePercent()` | double | Current position on spline (0-1) |
| `SetSplinePercent(double)` | void | Teleport to spline position |
| `SetSpline(SplineComputer)` | void | Assign new spline |

**Input Handling:**
- Uses `PlayerInput` component with `SendMessages` behavior
- Receives `OnMove(InputValue)` from Ferry action map
- D/Right Arrow = positive X = move toward +Z (higher spline %)
- A/Left Arrow = negative X = move toward -Z (lower spline %)

**Usage:**
```csharp
RaftController raft = GetComponent<RaftController>();

// Check movement
if (raft.IsMoving) { /* ... */ }

// Teleport to middle of river
raft.SetSplinePercent(0.5);

// Get current position
double percent = raft.GetSplinePercent();
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
| `InitializeRiverSpline(SplineComputer)` | Creates 5-point river along Z axis |

**Notes:**
- Creates spline from Z=-18 to Z=+16
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
    LeftBank
    RightBank
    Dock
    RiverSpline (SplineComputer)
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
- VCam_Navigation follows Raft with offset (15, 3, 0)
- Camera at +X side, looking toward -X (Y rotation = 270)
- +Z appears on right side of screen, -Z on left

**Character Offsets (from Raft):**
- Kharon: (0, 0.95, 0)
- Scorch: (0.8, 0.3, -0.8)

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
