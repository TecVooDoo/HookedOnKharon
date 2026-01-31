# Hooked on Kharon: Reeling in the Underworld - Project Status

**Project:** Hooked on Kharon: Reeling in the Underworld
**Developer:** TecVooDoo LLC / Rune (Stephen Brandon)
**3D Artist:** Son
**Platform:** Unity (Version TBD)
**Source:** `E:\Unity\Hooked On Kharon`
**Repository:** https://github.com/TecVooDoo/HookedOnKharon
**Document Version:** 17
**Last Updated:** January 30, 2026

**Archive:** `HookedOnKharon_Status_Archive.md` - Historical designs, old version history, completed phase details (create when needed)

---

## Quick Context

**What is this game?** A cozy low-poly 3D fishing sim set in the Greek Underworld. Play as Kharon, the eternal ferryman who just wants to fish in peace - but souls keep interrupting. Ferry the dead (grudgingly), collect obols, and upgrade gear across five legendary rivers.

**Tagline:** "Even death needs a break."

**Current Phase:** Pre-Production

**Last Session (Jan 30, 2026):** Fixed spline speed inconsistency - was using hardcoded divisor instead of actual spline length, causing different speeds on different splines. Made junction cooldown configurable in Inspector (default 2.5s). Updated default spline speed to 4 units/sec.

---

## Development Priorities (Ordered)

1. **Use installed assets first** - before writing custom code, check if an installed package already provides the functionality (see Dependencies/Packages section)
2. **SOLID principles** - single responsibility, open/closed, Liskov substitution, interface segregation, dependency inversion
3. **Memory efficiency** - no per-frame allocations, no per-frame LINQ, object pooling where appropriate
4. **Clean code** - readable, maintainable, consistent formatting
5. **Self-documenting code** - clear naming over comments; if code needs a comment, consider refactoring first
6. **Platform best practices** - Unity > C#, Cloudflare/Supabase > HTML/JS (platform-specific wins over language-generic)

---

## Active TODO

### Immediate (MVP Planning)
- [x] Select Unity version (Unity 6 - 6000.0.x)
- [x] Create Unity project structure at `E:\Unity\Hooked On Kharon`
- [x] Install core packages
- [x] Create folder structure under Assets/HOK
- [x] Set up Input Actions (HOKInputActions.inputactions)
- [x] Create Bootstrap scene with persistent GameManager
- [x] Integrate SOAP for game state management
- [x] Define precise MVP scope
- [ ] Coordinate art asset list with Son (deferred - waiting on Son)

### MVP Prototype - Navigation
- [x] Create prototype river scene (greybox Acheron)
- [x] Wire up input handling (PlayerInput component)
- [x] Basic raft movement along spline
- [x] Junction/branch system (up input to take alternate route)
- [x] Merchant branch spline off Acheron
- [x] Central Hub scene (drop-off dock only)
- [x] Hub free movement (tank controls: W/S forward/back, A/D rotate)
- [x] Hub camera (third-person, behind/above, angled down)
- [x] Add Hub action map to HOKInputActions (different controls from Ferry)
- [x] Replace Acheron raft with ---PLAYER--- prefab
- [x] Scene transitions between Acheron and Hub

### MVP Prototype - Fishing
- [ ] Set up fishing SOAP events/variables
- [ ] Fish definitions (10 fish: 6 common, 3 uncommon, 1 rare)
- [ ] Cast mechanic with aim
- [ ] Line visual (cast to water)
- [ ] Fish bite detection and hook timing
- [ ] Tension-based reeling (give slack / reel in)
- [ ] Catch resolution (success/fail)
- [ ] Idle fishing stub (auto-cast, same reel mechanics)
- [ ] Scorch proximity detection (flame intensity for rare fish)

### MVP Prototype - Ferry
- [ ] Set up ferry SOAP events/variables (currency, soul state)
- [ ] Kharon state transitions (Off-Duty/On-Duty)
- [ ] Soul spawn at Acheron pickup dock
- [ ] Soul decay timer system
- [ ] Pickup dock interaction
- [ ] Drop-off dock interaction (Hub)
- [ ] Payment system (full obol, partial, nothing variants)
- [ ] Ferry interruption flow (soul arrives → fishing interrupted)

### MVP Prototype - Progression
- [ ] Obols currency tracking
- [ ] Damned Merchant shop (Acheron branch)
- [ ] 3-4 gear upgrades (rod, line, lures)
- [ ] Gear affects fishing mechanics

### Future (Post-MVP)
- [ ] Other 4 rivers (Styx, Lethe, Phlegethon, Cocytus)
- [ ] Waterfall connectors between rivers
- [ ] Full fish codex (50+ species)
- [ ] Legendary fish multi-phase fights
- [ ] Multiple soul types
- [ ] Alternative payments (gear from souls)
- [ ] Hades route blocking system
- [ ] Special guest encounters
- [ ] Background events system
- [ ] Codex UI

---

## What Works (Completed)

**Design Phase (DONE):**
- High concept and tagline
- Core mechanics design
- Character design (Kharon dual persona, Scorch)
- All five rivers with themes and merchants
- Fish species framework (examples for each river)
- Progression system design
- Art style defined (low-poly 3D)
- Architecture document with SOAP patterns
- Art production brief for external team
- Micro-brief template for individual assets
- River Styx background micro-brief (example)

**Infrastructure (DONE):**
- Folder structure under Assets/HOK (Art, Audio, Data, Prefabs, Scenes, Scripts, UI)
- Input Actions asset (HOKInputActions) with Fishing, Ferry, UI action maps
- Bootstrap scene with persistent GameManager
- SOAP integration: CurrentGameState (IntVariable), OnGameStateChanged (ScriptableEventInt)
- GameState enum and GameManager script in HOK.Core namespace

**Raft Navigation (DONE):**
- Acheron_Greybox scene with greybox environment (water, banks, docks)
- RaftController with spline-based movement (Dreamteck Splines)
- Side-scrolling camera setup (Cinemachine at -Z, looking toward +Z)
- Correct Unity coordinate system: +X=right, -X=left, +Z=top, -Z=bottom
- FollowTarget script for Kharon/Scorch (avoids scale inheritance from raft)
- Input bindings: WASD + Arrow keys for ferry movement, Hub action map for free movement
- SplineInitializer editor tool for river/branch spline setup
- HOK_CodeReference.md architecture document created
- Materials: Bank_DarkGreen, Dock_LightBrown, Raft_Orange for greybox visibility

**Junction System (DONE):**
- SplineJunction component marks branch points on splines
- RaftController detects nearby junctions and listens for up input (W/Up Arrow)
- Visual indicator support (shows when junction is available)
- Bidirectional junction support (can return to previous spline)
- Merchant branch spline off Acheron with greybox dock/stall
- Junction_ToMerchant on main river spline
- Junction_ToRiver at start of merchant branch (returns to main river)
- Seamless junction transitions using world-space projection (no vertical teleport)
- Auto-return junction support at spline boundaries (dead-end branches)
- Junction cooldown prevents immediate auto-return after switching splines
- Spline percent clamped to 0-1 range in RaftController (standard bounds)

**Greybox Layout (IN PROGRESS):**
- Y-shaped river matching reference diagram
- Main river: entrance (right, +X) to dock (left, -X)
- Merchant branch: forks from junction toward upper-left (+Z, -X)
- Banks: BottomBank_Main, MainUpperBank_Right, MainUpperBank_Left, diagonal banks, merchant area banks
- Spline junction points need alignment tweaking to eliminate minor forward offset during transitions

**Central Hub Scene (DONE):**
- FreeMovementController for tank-style free movement (W/S forward/back, A/D rotate)
- Third-person Cinemachine camera following raft from behind/above
- Greybox layout: WaterPlane, DropOffDock (+Z), 5 river entrances around perimeter
- River entrances: Acheron (-Z), Styx (+X), Lethe (-X), Phlegethon (+X,-Z), Cocytus (-X,-Z)
- Raft prefab instance with Kharon on cooler, Scorch hidden underneath
- Kharon height gag: appears ~7ft on cooler, actually ~5'7" when off

**Scene Transitions (DONE):**
- SceneTransitionManager persists across scenes (DontDestroyOnLoad)
- Flow: Hub → RiverEntrance → River (spawn at entrance) → Travel to dock → Pick up soul → Travel back → RiverExit → Hub
- RiverEntrance: In Hub, triggers load of river scene, spawns at percent 0 (river mouth)
- RiverExit: At river mouth (percent ~0, +X side), triggers return to Hub
- Automatic input action map switching (Ferry <-> Hub)
- Raft position set on scene load (spline percent for rivers, world position for hub)

---

## Known Issues

**Pre-Production Phase:**
- Art pipeline with Son not yet established

**Junction System:**
- (All previous issues resolved)

**Design Gaps:**
- Fish species count not finalized (50+ target)
- Legendary fish mechanics still open
- Ferry interruption frequency needs playtesting

---

## What Doesn't Work / Incomplete

- Art asset coordination with Son
- Kitsu task setup

---

## Architecture

### Technology Stack (Planned)

| Layer | Choice | Notes |
|-------|--------|-------|
| Engine | Unity 6 (6000.0.x) | URP 17.3.0 |
| Render Pipeline | URP | Low-poly 3D optimized |
| Architecture | SOAP | ScriptableObject Architecture Pattern |
| Communication | Event Channels | Decoupled systems |
| UI Framework | uGUI | With Lean GUI + Text Animator |
| Save System | Easy Save 3 | Binary serialization |

### Namespaces

| Namespace | Purpose |
|-----------|---------|
| `HOK.Core` | Game state machine, managers |
| `HOK.Fishing` | Cast, reel, catch systems |
| `HOK.Ferry` | Soul transport, payment |
| `HOK.Companion` | Scorch behavior |
| `HOK.Progression` | Unlocks, currency, codex |
| `HOK.UI` | Menus, HUD, codex display |
| `HOK.Audio` | Music, SFX managers |
| `HOK.Data` | ScriptableObject definitions |

### Key Folders (Planned)

```
Assets/
  Art/
    Models/          # Characters, Fish, Environment, Props
    Animations/
    Materials/
    VFX/             # Fire effects, ghostly glow
  Audio/
    Music/
    SFX/
    Ambient/
  Prefabs/
  Scenes/
    MainMenu/
    Rivers/
    Shops/
  Scripts/
    Core/            # Game managers, state machine
    Fishing/         # Cast, reel, catch systems
    Ferry/           # Soul transport, payment
    Companion/       # Scorch behavior
    Progression/     # Unlocks, currency, codex
    UI/
    Audio/
  ScriptableObjects/
    Fish/
    Gear/
    Merchants/
    Souls/
    FishingSpots/
    Events/
```

### Key Scripts

| Script | Lines | Purpose |
|--------|-------|---------|
| GameManager.cs | 68 | Persistent singleton, SOAP state management |
| GameState.cs | 14 | Game state enum (OffDuty, Fishing, Ferrying, InMenu) |
| PlayerMovementController.cs | ~540 | Unified movement: Free mode (Hub) + Spline mode (River) with junctions |
| RaftController.cs | DEPRECATED | Use PlayerMovementController instead |
| FreeMovementController.cs | DEPRECATED | Use PlayerMovementController instead |
| SceneTransitionManager.cs | ~265 | Prefab singleton for scene loading, spawn positioning, input map switching |
| RiverExit.cs | ~95 | Trigger at river entrance/mouth, transitions to Hub |
| RiverEntrance.cs | ~130 | Trigger in Hub, transitions to river scenes |
| FollowTarget.cs | 47 | Follow target with offset (ExecuteAlways) |
| SplineJunction.cs | ~140 | Marks branch points on splines for navigation |
| SplineInitializer.cs | ~110 | Editor tool for river/branch spline setup |

### Dependencies / Packages (Installed)

**Asset Store Packages:**
| Package | Version | Purpose |
|---------|---------|---------|
| Dialogue System for Unity | 2.2.65 | Soul dialogue, merchant conversations |
| DOTween Pro | 1.0.386 | Animation, UI transitions |
| Dreamteck Splines | 3.0.6 | River paths, ferry routes |
| Easy Save 3 | 3.5.x | Save/load system |
| In-game Debug Console | 1.8.4 | Runtime debugging |
| Lean GUI | 2.1.0 | UI polish, tooltips, transitions |
| Odin Inspector | 4.0.1.2 | Enhanced inspector, serialization |
| Odin Validator | 4.0.1.2 | Data validation |
| SOAP | 3.7.1 | ScriptableObject Architecture Pattern |
| Text Animator | 3.1.1 | Animated dialogue text |

**Local Packages:**
| Package | Version | Purpose |
|---------|---------|---------|
| MCP for Unity | 9.0.7 | Claude Code integration |
| UniTask | 2.5.10 | Async/await |
| Improved Timers | 1.0.4 | Timer utilities |
| Unity Utility Library | 1.0.21 | General utilities |

**Unity Registry:**
| Package | Version | Purpose |
|---------|---------|---------|
| 2D Sprite | 1.0.0 | UI sprites, icons |
| Cinemachine | 3.1.5 | Camera control |
| Input System | 1.16.0 | Modern input handling |
| ProBuilder | 6.0.8 | 3D modeling, level design |
| Splines | 2.8.2 | Native spline support |
| TextMeshPro | (included) | UI text rendering |
| Timeline | 1.8.10 | Cutscenes, sequences |
| URP | 17.3.0 | Universal Render Pipeline |

---

## Game Rules

### Core Concept
Cozy fishing sim where you play as Kharon. Fish in peace until souls interrupt. Ferry them (grudgingly), then return to fishing. Upgrade gear, discover rare fish, unlock new rivers.

### Fishing Mechanics

**Active Mode:**
- Manual casting with aim
- Resistance-based reeling (push/pull tension)
- Give slack when fish fights, reel when fish rests

**Idle Mode:**
- Auto-cast and reel
- Background progression
- Lower catch rate for rare fish

### Ferry Mechanics

**Soul Decay System:**
- Each soul has a decay timer
- If timer expires before delivery, soul fades, no payment
- Creates tension between fishing (relaxation) and ferrying (duty)

**River Navigation:**
- Hub-and-spoke layout: central lake with five rivers radiating outward
- Each river has two-way access to/from the central hub
- One-way waterfall connectors between adjacent rivers (clockwise flow only)
- Waterfall order: Acheron -> Lethe -> Styx -> Cocytus -> Phlegethon
- Connectors are shortcuts, not required paths - player can always return to hub via river exit

**Map Layout:**
- **Central Hub:** Dark lake with single drop-off dock (Gates of Hades approach)
- **Rivers:** Five spokes, each with pickup docks along banks and merchant at far end
- **Pickup Docks:** Souls spawn here, wait for Kharon
- **Drop-off Dock:** Central hub only - all souls delivered here
- **Merchants:** Accessible during ferry runs, but soul timer keeps running (risk/reward)

**Route Blocking (Hades Mood System):**
- Hades can block river entrances and/or waterfall connectors
- Blocking forces longer alternate routes, pressures soul timer
- Player can never be fully trapped - hub access always available via some path
- Multiple simultaneous blocks create endgame routing challenges

**Unlock Progression (Flexible):**
- River entrances and waterfall connectors unlock independently
- Player may unlock a connector before its river entrance (access via adjacent river)
- Creates varied progression paths and replay value
- Example: Unlock Acheron->Lethe connector before Lethe entrance = must enter Lethe via Acheron

**Reference:** See `Reference Art/HOK_Art_SimpleMap.png` for visual layout

### Camera System

**2.5D Perspective with Dynamic Framing (Cast n' Chill inspired):**

**Above Water (Ferry/Navigation Mode):**
- Slight tilted angle from above - enough to see raft surface, Kharon, Scorch
- Waterline near bottom of screen
- Full view of background environment, docks, souls, river scenery
- Gives spatial depth to the scene

**Underwater (Fishing Mode):**
- Camera shifts up AND flattens angle toward 2D side-view
- Waterline at top of screen (or higher based on line depth)
- Clear view of fish, fishing line, underwater environment
- More readable for fishing gameplay

**Transition:**
- Smooth camera movement when casting/reeling
- Vertical position + angle both animate between modes
- Water plane acts as visual anchor point

**Foreground Occlusion:**
- Underwater rocks/obstacles in foreground fade to transparent
- Triggers when fishing line or hooked fish are behind them
- Player always has clear view of the catch
- Shader-based with depth or distance check

### Scorch Proximity Detection

| Distance | Behavior |
|----------|----------|
| Far | Calm, resting |
| Getting closer | Ears perk, sniffing |
| Close | Soft growl, pacing |
| Very close | Whoopie cushion bark + flame puff |
| On the spot | Full flame-on, excited spinning |

---

## Game State Machines

### Game State

```
GameState
  MainMenu
  Playing
    Fishing (primary state)
      Idle Mode
      Active Mode
    FerryInterrupt (soul arrives)
    Ferrying (transporting soul)
    Shopping (at merchant)
  Paused
  Codex (viewing fish collection)
```

### Kharon State

```
KharonState
  OffDuty (Fishing)
    Sitting on cooler, Scorch visible
    Can cast, reel, idle
    Mutters about fish spots
  Transitioning (Soul Alert)
    Puts down rod
    Stands, places cooler
    Steps onto cooler
    Whistles for Scorch
    Robes close
  OnDuty (Ferrying)
    Tall, imposing
    Scorch hidden in robes
    Silent (no dialogue responses)
    Can spot rare fish (muttering)
  Transitioning (Duty Complete)
    Coast clear check
    Robes open
    Steps off cooler
    Scorch emerges
```

---

## Data Flow Diagrams

### Core Game Loop

```
FISHING PHASE
  Select river/location
    -> Cast line (Active or Idle)
      -> Watch for fish, use Scorch
        -> Reel with tension management
          -> Catch fish, add to Codex
            -> FERRY INTERRUPTION
```

### Ferry Flow

```
Soul arrives
  -> Kharon puts rod away
    -> Steps onto cooler, whistles for Scorch
      -> Robes close (now tall)
        -> Poles to soul
          -> Accept payment OR reject non-payer
            -> May spot rare fish (muttering)
              -> Deliver soul
                -> Return to fishing spot
```

### Ferry Route Flow (Map Navigation)

```
Soul spawns at Pickup Dock (river spoke)
  -> Soul timer starts
    -> Kharon travels FROM hub TO that river
      -> Picks up soul at dock
        -> OPTIONAL: Detour to merchant (timer still running!)
          -> Navigate back to hub (direct or via waterfall shortcuts)
            -> Drop off at central Drop-off Dock
              -> Soul proceeds to Gates of Hades
                -> Kharon returns to fishing spot
```

**Routing Example (Blocked Paths):**
```
Blocked: Acheron entrance, Lethe entrance
Soul waiting at Acheron dock

Hub -> UP Phlegethon (open)
  -> Waterfall -> Acheron (clockwise)
    -> Pickup soul
      -> Waterfall -> Lethe
        -> Waterfall -> Styx
          -> DOWN Styx to Hub
            -> Drop off soul
```

---

## Key Patterns

### 1. SOAP Architecture

```csharp
// All game data as ScriptableObjects
// FishDefinition, GearDefinition, MerchantDefinition, etc.
// Event channels for decoupled communication
```

### 2. State Machine Pattern

```csharp
// Both GameState and KharonState use explicit state machines
// Transitions handled through events
// Clear entry/exit behaviors per state
```

---

## Key Design Elements (Non-Negotiable)

### Kharon's Dual Persona
- **On-duty:** Tall, imposing (standing on cooler), silent, professional
- **Off-duty:** Smaller skeleton revealed, sitting on cooler, mutters to himself
- **The secret:** No one knows he's short - the cooler is his height prop

### Scorch the Hell Hound
- Small hell hound companion
- **Bark:** Whoopie cushion sound + small flame
- Proximity detection via flame intensity
- When hidden in robes: fire exits back, souls think Kharon farted

### Five Rivers, Five Merchants

| River | Meaning | Difficulty | Merchant |
|-------|---------|------------|----------|
| Acheron | Pain/Woe | Beginner | Damned Merchant |
| Styx | Hate | Early | Hermes |
| Lethe | Forgetfulness | Mid | Hypnos/Dreaming Shade |
| Phlegethon | Fire | Advanced | Hephaestus's Shade |
| Cocytus | Lamentation | Endgame | Nyx |

### No Tech in Underworld
- No fish finders or depth sensors
- Use Scorch + environmental landmarks
- Player memory for rare fish spots

---

## Data Models (Planned)

### Fish Definition

```csharp
[CreateAssetMenu(fileName = "Fish", menuName = "HookedOnKharon/FishDefinition")]
public class FishDefinition : ScriptableObject
{
    public string fishName;
    public string description;
    public Sprite icon;
    public GameObject prefab;

    [Header("Location")]
    public RiverType river;
    public float minDepth;
    public float maxDepth;

    [Header("Rarity")]
    public FishRarity rarity;
    public float spawnWeight;

    [Header("Catching")]
    public float baseFightStrength;
    public float fightDuration;
    public LureType[] preferredLures;

    [Header("Rewards")]
    public int obolValue;
    public int codexXP;

    [Header("Lore")]
    [TextArea] public string codexEntry;
    [TextArea] public string catchQuote; // What Kharon mutters
}
```

### Enums

```csharp
public enum RiverType { Acheron, Styx, Lethe, Phlegethon, Cocytus }
public enum FishRarity { Common, Uncommon, Rare, Legendary }
public enum LureType { MemoryBait, RegretFragment, FinalBreath, BrokenOath, HerosGlory }
public enum GearType { Rod, Line, Lure, RaftUpgrade, ScorchCollar }
public enum GearTier { Basic, Mid, Advanced, Endgame, Legendary }
public enum SoulType { Common, Wealthy, Poor, SpecialGuest }
public enum PaymentType { FullObol, PartialObol, BarterItem, Nothing }
```

---

## MVP Scope

**Core Loop:** Fish on Acheron → soul interrupts → ferry to Hub → return → repeat. Spend obols at merchant to upgrade gear.

### In MVP

**Rivers/Areas:**
| Area | Features | Notes |
|------|----------|-------|
| Acheron (main) | Fishing spots, 1 pickup dock, branch junction | Full functionality |
| Acheron (merchant branch) | Damned Merchant shop | Tests branch navigation |
| Central Hub | Drop-off dock only | No fishing, just delivery |

**Junction/Branch System:**
- Junction points on splines where routes diverge
- Up input = take alternate route (merchant branch)
- Continue forward = stay on main path
- Visual indicator when approaching junction
- Same system will be used for waterfall connectors post-MVP

**Hub Navigation (different from rivers):**
- Free movement (not spline-locked) - full WASD/stick control
- Camera: third-person, behind/above raft, angled down
- River entrances around perimeter act as exit points
- Drop-off dock interaction triggers soul delivery

**Fish (10 total on Acheron):**
- 6 Common
- 3 Uncommon
- 1 Rare (Scorch detects via flame intensity)

**Fishing Mechanics:**
- Active mode: cast with aim, tension-based reeling, catch resolution
- Idle mode (stub): auto-cast, same tension mechanics, no aiming

**Souls:**
- 1 soul type (single visual/behavior)
- Payment variants: full obol, partial obol, nothing
- Single decay timer speed

**Scorch:**
- Proximity detection (flame intensity increases near rare fish)
- No upgrades

**Progression:**
- Obols currency
- Damned Merchant with 3-4 gear upgrades (rod, line, lures)

**Session Feel:**
- Fishing: quick catches, longer for fights (exact timing TBD via playtesting)
- Ferrying: longer due to travel (exact timing TBD via playtesting)

### NOT in MVP

- Other 4 rivers (Styx, Lethe, Phlegethon, Cocytus)
- Waterfall connectors
- Multiple soul types
- Alternative payments (gear from souls)
- Hades route blocking
- Legendary fish / multi-phase fights
- Special guests
- Background events
- Codex UI (track catches internally only)
- Audio beyond placeholder

---

## Lessons Learned (Don't Repeat)

### From Cast n' Chill Analysis
1. **Dual modes proven** - Active/Idle fishing works, keep as-is
2. **Resistance-based reeling** - Skill-based, engaging
3. **Companion adds warmth** - Hell hound twist on friendly dog

### From Other TecVooDoo Projects
4. **SOAP architecture** - Use for fish, gear, merchant definitions
5. **Profile early** - Low-poly should be fine, but verify
6. **Keep scope realistic** - MVP first, expand later
7. **Document pivots** - Track all decisions

### From Design Sessions
8. **Ferry mechanics need equal depth to fishing** - Both halves deserve full design
9. **Interconnected systems > separate systems** - Soul decay ties fishing gear to ferry routes
10. **Environmental storytelling > exposition** - Background events, no pop-ups

---

## Bug Patterns to Avoid

| Bug Pattern | Cause | Prevention |
|-------------|-------|------------|
| (none yet - planning phase) | - | - |

---

## Open Questions

| Question | Status | Notes |
|----------|--------|-------|
| Auto/Screensaver Mode | IDEA | Johnny Castaway-style passive mode |
| Total fish count | OPEN | 50+ target |
| Legendary fish mechanics | OPEN | Multi-phase? Special conditions? |
| Scorch upgrade specifics | OPEN | Collar effects? |
| Background event frequency | OPEN | Balance needed |
| Ferry interruption rate | OPEN | Critical for feel |
| Soul decay timer values | OPEN | Needs playtesting for feel |
| Hub location specifics | DECIDED | Central lake = drop-off dock, approach to Gates of Hades |
| Hades mood trigger frequency | OPEN | Daily? Per session? Random events? |
| Lethe confusion mechanic details | OPEN | UI treatment for forgetting destination |
| Alternative payment drop rates | OPEN | How often do souls offer gear vs. nothing? |
| Merchant reaction to faded souls | OPEN | Different merchants react differently when soul fades mid-transaction? |
| Phlegethon waterfall destination | OPEN | Does it loop back to Acheron or dead-end? |
| Water level / tide system | IDEA | Unlocking rivers lowers water (permanent progression) and/or tides create temporary access windows (dynamic gameplay). Could explain why rivers are locked (flooded caves) and how Hades blocks routes (flood surges). Needs more thought. |

---

## Red Flags / Watch Items

| Flag | Severity | Notes |
|------|----------|-------|
| Ferry interruption frequency | HIGH | Must balance - too often = annoying |
| Soul decay timer tuning | HIGH | Too fast = stressful, too slow = no tension |
| Scope creep on fish species | MEDIUM | Start with core set, expand later |
| Difficulty curve across rivers | MEDIUM | Needs playtesting |
| Route complexity | MEDIUM | Hub system must be intuitive, not confusing |
| Alternative payment balance | MEDIUM | Unique gear must feel special, not farmable |
| Art pipeline with Son | LOW | Communication is key |

---

## Decision Log

| Date | Decision | Rationale |
|------|----------|-----------|
| Dec 25, 2025 | Low-poly 3D art style | Son is 3D artist |
| Dec 25, 2025 | Ferry interrupts fishing | Core to Kharon's character |
| Dec 25, 2025 | Scorch as companion | Cute hell hound fits theme |
| Dec 25, 2025 | Whoopie cushion bark | Dark comedy gold |
| Dec 25, 2025 | Five merchants for five rivers | Progression + personality |
| Dec 25, 2025 | No tech/fish finders | Immersion, ancient setting |
| Dec 25, 2025 | Kharon's height secret | Comedy through visual gag |
| Jan 6, 2026 | Soul decay timer system | Creates ferry stakes, balances fishing tension |
| Jan 6, 2026 | Hub-based river navigation | 2.5D friendly, familiar portal/door pattern |
| Jan 6, 2026 | Fishing unlocks ferry routes | Links both halves mechanically |
| Jan 6, 2026 | Alternative payments become gear | Narrative reward, memorable items |
| Jan 6, 2026 | Hades mood as route modifier | Dynamic routing, mythology tie-in |
| Jan 6, 2026 | Detailed background events | Rewards mythology knowledge, no tutorial needed |
| Jan 20, 2026 | Collectibles system with merchant wall | Visual completion tracking without UI menus, high-value barter tier |
| Jan 23, 2026 | Document restructuring | Align with DLYH patterns, update template |
| Jan 23, 2026 | Unity project created | Unity 6, URP 17.3.0, all core packages installed |
| Jan 23, 2026 | GitHub repository | https://github.com/TecVooDoo/HookedOnKharon |
| Jan 23, 2026 | uGUI for UI framework | Mature ecosystem, better asset support than UI Toolkit |
| Jan 23, 2026 | SOAP for state management | Use installed assets first, decoupled event-driven architecture |
| Jan 23, 2026 | HOK namespace prefix | Changed from HOC to HOK to match project folder |
| Jan 24, 2026 | Hub-and-spoke map layout | Five rivers radiate from central hub, two-way river access, one-way waterfall shortcuts |
| Jan 24, 2026 | Central hub = drop-off point | All souls delivered to single drop-off dock at central lake (Gates approach) |
| Jan 24, 2026 | Clockwise waterfall flow | Connectors flow Acheron->Lethe->Styx->Cocytus->Phlegethon |
| Jan 24, 2026 | Merchants accessible while ferrying | Risk/reward - soul timer keeps running during merchant visits |
| Jan 24, 2026 | Flexible unlock progression | Connectors and entrances unlock independently, not linear |
| Jan 24, 2026 | Route blocking = time pressure | Hades blocks routes to force longer paths, never fully traps player |
| Jan 24, 2026 | MVP scope finalized | Acheron + Hub only, 10 fish, junction system, 1 soul type, merchant branch |
| Jan 24, 2026 | Junction input = Up | Pressing up at junction takes alternate route (merchant, connectors) |
| Jan 24, 2026 | Idle fishing = stub | Auto-cast only, same tension mechanics as active mode |
| Jan 24, 2026 | Hub uses free movement | Not spline-locked, full WASD control, third-person camera behind/above |

---

## Reference Documents

| Document | Path | Purpose |
|----------|------|---------|
| **Code Reference** | `HOK_CodeReference.md` | **READ FIRST** - Namespaces, classes, APIs, SOAP assets |
| Art Asset List | `HOC_Art_AssetList.md` | My tracking of art assets and priorities |
| Art Production Brief | `HOC_Art_Animation_Production_Brief.md` | Art direction for Son's team |
| River Styx Brief | `HOC_Art_MicroBrief_river_styx_background.md` | Example micro-brief |
| Reference Art | `Reference Art/` folder | Visual references |
| Simple Map Diagram | `Reference Art/HOK_Art_SimpleMap.png` | Hub-and-spoke layout with docks, merchants, connectors |
| DLYH Status | `C:\Unity\DontLoseYourHead\Documents\DLYH_Status.md` | Working async multiplayer reference |

---

## Coding Standards (Unity/C#)

- Prefer async/await (UniTask) over coroutines unless trivial
- Avoid allocations in Update
- No per-frame LINQ
- Clear separation between logic and UI
- ASCII-only documentation and identifiers
- No `var` keyword - explicit types always

### Refactoring Guidelines

**Goal Range:** Files should be 800-1200 lines for optimal Claude compatibility. Files up to 1300 lines are acceptable if that's the minimum achievable without adding unnecessary complexity.

**When to Refactor:**
- Extract when a file exceeds 1200 lines AND has clear, separable responsibilities
- Extract when duplicate code appears across multiple locations
- Extract when a class has multiple unrelated responsibilities

**When NOT to Refactor:**
- Don't refactor to hit an arbitrary line count
- Don't extract if it adds indirection without reducing complexity
- Don't create helper classes for one-off operations
- Don't extract if the code is cohesive and naturally belongs together

---

## AI Rules (Embedded)

### Critical Protocols
1. **Verify names exist** - search before referencing files/methods/classes
2. **Read HOK_CodeReference.md** - check existing APIs before writing new code
3. **Step-by-step verification** - one step at a time, wait for confirmation
4. **Read before editing** - always read files before modifying
5. **ASCII only** - no smart quotes, em-dashes, or special characters
6. **Validate after MCP edits** - run validate_script to catch syntax errors
7. **Use E: drive path** - never worktree paths
8. **Be direct** - give honest assessments, don't sugar-coat
9. **Acknowledge gaps** - say explicitly when something is missing or unclear
10. **Status update = CodeReference update** - when updating HOK_Status.md after code changes, ALWAYS also update HOK_CodeReference.md in the same commit

---

## Cross-Project Reference

**All TecVooDoo projects:** `E:\TecVooDoo\Projects\Documents\TecVooDoo_Projects.csv`

---

## Session Close Checklist

After each work session, update this document:

- [ ] Move completed TODOs to "What Works" section
- [ ] Add any new issues to "Known Issues"
- [ ] Update "Last Session" with date and summary
- [ ] Add new lessons to "Lessons Learned" if applicable
- [ ] Increment version number in header
- [ ] Archive old version history entries when needed (keep ~5-6 recent)
- [ ] **Update HOK_CodeReference.md** - MANDATORY if any scripts were added/changed (do this BEFORE committing status updates)

---

## Version History (Recent)

| Version | Date | Summary |
|---------|------|---------|
| 17 | Jan 30, 2026 | Fixed spline speed inconsistency (was using hardcoded /10.0 instead of actual spline length). Made junction cooldown configurable in Inspector. Updated default spline speed to 4 units/sec. |
| 16 | Jan 30, 2026 | Fixed junction system: removed takeJunctionPressed tracking (was causing one-time-only bug), simplified to cooldown-based prevention. Fixed Junction_ToRiver position from 0.95 to 0.05. Junctions now work repeatedly. |
| 15 | Jan 29, 2026 | Unified PlayerMovementController (Free + Spline modes). Separate Hub/River speeds. Spline rotation following. Auto-detect MainRiver tag. Scene transitions working both directions. Junction system needs fixing (next session). |
| 14 | Jan 29, 2026 | Scene transitions implemented: SceneTransitionManager, RiverExit, RiverEntrance completion. Auto input map switching. MVP Navigation section complete. |
| 13 | Jan 27, 2026 | Updated troubleshooting docs to DLYH format. Created HOK_Troubleshooting_Archive.md. Fixed HOK_CodeReference.md formatting. Validated code against docs, removed incorrect min/max percent references. |
| 12 | Jan 24, 2026 | Added Hub action map. Replaced Acheron raft with ---PLAYER--- prefab. Fixed junction cooldown for auto-return. Known issue: junction requires movement to activate. |
| 11 | Jan 24, 2026 | Central Hub scene complete: FreeMovementController (tank controls), third-person camera, greybox layout with dock/river entrances. Kharon positioned on cooler with Scorch hidden. Ready for playtesting. |
| 10 | Jan 24, 2026 | Fixed junction transitions: world-space projection for seamless spline switching, removed vertical teleport, fixed direction reversal bug. Minor forward offset remains (editor tweaking). |
| 9 | Jan 24, 2026 | Fixed coordinate system (camera at -Z looking +Z). Rebuilt greybox layout to match reference diagram. Added colored materials. Splines need tuning. |
| 8 | Jan 24, 2026 | Finalized MVP scope: Acheron + Hub, 10 fish, junction/branch system, 1 soul type, idle stub. Reorganized TODOs by system. |
| 7 | Jan 24, 2026 | Raft navigation: RaftController, FollowTarget, side-scrolling camera, input bindings, HOK_CodeReference.md |
| 6 | Jan 24, 2026 | Map design: hub-and-spoke layout, waterfall connectors, pickup/drop-off docks, flexible unlock progression, merchant access during ferry |
| 5 | Jan 23, 2026 | Bootstrap scene, GameManager with SOAP, input actions, folder structure |
| 4 | Jan 23, 2026 | Unity project created, packages installed, GitHub repo established |
| 3 | Jan 23, 2026 | Document restructuring - aligned with DLYH patterns and updated template |

---

**End of Project Status**
