# Hooked on Kharon: Reeling in the Underworld - Project Instructions

**Project:** Hooked on Kharon: Reeling in the Underworld
**Developer:** TecVooDoo LLC
**Designer:** Rune (Stephen Brandon)
**3D Artist:** Son
**Project Path:** E:\Unity\Hooked On Kharon
**Document Version:** 2
**Last Updated:** December 28, 2025

---

## Shared Documentation

**This project follows TecVooDoo standards. Review these documents:**

| Document | Location | Purpose |
|----------|----------|---------|
| Core Protocols | `E:\TecVooDoo\Projects\Documents\CORE_DevelopmentProtocols.md` | Universal development rules |
| Unity Standards | `E:\TecVooDoo\Projects\Documents\Type\TYPE_Unity.md` | Unity-specific patterns and tools |

---

## Project Paths

**Planning Location:** `E:\Unity\Hooked On Kharon`

**When development begins:** `E:\Unity\Hooked On Kharon`

---

## Project Documents

| Document | Purpose |
|----------|---------|
| HC_GDD | Game design and mechanics |
| HC_Architecture | Tech stack, systems, code structure |
| HC_DesignDecisions | History, lessons learned, decisions |
| HC_ProjectInstructions | Development protocols (this document) |

**Naming Convention:** `HC_DocumentName_v#_MMDDYYYY.md`

All four documents share the same version number. Increment all when any document is updated.

---

## Team

| Role | Person |
|------|--------|
| Designer/Developer | Rune (Stephen Brandon) |
| 3D Artist | Son |

---

## Project Overview

**Hooked on Kharon** is a cozy low-poly 3D fishing sim set in the Greek Underworld. Players control Kharon, the eternal ferryman who just wants to fish in peace.

**Tagline:** "Even death needs a break."

---

## Key Design Elements (Non-Negotiable)

These elements are locked in and should not be changed without discussion:

### Kharon's Dual Persona
- **On-duty:** Tall, imposing (standing on cooler), silent, professional
- **Off-duty:** Smaller skeleton revealed, sitting on cooler, mutters to himself
- **The secret:** No one knows he's short - the cooler is his height prop

### Scorch the Hell Hound
- Small hell hound companion
- **Bark:** Whoopie cushion sound + small flame
- Proximity detection via flame intensity

### Five Rivers, Five Merchants

| River | Merchant |
|-------|----------|
| Acheron | Damned Merchant |
| Styx | Hermes |
| Lethe | Hypnos/Dreaming Shade |
| Phlegethon | Hephaestus's Shade |
| Cocytus | Nyx |

### No Tech in Underworld
- No fish finders or depth sensors
- Use Scorch + environmental landmarks
- Player memory for rare fish spots

---

## Art Style

- **Low-poly 3D** (non-negotiable - son is 3D artist)
- Stylized, not realistic
- Each river has distinct color palette
- Ghostly/ethereal effects on fish

---

## Development Status

### Current Phase: Concept/Planning

| Item | Status |
|------|--------|
| High concept | COMPLETE |
| Core mechanics design | COMPLETE |
| Character design | COMPLETE |
| GDD | COMPLETE |
| Architecture doc | COMPLETE |
| Unity project | NOT STARTED |
| Prototype | NOT STARTED |

---

## Reference Materials

### In Project Folder
- Concept images (Charrons_Fishing_Concept_1/2/3.png)
- Cast n' Chill reference screenshots
- River reference images

### External
- Cast n' Chill (Steam) - Gameplay reference
- Hades (Supergiant) - Visual tone reference

---

## Project-Specific Notes

- Coordinate art asset list with son before development
- Low-poly should be fine for performance, but verify
- SOAP architecture works well for fish, gear, merchant definitions

---

## Known Issues / TODO for Next Session

- Select Unity version (6.x recommended)
- Create Unity project structure
- Define MVP scope precisely
- Begin prototype of core fishing loop
- Coordinate art asset list with son

---

**End of Project Instructions**

Review CORE_DevelopmentProtocols.md and TYPE_Unity.md for full development standards.
