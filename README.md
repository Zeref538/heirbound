# Heirbound

Top-down 2D action RPG. Unity 6, URP 2D.

> **You wear the dead. When you die, you become something your child wears.**

**Status:** design done, nothing built yet.

---

## What it is

A Zenonia-style action RPG sized for one person. Gear grants your abilities
instead of a skill tree. Death is permanent, and the gear you die in becomes
an heirloom your child inherits — carrying a trait shaped by how you died.

Six to eight generations later you reach the bottom and fight your first
ancestor, wearing the gear you have been inheriting pieces of all game.

---

## Read these in order

| File | What it is |
|---|---|
| [DESIGN.md](DESIGN.md) | The full design. Start here. |
| [docs/ART-PROMPTS.md](docs/ART-PROMPTS.md) | Prompts for generating sprite sheets and tiles |
| [docs/backlog/](docs/backlog/) | Parked concepts from the same brainstorm |

---

## Starting the project

1. **Unity Hub** → **New project**
2. Template: **Universal 2D**
3. Editor version: **6000.5.10f1**
4. Project name: `HeirboundGame`
5. Location: this folder (`Portfolio/Heirbound`)
6. Leave **Use AI Assistant** and **Use Unity CLI** unticked
7. Leave **Source control provider** empty — git is handled here already

Then **Edit → Project Settings → Player → Other Settings → Active Input
Handling**. Set it to **Both** if you plan to use the old `Input.GetAxis`
style. Unity restarts.

---

## Build order

Each stage is playable on its own. Do not start one until the last works.

1. Combat only — move, attack, dodge, one enemy. **Does hitting things feel good?**
2. One item that grants an ability. Equip it, feel the game change.
3. Death and inheritance — die, pick one piece, next character starts with it.
4. Dungeon assembler — rooms shuffle into floors.
5. The Hall — statues, loadout, save file.
6. Boss, curse clock, ending.
7. Letters, blood bonds, the named enemy.

Stopping after stage 3 still leaves something real to show.

---

## Rules that keep this finishable

- **Two buttons, forever.** Attack and dodge. Depth comes from gear, never
  from more inputs.
- **Four enemy types and two bosses.** That is the cap. Top-down art costs
  three directions per animation.
- **Three hand-built maps.** Hall, village, dungeon mouth. Everything else
  is assembled from room prefabs.
- **The out-of-scope list in DESIGN.md is a contract.** No classes, no
  shops, no crafting, no multiple endings.
