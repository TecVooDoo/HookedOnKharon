# Hooked on Kharon: Reeling in the Underworld - Design Decisions

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

## Purpose

This document tracks design decisions, rationale, and lessons learned for Hooked on Kharon development.

---

## Core Design Decisions

### Inspiration Source

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Base mechanics | Cast n' Chill | Proven cozy fishing formula, simple and accessible |
| Setting | Greek Underworld | Unique twist on fishing sim, rich mythology to draw from |
| Protagonist | Kharon the Ferryman | Built-in reason to be on water, iconic character |

### Art Style

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Visual style | Low-poly 3D | Son is 3D artist - plays to team strengths |
| NOT pixel art | Confirmed | Despite Cast n' Chill inspiration, 3D is the path |

---

## Character Design Decisions

### Kharon's Dual Persona

| Decision | Choice | Rationale |
|----------|--------|-----------|
| On-duty appearance | Tall, imposing, silent | Maintains mythological gravitas |
| Off-duty reveal | Smaller skeleton on cooler | Comedy gold, humanizes the character |
| The cooler secret | Height is faked | No one in underworld knows - adds humor |
| Silence on duty | Never speaks to souls | Professional, intimidating, myth-accurate |
| Muttering off-duty | Talks to himself/Scorch | Shows real personality, provides rare fish cues |

**Why This Works:**
- Creates two distinct gameplay feels (tense ferry vs. relaxed fishing)
- The "reveal" is an ongoing visual gag
- Maintains respect for the mythological source while adding humor

### Scorch the Hell Hound

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Companion type | Hell hound puppy | Fits underworld, cuter than adult Cerberus |
| Name | Scorch | Fire-themed, playful, easy to remember |
| Bark sound | Whoopie cushion + flame | Dark comedy, memorable, unique |
| Hidden bark mechanic | Fire exits back of robes | Souls think Kharon farted - perfect humor |
| Proximity detection | Flame intensity increases | "Hot/cold" system without breaking immersion |

**Why NOT other companions:**
- Cerberus: Too big, already used (Hades fetch)
- Lost Soul: Would need dialogue, Kharon doesn't talk
- Owl/Serpent: Less personality potential

---

## Gameplay Decisions

### Ferry as Interruption (Not Separate Mode)

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Ferry integration | Interrupts fishing | Creates rhythm, reinforces Kharon's irritation |
| Player control | Cannot ignore souls | Must ferry them (or reject non-payers) |
| Compensation | Rare fish spotting during ferry | Makes interruption feel valuable |

**Rejected Alternatives:**
- Separate ferry mode: Splits attention, loses the "interruption" feel
- Optional ferrying: Undermines Kharon's identity as ferryman
- Background income only: Wastes unique mechanic potential

### Rare Fish Spotting System

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Spotting trigger | During ferry runs only | Rewards the "interruption" |
| Location memory | Environmental markers, no tech | Fits underworld setting, adds skill |
| Kharon's muttering | 3 cues then soul interrupts | Audio reinforcement, comedy beat |
| Backtracking | Player must return manually | Creates meaningful choice |

**The Muttering Pattern:**
```
Mutter 1 → Pause → Mutter 2 → Pause → Mutter 3 → Soul speaks → Kharon silent
```
This gives players three chances to memorize, then comedic interruption.

### Soul Density Mechanic

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Too many souls | Fish scatter | Creates "bad" spots, strategic avoidance |
| Feeding areas | Fish grow monstrous | Risk/reward spots, bigger catches |

This creates location strategy beyond just "go to next river."

---

## Merchant System Decisions

### Five Merchants, Five Rivers

| River | Merchant | Why This Merchant |
|-------|----------|-------------------|
| Acheron | Damned Merchant | Ironic punishment (cheater serves forever), good dark humor |
| Styx | Hermes | Already in underworld guiding souls, fast-talker fits commerce |
| Lethe | Hypnos/Dreaming Shade | Forgetfulness theme, sleepy personality is funny |
| Phlegethon | Hephaestus's Shade | Craftsman god, fire river needs fire-resistant gear |
| Cocytus | Nyx | Primordial goddess, endgame mystique, riddle-speaker |

**Rejected:**
- Kharon's Own Stash: Wanted NPC interaction, not just menu
- Single merchant: Five rivers deserve five personalities

---

## Technical Decisions

### No Fish Finders / Tech

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Fish detection | Scorch + environmental observation | Fits ancient Greek setting |
| Minimap | None for rare fish | Player memory is the challenge |
| Landmarks | Named environmental features | "Skull rock," "weeping bank" - immersive |

This is a deliberate friction point that adds skill and immersion.

### Dual Fishing Modes

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Active mode | Manual cast/reel with tension | Skill-based, engaging |
| Idle mode | Auto-fish, reduced rare chance | Cozy "screensaver" option |

Directly from Cast n' Chill - proven to work.

---

## Scope Decisions

### MVP Scope

**In MVP:**
- 2-3 rivers (Acheron, Styx, maybe Lethe)
- Core fishing loop
- Ferry interruption system
- Scorch companion
- 1-2 merchants
- 15-20 fish species
- Basic progression

**Post-MVP:**
- All 5 rivers
- All 5 merchants
- Full fish codex (50+ species)
- Special guests
- Background events
- Legendary fish
- Full upgrade tree

### What We're NOT Doing

| Feature | Reason |
|---------|--------|
| Co-op | Scope creep, Kharon is a loner |
| Motion controls | Not needed for core experience |
| Mobile port | Focus on PC first |
| Competitive modes | Cozy game, not competitive |

---

## Open Questions

| Question | Status | Notes |
|----------|--------|-------|
| Unity version | OPEN | 6.x recommended |
| Total fish species count | OPEN | 50+ target |
| Legendary fish mechanics | OPEN | Multi-phase fights? Special conditions? |
| Scorch upgrades | OPEN | Collar that improves detection? |
| Background event frequency | OPEN | How often should Cerberus appear? |

---

## Red Flags / Watch Items

| Flag | Severity | Notes |
|------|----------|-------|
| Scope creep on fish species | MEDIUM | Start with core set, expand later |
| Ferry interruption frequency | HIGH | Must balance - too often = annoying |
| Difficulty curve across rivers | MEDIUM | Need playtesting |
| Art pipeline with son | LOW | Communication is key |

---

## Lessons from Cast n' Chill

| Cast n' Chill Feature | Our Adaptation |
|-----------------------|----------------|
| Dual modes (active/idle) | Keep as-is - proven |
| Resistance-based reeling | Keep as-is - proven |
| Dog companion | Hell hound with fire twist |
| 16 fishing spots | 5 rivers with multiple spots each |
| Time of day effects | River hazards instead (no sun in underworld) |
| Rusty's Bait Shop | 5 mythological merchants |

---

## Lessons from Other TecVooDoo Projects

| Lesson | Application |
|--------|-------------|
| SOAP architecture | Use for fish, gear, merchant definitions |
| Profile early | Low-poly should be fine, but verify |
| Document pivots | This doc tracks all decisions |
| Keep scope realistic | MVP first, expand later |

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
| Dec 25, 2025 | Title: Hooked on Kharon: Reeling in the Underworld | Catchy + descriptive |

---

## Reference Materials

### Concept Images (in project folder)
- Charrons_Fishing_Concept_1/2/3.png
- Cast n' Chill screenshots
- River reference images (Acheron, Styx, Lethe, Phlegethon, Cocytus)
- Kharon reference art
- Erebos underworld map

### External References
- Cast n' Chill (Steam) - Gameplay inspiration
- Ancient Greece Reloaded - Five Rivers mythology
- Hades (Supergiant) - Visual tone reference

---

**End of Design Decisions Document**
