# Hooked on Kharon: Reeling in the Underworld - Project Instructions

**Project:** Hooked on Kharon: Reeling in the Underworld
**Developer:** TecVooDoo LLC
**Designer:** Rune (Stephen Brandon)
**3D Artist:** Son
**Project Path:** E:\Unity\Hooked On Kharon
**Document Version:** 1
**Last Updated:** December 25, 2025

---

## Version History

| Version | Date | Summary |
|---------|------|---------|
| v1 | Dec 25, 2025 | Initial document - concept phase |

---

## IMPORTANT: Project Path

**The project is located at:** `E:\Unity\Hooked On Kharon`

This is a **Planning** stage project. When development begins, Unity project will be at: `E:\Unity\Hooked On Kharon`

---

## Project Overview

**Hooked on Kharon: Reeling in the Underworld** is a cozy low-poly 3D fishing sim set in the Greek Underworld. Players control Kharon, the eternal ferryman who just wants to fish in peace - but souls keep interrupting.

**Tagline:** "Even death needs a break."

**Core Concept:**
- Cast n' Chill inspired fishing mechanics
- Greek Underworld setting with five mythological rivers
- Ferry interruptions create gameplay rhythm
- Hell hound companion (Scorch) helps find fish
- Dark comedy through Kharon's grumpy personality

---

## Critical Development Protocols

### NEVER Assume Names Exist

**CRITICAL: Verify before referencing**

- NEVER assume file names, method names, or class names exist
- ALWAYS read/search for the actual names in the codebase first
- Incorrect assumptions waste context and force complete rewrites

### Step-by-Step Verification Protocol

**CRITICAL: Never rush ahead with multiple steps**

- Provide ONE step at a time
- Wait for user confirmation via text OR screenshot before proceeding
- User will verify each step is complete before moving forward
- If a step fails, troubleshoot that specific step before continuing
- Assume nothing - verify everything

### Documentation Research Protocol

**CRITICAL: Use current documentation**

- ALWAYS fetch the most up-to-date documentation before making recommendations
- User prefers waiting for accurate information over redoing work later
- Do not rely on potentially outdated knowledge - verify current APIs and patterns

---

## File Conventions

### Encoding Rule

**CRITICAL: ASCII Only**

- All scripts and text files MUST use ASCII encoding
- Do NOT use UTF-8 or other encodings
- Avoid special characters, smart quotes, em-dashes
- Use standard apostrophes (') not curly quotes
- Use regular hyphens (-) not em-dashes

### Core Document Naming Convention

**Format:** `HC_DocumentName_v#_MMDDYYYY.md`

**Rules:**
- All four core documents share the SAME version number
- Increment version for ALL documents when ANY document is updated
- If a document has no changes, update the filename only (no content changes needed)
- Move old versions to `Documents/Archives/` folder

**Core Documents:**
- `HC_ProjectInstructions_v#_MMDDYYYY.md`
- `HC_GDD_v#_MMDDYYYY.md`
- `HC_Architecture_v#_MMDDYYYY.md`
- `HC_DesignDecisions_v#_MMDDYYYY.md`

---

## Project Documents

| Document | Purpose |
|----------|---------|
| HC_GDD | Game design and mechanics |
| HC_Architecture | Tech stack, systems, code structure |
| HC_DesignDecisions | History, lessons learned, decisions, red flags |
| HC_ProjectInstructions | Development protocols (this document) |

**Note:** User will ask Claude to review core docs at the start of each chat.

---

## Team

| Role | Person |
|------|--------|
| Designer/Developer | Rune (Stephen Brandon) |
| 3D Artist | Son |

### Art Style
- **Low-poly 3D** (non-negotiable - son is 3D artist)
- Stylized, not realistic
- Each river has distinct color palette
- Ghostly/ethereal effects on fish

---

## Key Design Elements (Non-Negotiable)

These elements are locked in and should not be changed:

### Kharon's Dual Persona
- **On-duty:** Tall, imposing (standing on cooler), silent, professional
- **Off-duty:** Smaller skeleton revealed, sitting on cooler, mutters to himself
- **The secret:** No one knows he's short - the cooler is his height prop

### Scorch the Hell Hound
- Small hell hound companion
- Hides in robes during ferry duty
- Curls under cooler during fishing
- **Bark:** Whoopie cushion sound + small flame
- **Hidden bark:** Fire exits back of Kharon's robes (souls think he farted)
- Proximity detection via flame intensity

### Ferry Interruption
- Souls interrupt Kharon's fishing
- He must ferry them (grudgingly)
- Non-payers are left behind (myth-accurate)
- Rare fish can be spotted during ferry runs
- Kharon mutters landmarks 3x, then soul interrupts, he goes silent

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

## Coding Standards

### Code Style
- Use explicit type declarations (no `var`)
- PascalCase for public, camelCase for private
- Keep scripts focused and small

### File Size Limits
**800 lines maximum per script**
- When a script approaches 800 lines, refactor
- Extract logic into separate classes or services

### Architecture
- SOAP pattern (ScriptableObject Architecture)
- Event channels for decoupled communication
- Data-driven design via ScriptableObjects

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
| Design decisions doc | COMPLETE |
| Project instructions | COMPLETE |
| Unity project | NOT STARTED |
| Prototype | NOT STARTED |

---

## Reference Materials

### In Project Folder
- Concept images (Charrons_Fishing_Concept_1/2/3.png)
- Cast n' Chill reference screenshots
- River reference images
- Erebos underworld map
- Kharon reference art

### External
- Cast n' Chill (Steam) - Gameplay reference
- Ancient Greece Reloaded - Five Rivers of the Underworld
- Hades (Supergiant) - Visual tone reference

---

## Quick Reference Checklist

**Always:**
- [ ] Verify file/method/class names exist before referencing
- [ ] Wait for user verification before proceeding
- [ ] Use ASCII encoding only
- [ ] Use explicit types (no `var` keyword)
- [ ] Follow SOAP architecture patterns
- [ ] Fetch current documentation before recommendations

**Never:**
- [ ] Assume names exist without checking
- [ ] Rush ahead with multiple steps
- [ ] Use UTF-8 or special characters
- [ ] Use `var` keyword
- [ ] Make recommendations based on potentially outdated docs
- [ ] Change non-negotiable design elements without discussion

---

## Lessons from Other TecVooDoo Projects

1. **SOAP architecture works** - Use for fish, gear, merchant definitions
2. **Profile early** - Low-poly should be fine, but verify
3. **Document pivots clearly** - Design Decisions doc tracks all changes
4. **Keep scope realistic** - MVP first, expand later
5. **Communication with art team** - Coordinate with son on asset needs

---

## Known Issues / TODO for Next Session

- Select Unity version (6.x recommended)
- Create Unity project structure
- Define MVP scope precisely
- Begin prototype of core fishing loop
- Coordinate art asset list with son

---

**End of Project Instructions**

These instructions should be followed for every conversation in this project. User will ask Claude to review these docs at the start of each chat session.
