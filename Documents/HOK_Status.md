# Hooked on Kharon: Reeling in the Underworld - Project Status

**Project:** Hooked on Kharon: Reeling in the Underworld
**Developer:** TecVooDoo LLC / Rune (Stephen Brandon)
**3D Artist:** Son
**Platform:** Unity (Version TBD)
**Source:** `E:\Unity\Hooked On Kharon`
**Repository:** https://github.com/TecVooDoo/HookedOnKharon
**Document Version:** 5
**Last Updated:** January 23, 2026

**Archive:** `HookedOnKharon_Status_Archive.md` - Historical designs, old version history, completed phase details (create when needed)

---

## Quick Context

**What is this game?** A cozy low-poly 3D fishing sim set in the Greek Underworld. Play as Kharon, the eternal ferryman who just wants to fish in peace - but souls keep interrupting. Ferry the dead (grudgingly), collect obols, and upgrade gear across five legendary rivers.

**Tagline:** "Even death needs a break."

**Current Phase:** Pre-Production

**Last Session (Jan 23, 2026):** Bootstrap scene with GameManager using SOAP, input actions configured, folder structure created.

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
- [ ] Define precise MVP scope
- [ ] Coordinate art asset list with Son

### Soon (Prototype)
- [ ] Create prototype river scene (greybox Acheron)
- [ ] Wire up input handling (PlayerInput component)
- [ ] Set up additional SOAP events/variables (fishing, ferry, currency)
- [ ] Core fishing loop prototype
- [ ] Kharon state transitions (On-Duty/Off-Duty)
- [ ] Scorch proximity detection system
- [ ] Basic ferry interruption flow

### Future (Post-MVP)
- [ ] All 5 rivers and merchants
- [ ] Full fish codex (50+ species)
- [ ] Legendary fish multi-phase fights
- [ ] Special guest encounters
- [ ] Background events system

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

---

## Known Issues

**Pre-Production Phase:**
- Art pipeline with Son not yet established

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
- Hub-and-spoke with horizontal rivers and vertical connectors
- Some connectors one-way (waterfall down)
- Some connectors locked until gear requirements met

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

**In MVP:**
- 2-3 rivers (Acheron, Styx, maybe Lethe)
- Core fishing loop (Active + Idle)
- Ferry interruption system
- Soul decay timer (basic implementation)
- Hub navigation (simplified)
- Scorch companion with proximity detection
- 1-2 merchants
- 15-20 fish species
- Basic progression
- 2-3 alternative payment items

**NOT in MVP:**
- All 5 rivers
- Special guests
- Full background events
- Legendary fish
- Full upgrade tree
- Hades mood system
- Complex route blockages
- Full alternative payment catalog

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
| Hub location specifics | OPEN | Near Judges? Palace? Both? |
| Hades mood trigger frequency | OPEN | Daily? Per session? Random events? |
| Lethe confusion mechanic details | OPEN | UI treatment for forgetting destination |
| Alternative payment drop rates | OPEN | How often do souls offer gear vs. nothing? |

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

---

## Reference Documents

| Document | Path | Purpose |
|----------|------|---------|
| Art Asset List | `HOC_Art_AssetList.md` | My tracking of art assets and priorities |
| Art Production Brief | `HOC_Art_Animation_Production_Brief.md` | Art direction for Son's team |
| River Styx Brief | `HOC_Art_MicroBrief_river_styx_background.md` | Example micro-brief |
| Reference Art | `Reference Art/` folder | Visual references |
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
2. **Step-by-step verification** - one step at a time, wait for confirmation
3. **Read before editing** - always read files before modifying
4. **ASCII only** - no smart quotes, em-dashes, or special characters
5. **Validate after MCP edits** - run validate_script to catch syntax errors
6. **Use E: drive path** - never worktree paths
7. **Be direct** - give honest assessments, don't sugar-coat
8. **Acknowledge gaps** - say explicitly when something is missing or unclear

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

---

## Version History (Recent)

| Version | Date | Summary |
|---------|------|---------|
| 5 | Jan 23, 2026 | Bootstrap scene, GameManager with SOAP, input actions, folder structure |
| 4 | Jan 23, 2026 | Unity project created, packages installed, GitHub repo established |
| 3 | Jan 23, 2026 | Document restructuring - aligned with DLYH patterns and updated template |
| 2 | Jan 6, 2026 | Integrated ferry mechanics (soul decay, river navigation, routing, alternative payments), expanded creatures, background events, Hollywood gag details, new data models |
| 1 | Jan 5, 2026 | Initial consolidated document (replaces HC v2 4-doc set) |

---

**End of Project Status**
