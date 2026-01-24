# Hooked on Kharon - Ferry Mechanics Additions

**Source:** Claude conversation, January 6, 2026
**Purpose:** New sections to integrate into HookedOnKharon_Status.md

---

## NEW SECTION: Ferry Mechanics

*(Add after "Fishing Mechanics" section)*

### Soul Decay System

Souls are unstable in the Underworld. Without proper passage, they fade - and so does Kharon's payment.

**Core Rule:** Each soul has a decay timer. If it expires before delivery, the soul fades and Kharon receives nothing.

**Design Intent:** Creates tension between fishing (relaxation) and ferrying (duty). Players can't ignore souls indefinitely, but rushing means abandoning good fishing spots.

**Decay Rate by Soul Type:**

| Soul Type | Base Timer | Notes |
|-----------|------------|-------|
| Common Shade | Standard | Baseline decay rate |
| Wealthy Dead | Standard | Full obol payment, no rush |
| Poor/Unpaid | Short | Already spiritually weak |
| Oath-Breaker | Fast | Spiritually compromised |
| Hero | Slow | Purpose holds them together |
| Special Guest | Very Slow | Plot armor, essentially |

**Decay Modifiers by River:**

| River | Effect on Decay | Strategic Use |
|-------|-----------------|---------------|
| Acheron | Normal (1x) | Safe default route |
| Styx | Normal, but rejects oath-breakers | Honor-only express lane |
| Lethe | Soul confusion (see below) | Risky for disoriented souls |
| Phlegethon | Accelerated (1.5x-2x) | Fast but dangerous shortcut |
| Cocytus | Slowed (0.5x) | Preservation route, slow travel |

**Lethe Special Effect:** Souls don't decay faster, but they forget their destination. UI shows destination flickering/fading. If fully forgotten, soul wanders off at next hub - no payment, no penalty, just... gone. Proper Lethe gear (for Kharon) prevents this.

---

### River Navigation System

**Structure:** Hub-and-spoke with horizontal rivers and vertical connectors.

```
                    COCYTUS (frozen, endgame)
                         |
                    [Connector]
                         |
    ACHERON ---[Hub]--- STYX
   (starter)     |    (early-mid)
            [Connector]
                 |
               LETHE (mid)
                 |
            [Connector]
                 |
            PHLEGETHON (advanced)
```

**Hub Location:** Near the Judges' Platform (Minos, Rhadamanthus, Aeacus). Souls are judged here and assigned destinations. Natural crossroads.

**Connector Mechanics:**
- Vertical transitions between rivers
- Visual: Kharon poles up/down, camera shows him from back/front
- Scene transition to new river (like 2D RPG door/portal)
- Some connectors one-way (waterfall down, can't go back up)
- Some connectors locked until gear requirements met

**Blocked Routes:**
- Random blockages force alternate routing
- Examples: Fallen pillar, soul traffic jam, river dried up temporarily
- Creates routing decisions: "Direct path blocked, do I risk Phlegethon shortcut?"

**Hades' Mood System:**

Daily/session modifier affecting available routes.

| Mood | Cause | Effect |
|------|-------|--------|
| Brooding | Default, Persephone away | Standard routes only |
| Content | Persephone home, good day | Shortcuts open |
| Wrathful | Player angered him somehow | Extra blockages |
| Generous | Delivered VIP soul | Bonus route opens temporarily |

*Note: Persephone's presence could be seasonal (spring/summer she's above) or random for gameplay variety.*

---

### Ferry Routing Decisions

When a soul needs transport, player considers:

1. **Destination** - Which river bank? Which side of hub?
2. **Soul stability** - How fast are they decaying?
3. **Current location** - How far from pickup?
4. **Available routes** - What's open today?
5. **Risk tolerance** - Shortcut through Phlegethon or safe route through Cocytus?

**Example Decision:**
> Soul appears on Acheron, needs delivery to far bank of Styx.
> Direct route: Acheron -> Hub -> Styx (medium time, safe)
> Shortcut: Acheron -> Phlegethon connector -> Styx back entrance (fast, but 1.5x decay)
> Safe route: Acheron -> Hub -> Cocytus -> Styx (slow, but 0.5x decay)
>
> If soul is an oath-breaker (fast decay), Cocytus route might be only option.
> If soul is a hero (slow decay), Phlegethon shortcut is viable.

---

### Alternative Payments

Souls without obols may offer items instead. These become unique gear with attached lore.

**Payment Hierarchy:**
1. **Full Obols** - Standard payment, full decay timer
2. **Partial Obols** - Reduced payment, standard timer
3. **Valuable Item** - Unique gear piece, standard timer
4. **Minor Item** - Common crafting material, reduced timer
5. **Nothing** - Rejected (mythologically accurate - 100 years wandering)

**Bartered Gear Examples:**

| Item | Source Soul | Gear Type | Effect | Flavor Text |
|------|-------------|-----------|--------|-------------|
| Fisherman's Last Lure | Drowned fisherman | Lure | Attracts spectral fish differently | "He clutched it as he sank. Now it glows faintly, remembering water that wasn't so dark." |
| Sailor's Enchanted Rope | Lost sailor | Line | Heat-resistant (Phlegethon) | "Blessed by Poseidon for a voyage that never ended. The sea god's magic holds, even here." |
| Smith's Final Hook | Blacksmith | Hook | +Durability | "Forged with his last breath. The metal remembers the forge's heat." |
| Theseus's Thread | Theseus (special guest) | Line | Lethe-resistant (no tangle in mist) | "It found its way through the labyrinth. It can find its way through fog." |
| Ajax's Clasp | Ajax (special guest) | Reel component | +Fight strength | "He fell on his own sword, but his grip never weakened." |
| Soldier's Last Arrow | Fallen archer | Lure (spear-type) | Deep water effectiveness | "Aimed at an enemy he never saw fall. Now it seeks different prey." |
| Poet's Final Verse | Dead poet | Bait | Attracts Lethe fish specifically | "The last words he composed. The fish here hunger for stories." |
| Widow's Lock of Hair | Grieving soul | Charm | Slight decay resistance | "Given in love. Even death respects that." |

**Design Note:** These items have stories. The codex entry isn't just stats - it's who gave it to you and why. Players might remember "the drowned fisherman's lure" better than "Spectral Lure Mk. II."

---

### Fishing Skill Unlocks Ferry Routes

Mastering a river's fishing challenges also unlocks it for ferrying.

**Progression Logic:**
1. Player can only ferry through rivers they can fish in
2. To fish in Phlegethon, need heat-resistant gear
3. Heat-resistant gear also protects raft during ferry
4. Therefore: fishing progression = ferry route expansion

**Unlock Requirements:**

| River | Fishing Unlock | Ferry Unlock |
|-------|----------------|--------------|
| Acheron | Default | Default |
| Styx | Basic rod upgrade | Same |
| Lethe | Mist-resistant line | Same + Lethe charm (prevents Kharon confusion) |
| Phlegethon | Heat-resistant full kit | Same (gear protects raft) |
| Cocytus | Cold-resistant full kit | Same (gear prevents freeze) |

**Design Intent:** Players can't just grind ferry routes for money. They MUST engage with fishing to unlock shortcuts. The two halves of the game feed each other.

---

## NEW SECTION: Expanded Creatures

*(Add to or expand "Fish Species" section)*

### Beyond Ghost Fish

The rivers contain more than spectral versions of normal fish.

**Mythological Creatures:**

| Creature | River | Description | Catch Difficulty |
|----------|-------|-------------|------------------|
| Ketea Spawn | Cocytus | Lesser sea monsters, frozen mid-thrash | Rare |
| Oath-Manifestation Eel | Styx | Broken promises made physical, slippery | Uncommon |
| Forgotten Thing | Lethe | Creature that forgot what it was - wrong number of fins, confused anatomy | Rare |
| Carcinos | Acheron riverbed | Shade of the giant crab Hera sent against Heracles | Legendary |
| Hydra Remnant | Acheron | Lesser spawn, may "split" in inventory taking extra space | Rare |
| Lamprey of Styx | Styx | Attaches to oath-breakers, can damage line/gear | Uncommon (risky) |
| Salamander | Phlegethon | Classical fire-dweller, not the amphibian | Uncommon |
| Volcanic Shade | Phlegethon | Creatures that died in eruptions, still burning | Rare |
| Lamentation Lamprey | Cocytus | Feeds on grief, emits wailing sound | Uncommon |

**Creature Behaviors:**

- **Hydra Remnant:** When caught, has chance to "split" - suddenly takes 2 inventory slots instead of 1. Risk/reward for cooler management.
- **Lamprey of Styx:** Can be caught voluntarily, but may damage gear in the process. High value if landed safely.
- **Forgotten Thing:** Appearance randomized each catch - players never know exactly what they're reeling in from Lethe.
- **Oath-Manifestation Eel:** Extremely slippery. High escape chance even when hooked. Requires patience.

---

## NEW SECTION: Background Events (Detailed)

*(Expand existing "Background events system" in Future TODO)*

### Mythological Scenery by River

Events visible in the distance while fishing. No interaction, no pop-ups - just there for players who notice.

**Acheron (Pain/Woe):**

| Event | Description | Placement |
|-------|-------------|-----------|
| Sisyphus | Man endlessly pushing boulder uphill, rolls back down | Visible on far bank, near Tartarus entrance |
| Wandering Shades | Souls who couldn't pay, drifting along banks | Throughout, ambient |
| The Judges | Minos, Rhadamanthus, Aeacus deliberating | Near hub, souls lined up waiting |

**Styx (Hatred/Oaths):**

| Event | Description | Placement |
|-------|-------------|-----------|
| Oath Echoes | Ghostly figures shouting broken promises at each other | Occasional, startling |
| The Erinyes | Furies circling in the distance, hunting oath-breakers | Rare, ominous |
| Thetis Bathing Achilles | Very rare, mythological deep cut | Easter egg location |

**Lethe (Forgetfulness):**

| Event | Description | Placement |
|-------|-------------|-----------|
| Souls Drinking | Shades drinking from the river, expressions going blank | Common |
| Tantalus | Standing in water, reaching for fruit that pulls away | Visible from certain spots (irony: near forgetfulness, he can't forget his hunger) |
| Hypnos Sleeping | The god of sleep, dozing by the bank | Near his shop location |

**Phlegethon (Fire):**

| Event | Description | Placement |
|-------|-------------|-----------|
| Ixion's Wheel | Man bound to burning wheel, spinning eternally | Visible on banks |
| Pyriphlegethon Falls | Waterfall of fire, spectacular | Major landmark |
| Hephaestus's Old Forge | Abandoned divine smithy, still glowing | Near merchant location |

**Cocytus (Lamentation):**

| Event | Description | Placement |
|-------|-------------|-----------|
| The Danaids | 49 women carrying water in leaky jars, endless procession | Distant, haunting |
| Frozen Titans | Massive shapes under the ice | Deep water areas |
| Mourning Chorus | Shades wailing together, sound carries | Ambient audio event |

---

## UPDATED SECTION: Hollywood Gag Details

*(Add to "Key Design Elements" or create new "Comedy/Tone" section)*

### The Misconception IS the Costume

**The Joke:** Pop culture depicts Kharon as a tall, skeletal, hooded figure of dread. In this game, that's literally a costume. The real Kharon is a short, grumpy old man - which is actually closer to the ancient sources (Virgil, Aristophanes describe a scraggly bearded ferryman, not a skeleton).

**Visual Gag Breakdown:**

| Mode | What Souls See | Reality |
|------|----------------|---------|
| On-Duty | Towering robed figure, skull face, silent menace | Short old man standing on cooler, wearing mask, too tired to talk |
| Off-Duty | N/A (souls delivered) | Robes unzipped, sitting on cooler, muttering about fish, petting Scorch |

**Soul Comments (Occasional Dialogue):**

> "I thought there'd be more fire."
> "Aren't you supposed to be... taller?"
> "The paintings made you look more... skeletal."
> "Is that a dog under there?"
> "My grandmother said you had burning eyes."
> [Kharon says nothing. Poles faster.]

**Scorch Under Robes:**

- Small movements occasionally visible
- Flame exits out the back when Scorch gets excited
- Souls whisper to each other: "Did... did Kharon just fart fire?"
- Kharon remains stoically silent

**The Unmasking:**

When transitioning from On-Duty to Off-Duty, the reveal sequence:

1. Kharon checks coast is clear (no souls watching)
2. Unzips robes
3. Steps off cooler (height drops noticeably)
4. Removes skull mask, hangs it on raft pole
5. Revealed: tired, bearded old man face
6. Sits on cooler with a sigh
7. Scorch emerges, gets pets
8. Mutters something like "Finally. Where were those bubbles..."

---

## ADDITIONS TO EXISTING SECTIONS

### Add to "Open Questions"

| Question | Status | Notes |
|----------|--------|-------|
| Soul decay timer values | OPEN | Needs playtesting for feel |
| Hub location specifics | OPEN | Near Judges? Palace? Both? |
| Hades mood trigger frequency | OPEN | Daily? Per session? Random events? |
| Lethe confusion mechanic details | OPEN | UI treatment for forgetting destination |
| Alternative payment drop rates | OPEN | How often do souls offer gear vs. nothing? |

### Add to "Red Flags / Watch Items"

| Flag | Severity | Notes |
|------|----------|-------|
| Soul decay timer tuning | HIGH | Too fast = stressful, too slow = no tension |
| Route complexity | MEDIUM | Hub system must be intuitive, not confusing |
| Alternative payment balance | MEDIUM | Unique gear must feel special, not farmable |

### Add to "Decision Log"

| Date | Decision | Rationale |
|------|----------|-----------|
| Jan 6, 2026 | Soul decay timer system | Creates ferry stakes, balances fishing tension |
| Jan 6, 2026 | Hub-based river navigation | 2.5D friendly, familiar portal/door pattern |
| Jan 6, 2026 | Fishing unlocks ferry routes | Links both halves mechanically |
| Jan 6, 2026 | Alternative payments become gear | Narrative reward, memorable items |
| Jan 6, 2026 | Hades mood as route modifier | Dynamic routing, mythology tie-in |
| Jan 6, 2026 | Detailed background events | Rewards mythology knowledge, no tutorial needed |

### Add to "Lessons Learned"

| # | Lesson | Source |
|---|--------|--------|
| 8 | Ferry mechanics need equal depth to fishing | Design conversation Jan 2026 |
| 9 | Interconnected systems > separate systems | Soul decay ties fishing gear to ferry routes |
| 10 | Environmental storytelling > exposition | Background events, no pop-ups |

---

## UPDATED MVP SCOPE

**In MVP:**
- 2-3 rivers (Acheron, Styx, maybe Lethe)
- Core fishing loop (Active + Idle)
- Ferry interruption system
- **Soul decay timer (basic implementation)**
- **Hub navigation (simplified - maybe just Acheron/Styx + Hub)**
- Scorch companion with proximity detection
- 1-2 merchants
- 15-20 fish species
- Basic progression
- **2-3 alternative payment items**

**NOT in MVP:**
- All 5 rivers
- Special guests
- Full background events (maybe 1-2 as proof of concept)
- Legendary fish
- Full upgrade tree
- **Hades mood system**
- **Complex route blockages**
- **Full alternative payment catalog**

---

## NEW DATA MODEL: Soul Definition

*(Add to "Data Models" section)*

```csharp
[CreateAssetMenu(fileName = "Soul", menuName = "HookedOnKharon/SoulDefinition")]
public class SoulDefinition : ScriptableObject
{
    public string soulName;
    public string description;
    public Sprite portrait;
    public GameObject prefab;

    [Header("Classification")]
    public SoulType soulType;
    public bool isSpecialGuest;
    public bool isOathBreaker;

    [Header("Decay")]
    public float baseDecayTime;
    public float decayMultiplier; // Modified by river

    [Header("Payment")]
    public PaymentType paymentType;
    public int obolAmount;
    public GearDefinition barterItem; // If paying with item

    [Header("Destination")]
    public RiverType destinationRiver;
    public string destinationLandmark;

    [Header("Dialogue")]
    [TextArea] public string[] waitingLines; // Said while waiting for pickup
    [TextArea] public string[] travelLines; // Said during ferry (Kharon ignores)
    [TextArea] public string[] deliveryLines; // Said on arrival

    [Header("Lore")]
    [TextArea] public string codexEntry;
    public string causeOfDeath;
    public string mythologicalReference; // Empty for common souls
}

public enum PaymentType { FullObol, PartialObol, BarterItem, Nothing }
```

---

## NEW DATA MODEL: River Connection

*(Add to "Data Models" section)*

```csharp
[CreateAssetMenu(fileName = "RiverConnection", menuName = "HookedOnKharon/RiverConnection")]
public class RiverConnectionDefinition : ScriptableObject
{
    public string connectionName;
    public RiverType fromRiver;
    public RiverType toRiver;

    [Header("Requirements")]
    public GearDefinition[] requiredGear;
    public bool requiresBothDirections; // false = one-way

    [Header("Effects")]
    public float soulDecayModifier; // 1.0 = normal, 1.5 = faster, 0.5 = slower
    public bool blocksOathBreakers;

    [Header("Visuals")]
    public string transitionAnimationKey;
    [TextArea] public string flavorText;
}
```

---

**End of Additions Document**
