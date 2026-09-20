# Inheritance

**Status: backlog** — parked; starting with something 2D first

Genre: first-person 3D puzzle
Engine: Unity 6, URP
Target length: 45–60 minutes

---

## Hook

Two people remember the same house differently. You have to live in both
memories at once.

---

## The mechanic

One key swaps the room between **her memory** and **his memory**.

The two versions are the same room and not the same room. Walls move.
A staircase in her version is a blank wall in his. A door he remembers
being locked is standing open in hers. Furniture appears, vanishes, or
sits somewhere else entirely.

**You can carry one object across the swap.** Pick up a key in his
memory, swap, and the key now exists in hers — where the lock is. That
single rule is the whole puzzle language.

Rules that keep it honest:

- You swap freely, any time, no cooldown. Swapping is thinking, not a
  resource to ration.
- If you swap while standing where the other memory has a wall, you are
  pushed back and the swap fails. Space has to agree with you.
- You carry one object. Never two.
- Some objects exist in both memories. Those are the things they agree
  on, and the game should use that quietly — the objects both of them
  remember are the ones that mattered.

---

## The story

A mother and her son are clearing out the house after a death. You move
through it as memory rather than as a place.

Each room is a disagreement. Most are small and human: she remembers the
kitchen sunny, he remembers it dark. A photograph on her wall is face-down
on his shelf.

In one room the contradiction is the point. One of them is remembering
something that did not happen, and the puzzle only solves once you work
out which — because the solution requires trusting one version and
refusing the other.

**The mechanic is the theme.** The game is about two people who lived
the same life and came away with different pasts. You are not reading
that in a note; you are walking through it.

Told through the house, not through cutscenes:

- what each of them keeps and what each of them throws away
- what is missing from one memory entirely
- rooms that get harder to reconcile the closer you get to the truth

No voice acting needed. If there is dialogue at all, it is two people
talking past each other in another room while you solve something.

---

## Why it works as a portfolio piece

- **Small enough to finish.** One house, six to eight rooms. Finished
  beats ambitious.
- **No combat, no enemies, no AI, no character animation.** Every hard
  and expensive system in games is absent.
- **It demos in a GIF.** A room visibly folding into a different room is
  legible in three seconds with no explanation.
- **It reads as design, not just code.** The mechanic carries a theme.
  That is the thing that makes a reviewer think *designer*.

---

## Scope

**In:**

- One house, 6–8 rooms
- Two memory states per room
- Carry-one-object rule
- One "somebody is lying" room as the climax
- Ambient sound, no voice acting

**Out (say no now, not later):**

- More than two memories
- Any third character
- Inventory beyond one slot
- Saving and loading mid-run — it is an hour long, let it be one sitting
- Multiple endings

---

## Risks

| Risk | Why it bites | What to do |
|---|---|---|
| Puzzle design is hard | Bad puzzles read as "student project" louder than bad art | Build one room end to end and make someone else play it before building room two |
| Two versions of every room = double the art | Art is the slow part in 3D | Same walls, same shell, change what is IN the room. Not two models — one model, two dressings |
| Player gets lost between states | Disorienting instead of clever | Keep a constant anchor in every room — one object that never moves between memories |

---

## First playable

The smallest thing that proves the idea. Nothing else until this works:

1. One room, two versions, swap on keypress
2. The push-back rule when a wall is in the way
3. Pick up one object and carry it across the swap
4. One lock that only opens because the key came from the other memory

If that is fun for sixty seconds, the game works. If it is not, no
amount of house fixes it.

---

## Open questions

- Is the swap instant, or does it have a short transition? A transition
  is prettier and also hides loading.
- Do you see a ghost of the other memory, faintly, before swapping? It
  makes puzzles readable but might spoil the surprise of each room.
- First person or third? First person is cheaper — no player model, no
  animation.
