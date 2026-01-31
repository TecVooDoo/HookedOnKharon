# Hooked on Kharon - Code Reference

Purpose: Quick reference for existing code, APIs, and conventions. Check this before writing new code to avoid referencing non-existent classes or methods.

Last Updated: 2026-01-31

---

## Namespaces

| Namespace | Purpose | Status |
|-----------|---------|--------|
| HOK.Core | Game state, managers, shared utilities | Active |
| HOK.Ferry | Raft movement, soul transport | Active |
| HOK.Fishing | Cast, reel, catch systems | Active |
| HOK.Companion | Scorch behavior | Planned |
| HOK.Progression | Unlocks, currency, codex | Planned |
| HOK.UI | Menus, HUD, codex display | Planned |
| HOK.Audio | Music, SFX managers | Planned |
| HOK.Data | ScriptableObject definitions | Active |
| HOK.Editor | Editor utilities, tools | Active |

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
| Instance | static GameManager | Singleton instance |
| CurrentState | GameState (property) | Current game state from SOAP variable |
| SetState(GameState) | void | Changes state, raises event |

**Usage:**

```csharp
GameState state = GameManager.Instance.CurrentState;
GameManager.Instance.SetState(GameState.Ferrying);
```

---

#### SceneTransitionManager.cs

**Path:** `Assets/HOK/Scripts/Core/SceneTransitionManager.cs`
**Type:** MonoBehaviour (Singleton, DontDestroyOnLoad)

**Purpose:**
Manages scene transitions between rivers and the hub. Coordinates raft spawning position, movement mode switching, and input action map switching.

**Dependencies:**
- `HOK.Ferry.RiverType`
- `HOK.Ferry.PlayerMovementController`
- `Dreamteck.Splines.SplineComputer`
- `UnityEngine.InputSystem.PlayerInput`
- `UnityEngine.SceneManagement`

**Serialized Fields:**

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| hubSceneName | string | "Central_Hub" | Name of the hub scene |
| transitionDelay | float | 0.2f | Delay before loading (for fade effects) |
| ferryActionMap | string | "Ferry" | Action map for river scenes |
| hubActionMap | string | "Hub" | Action map for hub scene |

**Public API:**

| Member | Type | Description |
|--------|------|-------------|
| Instance | static SceneTransitionManager | Singleton instance |
| HubSceneName | string (property) | Gets hub scene name |
| IsTransitioning | bool (property) | True during scene load |
| TransitionToHub(RiverType, Vector3, float) | void | Load hub at specified position/rotation |
| TransitionToRiver(RiverType, string, float) | void | Load river scene at spline percent |

**Usage:**

```csharp
// From river exit (RiverExit component)
SceneTransitionManager.Instance.TransitionToHub(
    RiverType.Acheron,
    new Vector3(0, 0, -10),  // Hub spawn position
    180f                      // Facing direction
);

// From hub entrance (RiverEntrance component)
SceneTransitionManager.Instance.TransitionToRiver(
    RiverType.Acheron,
    "Acheron_Greybox",
    0.0f  // Spawn at river entrance/mouth
);
```

**Notes:**
- Create as prefab and place in every scene (singleton pattern handles duplicates)
- Automatically switches input action maps based on scene type
- Automatically switches PlayerMovementController mode (Free for Hub, Spline for Rivers)
- Stores spawn data before load, applies after scene loaded
- Finds SplineComputer in scene and assigns to PlayerMovementController on river spawn

---

#### FollowTarget.cs

**Path:** `Assets/HOK/Scripts/Core/FollowTarget.cs`
**Type:** MonoBehaviour
**Attributes:** ExecuteAlways

**Serialized Fields:**

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| target | Transform | null | Transform to follow |
| offset | Vector3 | (0, 0.5, 0) | Position offset from target |
| followRotation | bool | false | Match target rotation |

**Public API:**

| Method | Description |
|--------|-------------|
| SetTarget(Transform) | Sets the follow target |
| SetOffset(Vector3) | Sets the position offset |

**Usage:**

```csharp
FollowTarget follower = GetComponent<FollowTarget>();
follower.SetTarget(raftTransform);
follower.SetOffset(new Vector3(0f, 0.95f, 0f));
```

**Notes:**
- Runs in both Edit and Play mode due to ExecuteAlways
- Updates in LateUpdate to run after target moves
- Used for Kharon/Scorch to follow raft without inheriting scale

---

### HOK.Ferry

#### RiverType.cs (Enum)

**Path:** `Assets/HOK/Scripts/Ferry/RiverEntrance.cs` (defined in same file)
**Type:** Enum

```csharp
public enum RiverType
{
    Acheron,    // Pain/Woe - Beginner
    Styx,       // Hate - Early
    Lethe,      // Forgetfulness - Mid
    Phlegethon, // Fire - Advanced
    Cocytus     // Lamentation - Endgame
}
```

---

#### RaftController.cs

**Path:** `Assets/HOK/Scripts/Ferry/RaftController.cs`
**Type:** MonoBehaviour

**Purpose:**
- Moves the raft along a Dreamteck SplineComputer (river spline)
- Supports spline junctions for branching to merchant splines
- Manual branch: button press while in range; if stopped, next move occurs on branch
- Auto return: junctions configured as auto (RequiresButtonPress == false) trigger automatically while moving

**Dependencies:**
- `Dreamteck.Splines.SplineComputer`
- `UnityEngine.InputSystem.InputValue`
- `Obvious.Soap.ScriptableEventNoParam` (optional)
- `HOK.Ferry.SplineJunction`

**Serialized Fields:**

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| maxSpeed | float | 5f | Maximum movement speed |
| acceleration | float | 3f | Acceleration rate |
| deceleration | float | 5f | Deceleration rate |
| spline | SplineComputer | null | River spline to follow |
| startingPercent | double | 0.5 | Starting spline percent |
| junctions | List\<SplineJunction\> | empty | Junctions on current spline (auto-populated) |
| onRaftStartedMoving | ScriptableEventNoParam | null | Event when movement begins |
| onRaftStoppedMoving | ScriptableEventNoParam | null | Event when movement stops |
| onJunctionAvailable | ScriptableEventNoParam | null | Event when entering junction range |
| onJunctionTaken | ScriptableEventNoParam | null | Event when taking a junction |

**Public API:**

| Member | Type | Description |
|--------|------|-------------|
| IsMoving | bool (property) | True if raft is moving |
| CurrentSpeed | float (property) | Current movement speed |
| CurrentSpline | SplineComputer (property) | The spline being followed |
| ActiveJunction | SplineJunction (property) | Junction currently in range (if any) |
| IsJunctionAvailable | bool (property) | True if a junction is currently available |
| OnMove(InputValue) | void | PlayerInput callback: sets Move vector |
| OnTakeJunction(InputValue) | void | PlayerInput callback: manual junction activation |
| SetMoveInput(Vector2) | void | Direct movement input setter |
| SetTakeJunctionPressed(bool) | void | Manual activation hook (tests / non-InputValue use) |
| GetSplinePercent() | double | Current position on spline (0-1) |
| SetSplinePercent(double) | void | Teleport to spline percent |
| SetSpline(SplineComputer) | void | Assign new spline and refresh junctions |
| SetSpline(SplineComputer, double) | void | Assign new spline at entry percent |

**Input Handling (Authoritative):**

Uses PlayerInput with SendMessages or UnityEvents. Movement and junction selection are intentionally decoupled.

**Ferry Action Map (expected):**

**Move (Vector2):**
- Controls raft movement along spline only
- Camera at -Z looking toward +Z: +X is screen right, -X is screen left
- A / Left Arrow: move toward dock (left, higher spline %)
- D / Right Arrow: move toward entrance (right, lower spline %)

**TakeJunction (Button):**
- Manual branch activation
- Press while in range to arm the branch
- If stopped: next movement occurs on branch
- If moving: junction is taken immediately

**Automatic Junctions:**

Junctions with `RequiresButtonPress == false` are auto-taken when:
- Raft is moving (or move intent exists), and
- Cooldown is clear

Used for merchant -> main river return. Cooldown prevents ping-pong.

**Usage:**

```csharp
RaftController raft = GetComponent<RaftController>();

if (raft.IsMoving)
{
    // ...
}

if (raft.IsJunctionAvailable)
{
    SplineJunction junction = raft.ActiveJunction;
}

raft.SetSplinePercent(0.5);
raft.SetSpline(otherSpline, 0.0);
```

**Notes:**
- Junction discovery uses `GetComponentsInChildren<SplineJunction>()` on the active spline
- Authoring rule for MVP: SplineJunction objects must be children of their source SplineComputer
- Junction switching projects current world position onto target spline to find entry percent
- `targetEntryPercent` exists on SplineJunction but is currently unused by RaftController

---

#### SplineJunction.cs

**Path:** `Assets/HOK/Scripts/Ferry/SplineJunction.cs`
**Type:** MonoBehaviour

**Purpose:**
Marks a junction point on a spline where the raft can branch to an alternate route. Place on or under a GameObject with SplineComputer (recommended: as a child of the spline).

**Serialized Fields:**

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| junctionPercent | float | 0.5 | Position on source spline (0-1) |
| activationRange | float | 0.05 | Range around junction for activation (percent distance) |
| targetSpline | SplineComputer | null | Spline to switch to |
| targetEntryPercent | float | 0.0 | Entry position on target spline (currently unused by RaftController) |
| requiredDirection | int | 0 | Required travel direction (-1, 0, 1) |
| requiresButtonPress | bool | true | If true, player must press TakeJunction. If false, auto-take when in range (return junctions) |
| indicatorObject | GameObject | null | Visual indicator when available |
| isAvailable | bool | true | Whether junction is usable |
| autoReturnAtStart | bool | false | Legacy: auto trigger at spline percent 0 (prefer requiresButtonPress=false + activationRange) |

**Public API:**

| Member | Type | Description |
|--------|------|-------------|
| SourceSpline | SplineComputer (property) | Spline this junction belongs to |
| JunctionPercent | float (property) | Position on source spline |
| ActivationRange | float (property) | Detection range |
| TargetSpline | SplineComputer (property) | Destination spline |
| TargetEntryPercent | float (property) | Target entry percent (currently unused) |
| RequiredDirection | int (property) | Required travel direction |
| RequiresButtonPress | bool (property) | Manual vs auto-take |
| IsAvailable | bool (property) | Get/set availability |
| AutoReturnAtStart | bool (property) | Legacy auto-return flag |
| IsInRange(double, int) | bool | Check if raft can use junction |
| UpdateIndicator(bool) | void | Show/hide visual indicator |

**Scene Setup (Authoritative):**

1. Create SplineJunction as a child of the source RiverSpline GameObject (SplineComputer)
2. Set `junctionPercent` to position along source spline
3. Assign `targetSpline` to the branch spline
4. Optionally assign `indicatorObject` for visual feedback
5. Configure behavior:
   - Main -> Merchant branch junction: `requiresButtonPress = true`
   - Merchant -> Main river return junction: `requiresButtonPress = false` (auto)

**Legacy note:** `autoReturnAtStart` exists but is not the recommended return behavior for MVP.

---

#### PlayerMovementController.cs

**Path:** `Assets/HOK/Scripts/Ferry/PlayerMovementController.cs`
**Type:** MonoBehaviour

**Purpose:**
Unified movement controller that handles both free movement (Hub) and spline-based movement (Rivers). Replaces separate RaftController and FreeMovementController. Allows same raft prefab to work in any scene.

**Dependencies:**
- `Dreamteck.Splines.SplineComputer`
- `UnityEngine.InputSystem.InputValue`
- `Obvious.Soap.ScriptableEventNoParam` (optional)
- `HOK.Ferry.SplineJunction`

**Movement Modes:**

| Mode | Description |
|------|-------------|
| Free | Tank-style movement on X-Z plane (Hub scenes) |
| Spline | Movement constrained to spline (River scenes) |

**Serialized Fields:**

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| currentMode | MovementMode | Free | Current movement mode |
| freeMaxSpeed | float | 5f | Max speed in Free mode (Hub) |
| freeAcceleration | float | 3f | Acceleration in Free mode |
| freeDeceleration | float | 5f | Deceleration in Free mode |
| rotationSpeed | float | 55f | Rotation speed (Free mode) |
| waterLevel | float | 0.3f | Fixed Y position (Free mode) |
| splineMaxSpeed | float | 1f | Max speed in Spline mode (River) |
| splineAcceleration | float | 1f | Acceleration in Spline mode |
| splineDeceleration | float | 3f | Deceleration in Spline mode |
| spline | SplineComputer | null | River spline (Spline mode) |
| startingPercent | double | 0.0 | Starting spline percent |
| splineRotationSpeed | float | 5f | How fast raft aligns with spline |
| autoDetectSplineOnStart | bool | true | Auto-find spline tagged "MainRiver" |
| junctions | List\<SplineJunction\> | empty | Available junctions |
| onRaftStartedMoving | ScriptableEventNoParam | null | Movement started event |
| onRaftStoppedMoving | ScriptableEventNoParam | null | Movement stopped event |
| onJunctionAvailable | ScriptableEventNoParam | null | Junction available event |
| onJunctionTaken | ScriptableEventNoParam | null | Junction taken event |

**Public API:**

| Member | Type | Description |
|--------|------|-------------|
| CurrentMode | MovementMode (property) | Current movement mode |
| IsMoving | bool (property) | True if raft is moving |
| CurrentSpeed | float (property) | Current movement speed |
| CurrentVelocity | Vector3 (property) | World-space velocity (Free mode) |
| CurrentSpline | SplineComputer (property) | The spline being followed |
| ActiveJunction | SplineJunction (property) | Junction currently in range |
| IsJunctionAvailable | bool (property) | True if junction available |
| SetMode(MovementMode) | void | Switch movement mode |
| SetFreeMode() | void | Switch to free movement |
| SetSplineMode(SplineComputer, double) | void | Switch to spline mode |
| OnMove(InputValue) | void | PlayerInput callback |
| OnTakeJunction(InputValue) | void | Junction activation (Spline mode). Calls ArmManualJunction on press; cooldown prevents spam. |
| SetMoveInput(Vector2) | void | Direct input setter |
| SetPosition(Vector3) | void | Set position (Free mode) |
| GetSplinePercent() | double | Get spline position |
| SetSplinePercent(double) | void | Set spline position |
| SetSpline(SplineComputer) | void | Change spline |
| SetSpline(SplineComputer, double) | void | Change spline with entry percent |

**Usage:**

```csharp
PlayerMovementController movement = GetComponent<PlayerMovementController>();

// Check mode
if (movement.CurrentMode == MovementMode.Free)
{
    movement.SetPosition(new Vector3(0, 0, -10));
}

// Switch to spline mode (typically done by SceneTransitionManager)
movement.SetSplineMode(riverSpline, 0.0);

// Switch to free mode
movement.SetFreeMode();
```

**Notes:**
- Combines RaftController (spline) and FreeMovementController (free) into one script
- Mode is typically set by SceneTransitionManager on scene load
- Same raft prefab can be used in both Hub and River scenes
- Junction behavior only active in Spline mode
- Junction input uses cooldown-based spam prevention (1.5s default) instead of tracking button state
- OnTakeJunction simply arms the manual junction on any press; the cooldown prevents double-triggering

---

#### FreeMovementController.cs (DEPRECATED)

**Path:** `Assets/HOK/Scripts/Ferry/FreeMovementController.cs`
**Type:** MonoBehaviour
**Status:** DEPRECATED - Use PlayerMovementController instead

**Purpose:**
Legacy controller for free movement in Hub. Replaced by PlayerMovementController's Free mode.

---

#### RaftController.cs (DEPRECATED)

**Path:** `Assets/HOK/Scripts/Ferry/RaftController.cs`
**Type:** MonoBehaviour
**Status:** DEPRECATED - Use PlayerMovementController instead

**Purpose:**
Legacy controller for spline movement in Rivers. Replaced by PlayerMovementController's Spline mode.

---

#### RiverEntrance.cs

**Path:** `Assets/HOK/Scripts/Ferry/RiverEntrance.cs`
**Type:** MonoBehaviour
**Requires:** Collider (set as trigger)

**Purpose:**
Marks a river entrance in the Central Hub. When the raft enters the trigger, transitions to the target river scene.

**Serialized Fields:**

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| targetSceneName | string | "" | Scene to load |
| riverType | RiverType | Acheron | Which river this leads to |
| spawnPercent | float | 0.0 | Where on river spline to spawn (0=entrance, 1=dock) |
| indicatorObject | GameObject | null | Visual indicator |
| isUnlocked | bool | true | Whether entrance is usable |

**Public API:**

| Member | Type | Description |
|--------|------|-------------|
| River | RiverType (property) | The river type |
| IsUnlocked | bool (property) | Get/set availability |
| TargetSceneName | string (property) | Scene to load |
| SpawnPercent | float (property) | Spawn position on river |
| UpdateIndicator(bool) | void | Show/hide visual |

**Scene Setup:**
1. Place at river entrance position in Hub scene
2. Add Collider component (will be auto-set as trigger)
3. Configure targetSceneName to match river scene name exactly
4. Set spawnPercent to 0.0 (river entrance/mouth where player enters)
5. Only unlock Acheron for MVP; others remain locked

---

#### RiverExit.cs

**Path:** `Assets/HOK/Scripts/Ferry/RiverExit.cs`
**Type:** MonoBehaviour
**Requires:** Collider (set as trigger)

**Purpose:**
Marks a river exit point at the river entrance/mouth (NOT the dock). Place near spline percent 0 where players exit back to the hub after picking up souls.

**Serialized Fields:**

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| riverType | RiverType | Acheron | Which river this exit is in |
| hubSpawnPosition | Vector3 | (0,0,0) | Where to spawn in hub (X,Z used) |
| hubSpawnRotationY | float | 0 | Facing direction in hub |
| indicatorObject | GameObject | null | Visual indicator |

**Public API:**

| Member | Type | Description |
|--------|------|-------------|
| River | RiverType (property) | The river type |
| UpdateIndicator(bool) | void | Show/hide visual |

**Scene Setup:**
1. Place near river entrance/mouth (spline percent ~0, +X side in Acheron)
2. Add Collider component (will be auto-set as trigger)
3. Set hubSpawnPosition to match corresponding RiverEntrance position in Hub
4. Set hubSpawnRotationY so raft faces away from entrance (into hub)

**River Flow:**
- Hub → RiverEntrance → Spawn at river mouth (percent 0)
- Player travels to dock (percent 1) to pick up souls
- Player travels back to river mouth (percent 0)
- River mouth → RiverExit → Hub

**Gizmos:**
- Cyan sphere at exit position
- Yellow arrow showing hub facing direction
- Magenta line to hub spawn position

---

### HOK.Editor

#### SplineInitializer.cs

**Path:** `Assets/HOK/Scripts/Editor/SplineInitializer.cs`
**Type:** Static class (Editor only)

**Menu Items:**

| Path | Description |
|------|-------------|
| HOK/Initialize River Spline | Sets up selected SplineComputer with default river path |

**Public API:**

| Method | Description |
|--------|-------------|
| InitializeRiverSpline(SplineComputer) | Creates 5-point river along X axis |
| InitializeMerchantBranchSpline(SplineComputer) | Creates 4-point branch toward upper-left |

**Notes:**
- River spline: X=18 (entrance, right) to X=-17 (dock, left) at Z=2
- Branch spline: X=-5,Z=5 (junction) to X=-17,Z=14 (merchant dock)
- Y position is 0.3 (raft height above water)
- Uses BSpline type with SmoothMirrored points

---

#### FishDefinitionCreator.cs

**Path:** `Assets/HOK/Scripts/Editor/FishDefinitionCreator.cs`
**Type:** Static class (Editor only)

**Menu Items:**

| Path | Description |
|------|-------------|
| HOK/Create Acheron Fish Definitions | Creates 10 fish for Acheron river |

**Purpose:**
Creates the MVP fish definitions for Acheron river. Creates 6 Common, 3 Uncommon, and 1 Rare fish in `Assets/HOK/Data/Definitions/Fish/{Common,Uncommon,Rare}/`.

**Fish Created:**
- Common: WoePerch, PainCarp, SorrowTrout, LamentBass, AcheronEel, GloomGuppy
- Uncommon: GriefSalmon, RegretPike, MourningSturgeon
- Rare: EmberfinPhantom (requires Scorch proximity 0.7+)

**Notes:**
- Uses SerializedObject to set private fields on FishDefinition
- Skips existing fish to avoid overwrites
- Creates folder structure if needed

---

### HOK.Fishing

#### FishingState.cs

**Path:** `Assets/HOK/Scripts/Fishing/FishingState.cs`
**Type:** Enum

```csharp
public enum FishingState
{
    Inactive = 0,       // Not fishing - movement enabled
    Idle = 1,           // Ready to cast - can aim
    Casting = 2,        // Cast animation playing
    LineInWater = 3,    // Waiting for fish bite
    FishBiting = 4,     // Fish is biting - hook window active
    Hooked = 5,         // Fish hooked - reeling phase
    CatchResolution = 6 // Success/fail animation
}
```

---

#### FishingController.cs

**Path:** `Assets/HOK/Scripts/Fishing/FishingController.cs`
**Type:** MonoBehaviour

**Purpose:**
Main fishing controller that manages the fishing state machine, input handling, and coordinates all fishing subsystems (casting, hook timing, reeling).

**Dependencies:**
- `Obvious.Soap.IntVariable` - CurrentFishingState, LineTension, HookDepth, ScorchProximity
- `Obvious.Soap.ScriptableEventInt` - OnGameStateChanged, OnFishingStateChanged
- `Obvious.Soap.ScriptableEventNoParam` - OnCastStarted, OnFishBite, etc.
- `HOK.Data.FishDefinition`

**Serialized Fields:**

| Field | Type | Description |
|-------|------|-------------|
| currentFishingStateVariable | IntVariable | Current fishing state as int |
| lineTensionVariable | FloatVariable | Line tension (0-1) |
| hookDepthVariable | FloatVariable | Hook depth (0-1) for camera |
| scorchProximityVariable | FloatVariable | Scorch proximity (0-1) |
| fishPool | List\<FishPoolEntry\> | Available fish for this spot |
| rodTipTransform | Transform | Where line originates |
| hookTransform | Transform | Hook/lure position |

**Public API:**

| Member | Type | Description |
|--------|------|-------------|
| CurrentState | FishingState (property) | Current fishing state |
| IsFishing | bool (property) | True if not Inactive |
| HookedFish | FishDefinition (property) | Currently hooked fish |
| LineTension | float (property) | Current tension (0-1) |
| HookDepth | float (property) | Current hook depth (0-1) |
| OnCast(InputValue) | void | PlayerInput: Cast button |
| OnReel(InputValue) | void | PlayerInput: Reel button (hold) |
| OnHook(InputValue) | void | PlayerInput: Hook button |
| OnAimDirection(InputValue) | void | PlayerInput: Aim vector |
| OnStartFishing(InputValue) | void | PlayerInput: Enter fishing mode |
| OnCancelFishing(InputValue) | void | PlayerInput: Exit fishing mode |
| StartFishing() | void | Manual fishing mode entry |
| StopFishing() | void | Manual fishing mode exit |
| SetScorchProximity(float) | void | Set Scorch proximity value |

**State Flow:**
```
Inactive -> Idle (GameState.Fishing)
Idle -> Casting (Cast input)
Casting -> LineInWater (cast completes)
LineInWater -> FishBiting (bite timer)
FishBiting -> Hooked (successful hook)
FishBiting -> LineInWater (missed hook)
Hooked -> CatchResolution (catch or escape)
CatchResolution -> Idle (reset)
```

---

#### FishingLineRenderer.cs

**Path:** `Assets/HOK/Scripts/Fishing/FishingLineRenderer.cs`
**Type:** MonoBehaviour
**Requires:** LineRenderer

**Purpose:**
Renders the fishing line from rod tip to hook position. Updates HookDepth SOAP variable for camera tracking.

**Serialized Fields:**

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| rodTipTransform | Transform | null | Rod tip position |
| hookTransform | Transform | null | Hook position |
| currentFishingStateVariable | IntVariable | null | Controls visibility |
| hookDepthVariable | FloatVariable | null | Updated based on hook Y |
| waterSurfaceY | float | 0.3 | Water surface Y position |
| maxDepth | float | 10 | Max depth for normalization |
| lineSegments | int | 20 | Line smoothness |
| lineSag | float | 0.5 | Line curve amount |

**Public API:**

| Method | Description |
|--------|-------------|
| SetWaterSurface(float) | Set water Y position |
| SetLineSag(float) | Set line curve amount |
| SetLineVisible(bool) | Force show/hide |

---

#### FishingCameraController.cs

**Path:** `Assets/HOK/Scripts/Fishing/FishingCameraController.cs`
**Type:** MonoBehaviour

**Purpose:**
Controls camera during fishing with Cast n' Chill style depth tracking. Camera transitions from 2.5D above-water view to near-2D side-view as hook depth increases. Waterline rises on screen as camera descends.

**Serialized Fields:**

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| currentFishingStateVariable | IntVariable | null | Fishing state |
| hookDepthVariable | FloatVariable | null | Hook depth (0-1) |
| fishingCamera | CinemachineCamera | null | Fishing view camera |
| navigationCamera | CinemachineCamera | null | Default camera |
| cameraYOffsetMin | float | 5 | Y offset at min depth |
| cameraYOffsetMax | float | -8 | Y offset at max depth |
| cameraAngleMin | float | 30 | Pitch at min depth (2.5D) |
| cameraAngleMax | float | 5 | Pitch at max depth (side-view) |
| positionSmoothSpeed | float | 3 | Position transition speed |
| rotationSmoothSpeed | float | 3 | Rotation transition speed |
| followTarget | Transform | null | Target to follow |

**Public API:**

| Method | Description |
|--------|-------------|
| SetFollowTarget(Transform) | Set follow target |
| SetWaterSurface(float) | Set water Y position |
| SnapToTarget() | Immediate camera snap |
| GetCurrentNormalizedDepth() | Get depth 0-1 |

**Camera Behavior:**
- Depth 0 (surface): Camera above water, 30deg angle (2.5D view)
- Depth 0.5: Waterline at screen center, 17deg angle
- Depth 1.0 (max): Camera below water, 5deg angle (near side-view)

---

### HOK.Data

#### FishDefinition.cs

**Path:** `Assets/HOK/Scripts/Data/FishDefinition.cs`
**Type:** ScriptableObject
**Menu:** HOK/Definitions/Fish Definition

**Purpose:**
Defines a fish species with all properties for fishing gameplay.

**Serialized Fields:**

| Field | Type | Range | Description |
|-------|------|-------|-------------|
| fishName | string | - | Display name |
| description | string | - | Description text |
| icon | Sprite | - | UI icon |
| prefab | GameObject | - | 3D model prefab |
| rarity | FishRarity | - | Common/Uncommon/Rare/Legendary |
| baseCatchDifficulty | float | 0-1 | Catch difficulty modifier |
| tensionMultiplier | float | 0.5-5 | Tension build rate |
| escapeStrength | float | 0.1-5 | How hard fish pulls |
| hookWindowSize | float | 0.1-1 | Hook timing leniency |
| minBiteTime | float | 1-30 | Min wait for bite |
| maxBiteTime | float | 1-60 | Max wait for bite |
| biteWindowDuration | float | 0.3-3 | Hook window duration |
| minScorchProximity | float | 0-1 | Required Scorch proximity |
| requiredRiver | string | - | River restriction (empty=any) |
| baseObols | int | 1-1000 | Currency reward |
| isCollectible | bool | - | Goes to collection |

**Public API:**

| Method | Description |
|--------|-------------|
| GetRandomBiteTime() | Random time in min/max range |
| CanSpawnWithScorchProximity(float) | Check proximity requirement |
| CanSpawnInRiver(string) | Check river requirement |

**FishRarity Enum:**
```csharp
public enum FishRarity
{
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Legendary = 3
}
```

---

## SOAP Assets

**Location:** `Assets/HOK/Data/`

### Variables

| Asset | Type | Path | Description |
|-------|------|------|-------------|
| CurrentGameState | IntVariable | Variables/CurrentGameState.asset | Current GameState as int |
| CurrentFishingState | IntVariable | Variables/Fishing/CurrentFishingState.asset | Current FishingState as int |
| LineTension | FloatVariable | Variables/Fishing/LineTension.asset | Line tension 0-1 |
| CastPower | FloatVariable | Variables/Fishing/CastPower.asset | Cast power 0-1 |
| HookDepth | FloatVariable | Variables/Fishing/HookDepth.asset | Hook depth 0-1 for camera |
| ScorchProximity | FloatVariable | Variables/Fishing/ScorchProximity.asset | Scorch proximity 0-1 |

### Events

| Asset | Type | Path | Description |
|-------|------|------|-------------|
| OnGameStateChanged | ScriptableEventInt | Events/OnGameStateChanged.asset | Fired when game state changes |
| OnFishingStateChanged | ScriptableEventInt | Events/Fishing/OnFishingStateChanged.asset | Fishing state changes |
| OnCastStarted | ScriptableEventNoParam | Events/Fishing/OnCastStarted.asset | Cast initiated |
| OnFishBite | ScriptableEventNoParam | Events/Fishing/OnFishBite.asset | Fish is biting |
| OnFishHooked | ScriptableEventNoParam | Events/Fishing/OnFishHooked.asset | Successfully hooked |
| OnFishCaught | ScriptableEventNoParam | Events/Fishing/OnFishCaught.asset | Fish caught |
| OnFishLost | ScriptableEventNoParam | Events/Fishing/OnFishLost.asset | Fish escaped |

---

## Input Actions

**Asset:** `Assets/HOK/Settings/HOKInputActions.inputactions`

### Action Maps

| Map | Status | Description |
|-----|--------|-------------|
| Fishing | Active | Fishing-related inputs |
| Ferry | Active | River navigation |
| UI | Planned | Menu navigation |
| Hub | Active | Free movement in hub |

### Fishing Actions

| Action | Type | Bindings | Description |
|--------|------|----------|-------------|
| Cast | Button | LMB (press), RT | Cast the line |
| Reel | Button | LMB (hold), RT | Reel in the line |
| Hook | Button | Space, Y/South | Attempt to hook fish |
| AimDirection | Vector2 | Mouse Position, Left Stick | Aim direction for cast |

### Ferry Actions

| Action | Type | Bindings | Description |
|--------|------|----------|-------------|
| Move | Vector2 | WASD, Arrows, Left Stick | Move raft along river |
| Interact | Button | E, X | Interact with souls/docks |
| AcceptPayment | Button | F, Y | Accept soul payment |
| RejectSoul | Button | R, B | Reject non-paying soul |
| TakeJunction | Button | W, Up Arrow | Manual branch activation |
| StartFishing | Button | F, buttonSouth | Enter fishing mode |

---

## Scene Structure

### Acheron_Greybox Scene

**Path:** `Assets/HOK/Scenes/Rivers/Acheron_Greybox.unity`

**Hierarchy:**
- RiverSpline (SplineComputer)
  - Junction_ToMerchant (SplineJunction, `requiresButtonPress = true`)
- MerchantBranchSpline (SplineComputer)
  - Junction_ToRiver (SplineJunction, `requiresButtonPress = false`)

**Camera / Coordinate System:**
- Camera at -Z looking toward +Z
- +X = screen right, -X = screen left
- River runs along X axis: entrance (+X, right) -> dock (-X, left)
- Branch forks toward +Z (top of screen)

**Character Offsets (from Raft):**
- Kharon: (0, 0.95, 0)
- Scorch: (0.8, 0.3, -0.8)

**Hierarchy (required for transitions):**
- ---PLAYER---
  - Raft (with PlayerMovementController, Rigidbody, Collider, PlayerInput)
  - Kharon (with FollowTarget)
  - Scorch (with FollowTarget)
- ---MANAGERS---
  - SceneTransitionManager (prefab instance)
- ---ENVIRONMENT---
  - RiverSpline (SplineComputer)
    - Junction_ToMerchant (SplineJunction)
  - MerchantBranchSpline (SplineComputer)
    - Junction_ToRiver (SplineJunction)
  - RiverExit (RiverExit component near river entrance/mouth at +X, triggers transition to Hub)

---

### Central_Hub Scene

**Path:** `Assets/HOK/Scenes/Hubs/Central_Hub.unity`

Hub uses PlayerMovementController in Free mode with tank controls.

**Hierarchy (required for transitions):**
- ---PLAYER---
  - Raft (with PlayerMovementController, Rigidbody, Collider, PlayerInput)
  - Kharon (with FollowTarget)
  - Scorch (with FollowTarget)
- ---MANAGERS---
  - SceneTransitionManager (prefab instance)
- ---ENVIRONMENT---
  - RiverEntrances
    - Acheron_Entrance (RiverEntrance, targetSceneName="Acheron_Greybox", isUnlocked=true)
    - Styx_Entrance (RiverEntrance, isUnlocked=false)
    - Lethe_Entrance (RiverEntrance, isUnlocked=false)
    - Phlegethon_Entrance (RiverEntrance, isUnlocked=false)
    - Cocytus_Entrance (RiverEntrance, isUnlocked=false)

---

### Bootstrap Scene

**Path:** `Assets/HOK/Scenes/_Bootstrap/Bootstrap.unity`

**Purpose:** Initialization scene that creates persistent managers.

**Hierarchy:**
- GameManager (with GameManager component, DontDestroyOnLoad)
- SceneTransitionManager (prefab instance - singleton handles duplicates)

**Notes:**
- Must be first scene in Build Settings (index 0)
- GameManager persists across all scene loads
- SceneTransitionManager is now a prefab placed in every scene for easier testing
- Singleton pattern in SceneTransitionManager ensures only one instance exists
- After initialization, load first gameplay scene (Acheron or Hub)

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
- Persistent objects go under DontDestroyOnLoad
- Cameras under `---CAMERA---`
- Player-related objects under `---PLAYER---`

---

## Dependencies Reference

### Dreamteck Splines

```csharp
using Dreamteck.Splines;

SplineSample sample = splineComputer.Evaluate(percent); // percent is 0-1
Vector3 position = sample.position;
Vector3 forward = sample.forward;

float length = splineComputer.CalculateLength();
```

### SOAP (Obvious.Soap)

```csharp
using Obvious.Soap;

[SerializeField] private IntVariable myInt;
int value = myInt.Value;
myInt.Value = 5;

[SerializeField] private ScriptableEventNoParam myEvent;
myEvent.Raise();

[SerializeField] private ScriptableEventInt myIntEvent;
myIntEvent.Raise(42);
```

### Input System

```csharp
using UnityEngine.InputSystem;

public void OnMove(InputValue value)
{
    Vector2 input = value.Get<Vector2>();
}

public void OnTakeJunction(InputValue value)
{
    // Simply arm on press - cooldown prevents spam
    if (value.isPressed)
    {
        ArmManualJunction();
    }
}
```

---

## File Locations Quick Reference

| Type | Location |
|------|----------|
| Scripts | Assets/HOK/Scripts/\<Namespace\>/ |
| SOAP Variables | Assets/HOK/Data/Variables/ |
| SOAP Events | Assets/HOK/Data/Events/ |
| Fish Definitions | Assets/HOK/Data/Definitions/Fish/{Common,Uncommon,Rare}/ |
| Input Actions | Assets/HOK/Settings/ |
| Scenes | Assets/HOK/Scenes/ |
| Prefabs | Assets/HOK/Prefabs/ |
| Materials | Assets/HOK/Art/Materials/ |

---

**End of Code Reference**
