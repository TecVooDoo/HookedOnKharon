# Hooked on Kharon: Reeling in the Underworld - Architecture Document

**Project:** Hooked on Kharon: Reeling in the Underworld
**Developer:** TecVooDoo LLC
**Designer:** Rune (Stephen Brandon)
**Project Path:** E:\Unity\Hooked On Kharon
**Document Version:** 1
**Last Updated:** December 25, 2025

---

## Version History

| Version | Date | Summary |
|---------|------|---------|
| v1 | Dec 25, 2025 | Initial document - concept phase |

---

## Technology Stack (Planned)

### Engine
- **Unity** (Version TBD) - Consistent with other TecVooDoo projects
- **Render Pipeline:** URP recommended for low-poly 3D style

### Architecture Pattern
- **SOAP (ScriptableObject Architecture Pattern)** - Data-driven design
- Event channels for decoupled communication
- ScriptableObjects for fish, merchants, gear definitions

---

## Core Systems

### Game State Machine

```
GameState
├── MainMenu
├── Playing
│   ├── Fishing (primary state)
│   │   ├── Idle Mode
│   │   └── Active Mode
│   ├── FerryInterrupt (soul arrives)
│   ├── Ferrying (transporting soul)
│   └── Shopping (at merchant)
├── Paused
└── Codex (viewing fish collection)
```

### Kharon State Machine

```
KharonState
├── OffDuty (Fishing)
│   ├── Sitting on cooler
│   ├── Scorch visible under cooler
│   ├── Can cast, reel, idle
│   └── Mutters about fish spots
├── Transitioning (Soul Alert)
│   ├── Puts down rod
│   ├── Stands, places cooler
│   ├── Steps onto cooler
│   ├── Whistles for Scorch
│   └── Robes close
├── OnDuty (Ferrying)
│   ├── Tall, imposing
│   ├── Scorch hidden in robes
│   ├── Silent (no dialogue responses)
│   ├── Can spot rare fish (muttering)
│   └── Accepts/rejects payment
└── Transitioning (Duty Complete)
    ├── Coast clear check
    ├── Robes open
    ├── Steps off cooler
    ├── Scorch emerges
    └── Returns to OffDuty
```

---

## Data Models

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

public enum RiverType { Acheron, Styx, Lethe, Phlegethon, Cocytus }
public enum FishRarity { Common, Uncommon, Rare, Legendary }
public enum LureType { MemoryBait, RegretFragment, FinalBreath, BrokenOath, HerosGlory }
```

### Merchant Definition

```csharp
[CreateAssetMenu(fileName = "Merchant", menuName = "HookedOnKharon/MerchantDefinition")]
public class MerchantDefinition : ScriptableObject
{
    public string merchantName;
    public string title;
    public RiverType river;
    public Sprite portrait;
    public GameObject prefab;

    [Header("Personality")]
    [TextArea] public string greeting;
    [TextArea] public string farewell;
    [TextArea] public string cantAfford;

    [Header("Inventory")]
    public ShopItem[] inventory;
}

[System.Serializable]
public class ShopItem
{
    public GearDefinition gear;
    public int obolCost;
    public BarterItem[] alternateCost;
    public bool requiresPreviousPurchase;
}
```

### Gear Definition

```csharp
[CreateAssetMenu(fileName = "Gear", menuName = "HookedOnKharon/GearDefinition")]
public class GearDefinition : ScriptableObject
{
    public string gearName;
    public string description;
    public GearType gearType;
    public GearTier tier;
    public Sprite icon;

    [Header("Stats")]
    public float lineStrength;
    public float castDistance;
    public float reelSpeed;
    public float rareFishBonus;

    [Header("Special")]
    public bool fireResistant; // For Phlegethon
    public bool coldResistant; // For Cocytus
    public RiverType[] unlocksRivers;
}

public enum GearType { Rod, Line, Lure, RaftUpgrade, ScorchCollar }
public enum GearTier { Basic, Mid, Advanced, Endgame, Legendary }
```

### Soul Definition

```csharp
[CreateAssetMenu(fileName = "Soul", menuName = "HookedOnKharon/SoulDefinition")]
public class SoulDefinition : ScriptableObject
{
    public string soulName;
    public SoulType soulType;
    public Sprite portrait;
    public GameObject prefab;

    [Header("Payment")]
    public bool canPayObol;
    public int obolAmount;
    public BarterItem barterItem; // If can't pay obol

    [Header("Dialogue")]
    [TextArea] public string[] ferryDialogue; // Kharon ignores all of this
    public bool isSpecialGuest; // Orpheus, Heracles, etc.

    [Header("Spawn")]
    public RiverType[] spawnRivers;
    public float spawnWeight;
}

public enum SoulType { Common, Wealthy, Poor, SpecialGuest }
```

---

## Core Mechanics Systems

### Fishing System

```
FishingSystem
├── CastController
│   ├── AimDirection
│   ├── CastPower
│   └── LureSelection
├── LineController
│   ├── CurrentTension (0-100)
│   ├── FishFighting (bool)
│   └── LineBroken (event)
├── ReelController
│   ├── ReelInput
│   ├── SlackGiven
│   └── FishDistance
├── FishSpawner
│   ├── SpawnByRiver
│   ├── SpawnByDepth
│   ├── RarityRolls
│   └── SoulDensityModifier
└── CatchHandler
    ├── CalculateRewards
    ├── AddToCodex
    └── TriggerCatchAnimation
```

### Idle Fishing Mode

```
IdleFishingSystem
├── AutoCast (timer-based)
├── AutoReel (simplified tension)
├── ReducedRareChance
├── BackgroundProgression
└── NotificationOnLegendary
```

### Ferry System

```
FerrySystem
├── SoulSpawner
│   ├── SpawnTimer
│   ├── InterruptFishing (event)
│   └── QueueSouls (if multiple)
├── PaymentHandler
│   ├── AcceptObol
│   ├── AcceptBarter
│   └── RejectNonPayer
├── TransportController
│   ├── PoleAnimation
│   ├── TravelPath
│   └── DeliverSoul
└── RareFishSpotter
    ├── SpotChance (during ferry)
    ├── TriggerMuttering
    └── StoreLocationMarker
```

### Rare Fish Spotting System

```
RareFishSpotSystem
├── SpotTrigger
│   ├── RandomChanceDuringFerry
│   └── FishRarityAffectsChance
├── MutteringSystem
│   ├── GenerateLandmarkDescription
│   ├── PlayMutter1 (delay)
│   ├── PlayMutter2 (delay)
│   ├── PlayMutter3 (delay)
│   └── SoulInterrupts (Kharon goes silent)
├── LocationMarker
│   ├── StoreEnvironmentCues
│   ├── NoMinimapPing
│   └── PlayerMustRemember
└── BacktrackSystem
    ├── ReturnToSpot
    └── ScorchProximityDetection
```

### Scorch Companion System

```
ScorchSystem
├── VisibilityState
│   ├── Visible (fishing)
│   └── Hidden (in robes)
├── ProximityDetection
│   ├── DetectionRadius
│   ├── CurrentDistance (to target)
│   └── BehaviorState
├── Behaviors
│   ├── Resting (far)
│   ├── Alert (closer)
│   ├── Growling (close)
│   ├── Barking (very close) // Whoopie cushion + flame
│   └── FlameOn (on spot)
└── HiddenBark
    ├── TriggerFromRobes
    ├── FireFromBack (visual)
    └── SoulReaction (flabbergasted)
```

---

## Map System

### Erebos World Map

```
WorldMap
├── Rivers[]
│   ├── Acheron
│   │   ├── FishingSpots[]
│   │   ├── Merchant (Damned Merchant)
│   │   └── Unlocked (default)
│   ├── Styx
│   │   ├── FishingSpots[]
│   │   ├── Merchant (Hermes)
│   │   └── UnlockRequirement
│   ├── Lethe
│   │   ├── FishingSpots[]
│   │   ├── Merchant (Hypnos)
│   │   └── UnlockRequirement
│   ├── Phlegethon
│   │   ├── FishingSpots[]
│   │   ├── Merchant (Hephaestus)
│   │   └── UnlockRequirement
│   └── Cocytus
│       ├── FishingSpots[]
│       ├── Merchant (Nyx)
│       └── UnlockRequirement
└── SpecialLocations[]
    ├── AcherusianLake
    ├── ElysiumShores
    └── TartarusDepths
```

### Fishing Spot

```csharp
[CreateAssetMenu(fileName = "FishingSpot", menuName = "HookedOnKharon/FishingSpot")]
public class FishingSpotDefinition : ScriptableObject
{
    public string spotName;
    public RiverType river;
    public Vector2 mapPosition;

    [Header("Environment")]
    public string[] landmarks; // For rare fish spotting
    public float soulDensity; // Affects fish behavior
    public BackgroundEvent[] possibleEvents;

    [Header("Fish")]
    public FishDefinition[] availableFish;
    public float[] spawnWeights;

    [Header("Hazards")]
    public bool hasHeatHazard;
    public bool hasColdHazard;
    public float hazardTimer;
}
```

---

## Progression System

### Player Progress

```
PlayerProgress
├── Obols (currency)
├── BarterInventory[]
├── OwnedGear[]
├── EquippedGear
│   ├── Rod
│   ├── Line
│   ├── Lure
│   └── ScorchCollar
├── UnlockedRivers[]
├── Codex
│   ├── CaughtFish[]
│   ├── TotalCatches (per fish)
│   └── LegendariesCaught
└── Stats
    ├── TotalFishCaught
    ├── TotalSoulsFerried
    ├── TotalObolsEarned
    └── LongestFishFight
```

### Save System

- Save player progress to file
- Auto-save on significant events
- Support for multiple save slots

---

## Audio System

### Sound Categories

```
AudioSystem
├── Ambient
│   ├── RiverAmbient (per river type)
│   ├── BackgroundEvents (distant)
│   └── UnderworldAtmosphere
├── Character
│   ├── KharonMuttering[]
│   ├── ScorchBark (whoopie cushion + flame)
│   ├── SoulDialogue[]
│   └── MerchantDialogue[]
├── Fishing
│   ├── CastSound
│   ├── ReelSounds
│   ├── LineTension
│   ├── FishSplash
│   └── CatchFanfare (by rarity)
├── UI
│   ├── MenuSounds
│   ├── PurchaseSound
│   └── CodexUnlock
└── Music
    ├── RiverThemes (per river)
    ├── FerryTheme
    ├── ShopTheme
    └── LegendaryFightTheme
```

---

## Project Structure (Proposed)

```
Assets/
├── Art/
│   ├── Models/
│   │   ├── Characters/      # Kharon, Scorch, Merchants, Souls
│   │   ├── Fish/            # All fish species
│   │   ├── Environment/     # Rivers, backgrounds, props
│   │   └── Props/           # Raft, cooler, fishing gear
│   ├── Animations/
│   ├── Materials/
│   └── VFX/                 # Fire effects, ghostly glow
├── Audio/
│   ├── Music/
│   ├── SFX/
│   └── Ambient/
├── Prefabs/
│   ├── Characters/
│   ├── Fish/
│   ├── Environment/
│   └── UI/
├── Scenes/
│   ├── MainMenu/
│   ├── Rivers/
│   └── Shops/
├── Scripts/
│   ├── Core/               # Game managers, state machine
│   ├── Fishing/            # Cast, reel, catch systems
│   ├── Ferry/              # Soul transport, payment
│   ├── Companion/          # Scorch behavior
│   ├── Progression/        # Unlocks, currency, codex
│   ├── UI/                 # Menus, HUD, codex display
│   └── Audio/              # Music, SFX managers
├── ScriptableObjects/
│   ├── Fish/
│   ├── Gear/
│   ├── Merchants/
│   ├── Souls/
│   ├── FishingSpots/
│   └── Events/
└── Settings/
    └── Input/
```

---

## Technical Considerations

### Performance
- Low-poly 3D is inherently performant
- Object pooling for fish spawning
- LOD for distant background events
- Idle mode as lightweight "screensaver"

### Platform Targets
- PC (Steam) - Primary
- Console potential (Switch-friendly due to low-poly)

---

## Development Status

### Current Phase: Concept/Planning

| System | Status |
|--------|--------|
| Core architecture design | COMPLETE |
| Data models | COMPLETE |
| System diagrams | COMPLETE |
| Technical prototype | TODO |
| Asset pipeline | TODO |

---

**End of Architecture Document**
