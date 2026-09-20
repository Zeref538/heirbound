# Heirbound — design

Top-down 2D action RPG. Unity 6, URP 2D.

> **You wear the dead. When you die, you become something your child wears.**

---

## Why this exists

A Zenonia-style action RPG that one person can actually finish. Every hook
in it was chosen because it *deletes* work as well as adding identity:

- Gear grants abilities, so there is no skill tree, no skill points, no
  class select.
- Death passes gear down, so dying is content instead of a reload.
- The dungeon shuffles from a room pool, so 32 rooms become endless floors.

---

## The core loop

1. Leave the Hall wearing your inheritance
2. Descend, fight, find gear that changes how you play
3. Die
4. Choose **one** piece to pass down
5. Your child starts with it. The Hall gains a statue. The curse deepens.

---

## Combat

One attack button. One dodge. That is the entire base moveset and it never
grows.

Everything interesting comes from equipment. A player with a dash sword and
fire boots plays a different game from one with a healing ring and a heavy
hammer, using the same two buttons.

Top-down, eight-direction movement. Sprites are drawn for **three**
directions — down, up, side — and the side sprite is mirrored for the other
way. Every top-down 2D game does this; it cuts character art by more than
half.

Animations, and no more than these:

- idle
- walk
- attack
- hurt
- death

---

## Gear is the build

Five slots: **weapon, armour, boots, ring, charm.**

Around twenty items. Each carries:

- a name, and the name of who wore it before
- one ability
- a small stat change

Examples of the shape of an ability, not a final list:

| Item | Ability |
|---|---|
| Dueller's blade | every third hit dashes you forward through enemies |
| Ashwalker boots | your dodge leaves a short trail of fire |
| Ring of the glutton | kills heal you, but you take more damage |
| Widow's charm | when an enemy dies near you, the next enemy is slowed |

**Blood bonds.** Wearing two pieces that belonged to the same ancestor grants
a bonus. This is what stops the player simply always equipping the highest
numbers — keeping a family set together has to compete with raw power.

---

## Death and inheritance

Death is permanent for that character. Then:

1. **Pick one piece to pass down.** One. This choice is where the tension
   lives: the sword that got you this far, or the boots that would have
   saved you.
2. **The piece gains a trait from how you died.** Killed by fire, it now
   carries fire resistance. Killed by the same enemy three generations
   running, it carries something about that.
3. **A statue joins the Hall**, wearing what you died in.
4. **Your child begins** with that heirloom and nothing else.

### Fading memory

An ancestor's ability weakens each time it is passed down. Great-
grandfather's sword is a shadow of itself. This stops the player snowballing
forever, and quietly pushes them to create *new* heirlooms instead of
clinging to one.

---

## The world

Three hand-built maps. This is all the map-making in the entire project.

**The Hall** — the hub. A corridor of ancestor statues that grows every
generation. Read who they were and what killed them. Choose your loadout.
The Hall is also the save file made visible.

**The village** — two or three NPCs, the story, the reason any of this
matters.

**The Descent** — the dungeon mouth.

### The dungeon

Four themed floors. Each floor is assembled at runtime from a pool of about
eight hand-made rooms, so 32 rooms produce endless layouts.

Bosses on floors two and four.

---

## The curse, and the ending

Each generation the world worsens: darker, tougher, more corrupted. This is
the difficulty curve and it is free — no balancing sliders, just a
generation counter.

After six to eight generations you reach the bottom and fight your **first
ancestor**, still down there, wearing the gear you have been inheriting
pieces of the whole game.

---

## Story delivery

No cutscenes. No voice acting.

- **Letters.** Each ancestor leaves one short note for the next. Six to
  eight letters across the whole game is the entire script.
- **The statues.** Each one records a name, a death, and what they wore.
- **The village NPCs** age and change between generations.

The story is a family bound to a curse. You are the fourth, fifth, sixth to
try. The mechanic *is* the story — that is the test every idea here had to
pass.

---

## Worth adding if things go well

**The one who killed your father.** The enemy that killed your last
character gets a name, grows stronger, and waits somewhere in the next run.
Kill it and take its gear. One personal grudge per generation. A small
version is cheap — remember one enemy, buff it, name it.

---

## Explicitly out of scope

Say no now, not in month three.

- Multiple classes — your gear already is your class
- Towns, shopkeepers, quest givers — the Hall is the only hub
- A crafting system — loot *is* the crafting
- Multiple endings
- Online anything
- Procedurally generated *items* — twenty hand-made ones are better

---

## How it is built

### Items are ScriptableObjects

A ScriptableObject is a Unity asset that holds data as a file in your
Project window. You create an item the same way you create a sprite: right
click, fill in name, icon and ability in the Inspector, save.

Twenty items becomes twenty small asset files instead of a thousand-line
script, and you can add a twenty-first without touching code. For an RPG
this is the single most useful Unity concept there is.

### Rooms are prefabs

A prefab is a saved GameObject you can stamp copies of. Each dungeon room is
one. A small assembler script picks rooms from a pool and lines up their
doorways.

### The save file is the Hall

A list of ancestors: name, what they wore, what killed them, which piece was
passed down. Nothing else needs saving, because a run is one sitting.

---

## Risks

| Risk | Why it bites | What to do about it |
|---|---|---|
| Combat feel | If hitting things is not satisfying, no amount of loot saves it | Stage 1 of the build order is nothing but combat. Do not move on until it feels good |
| Top-down art volume | 3 directions x 5 animations x every character | Be ruthless: one player, four enemy types, two bosses. That is the cap |
| Systems talking to each other | Seven small systems still have to connect | Each build stage below must be playable before the next starts |
| Scope creep | RPGs invite "just one more system" | The out-of-scope list above is a contract |

---

## Build order

Each stage is playable on its own. If you stop at stage 3 you still have
something real to show.

1. **Combat only.** Top-down movement, attack, dodge, one enemy. No RPG at
   all. The question being answered is: does hitting things feel good?
2. **One item that grants an ability.** Equip it, feel the game change.
   This proves the whole concept.
3. **Death and inheritance.** Die, choose one piece, next character starts
   with it.
4. **The dungeon assembler.** Rooms shuffle into floors.
5. **The Hall.** Statues, loadout screen, save file.
6. **Boss, curse clock, ending.** Now it is a game.
7. **Letters, blood bonds, the named enemy.** The polish people remember.
