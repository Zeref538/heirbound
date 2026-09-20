# Art prompts

For generating sprite sheets with an image AI, then slicing them.

Top-down needs **three** directions per animation — down (toward camera),
up (away), and side. The side sheet is mirrored in Unity for the other
direction, so you never draw a fourth.

Five animations x three directions = **15 sheets per character.** That is
the real cost of top-down, and it is why the enemy roster is capped at four
types plus two bosses.

---

## Shared rules — paste at the top of every prompt

```
TECHNICAL RULES:
- ONE horizontal strip, a single row, no grid, no second row.
- Divide the canvas into N equal square cells. One pose per cell, fully
  inside its cell, at least 20 pixels of empty space on all sides.
- No pose may touch, overlap or cross into a neighbouring cell.
- Fully transparent background (PNG with alpha). No background colour,
  no ground, no cast shadow, no border, no text, no frame numbers.
- TOP-DOWN view, camera looking down at roughly 60 degrees, the same
  angle in every frame.
- The character must be the SAME SIZE in every frame and in every sheet.
- The character stays centred in its cell. Do not move it across the
  strip - movement comes from the game, not the art.
```

The last two rules matter most. Sheets generated separately drift in scale,
and art that travels across the canvas fights the engine. Both of those
cost real rework the first time round on Shadow.

Attach a reference image of the character to **every** prompt after the
first, so size and style stay locked.

---

## The player — one set per direction

Replace `<DIRECTION>` with one of:

- `facing toward the viewer, seen from above and slightly behind the head`
- `facing away from the viewer, seen from above, back of the head visible`
- `facing to the right in profile, seen from above`

**Idle — 6 frames**
```
[shared rules] N = 6. Canvas 3072 x 512, six 512x512 cells.
CHARACTER: <describe the character>
VIEW: <DIRECTION>
ANIMATION: a gentle idle breathing loop. Small weight shift, cloak or hair
settling. The first and last frames should flow into each other so the loop
does not jump.
```

**Walk — 8 frames**
```
[shared rules] N = 8. Canvas 4096 x 512, eight 512x512 cells.
CHARACTER: <same description>
VIEW: <DIRECTION>
ANIMATION: one full walk cycle - contact, down, pass, up, then the same on
the other leg. The character walks in place, centred in every cell.
```

**Attack — 5 frames**
```
[shared rules] N = 5. Canvas 2560 x 512, five 512x512 cells.
CHARACTER: <same description>
VIEW: <DIRECTION>
ANIMATION: a single weapon swing. 1 wind-up, 2 committed, 3 peak of the
swing with a motion arc, 4 follow-through, 5 returning to stance.
```

**Hurt — 3 frames**
```
[shared rules] N = 3. Canvas 1536 x 512, three 512x512 cells.
CHARACTER: <same description>
VIEW: <DIRECTION>
ANIMATION: a damage flinch. 1 recoiling, 2 deepest recoil, 3 recovering.
```

**Death — 6 frames**
```
[shared rules] N = 6. Canvas 3072 x 512, six 512x512 cells.
CHARACTER: <same description>
VIEW: <DIRECTION>
ANIMATION: collapsing to the ground and going still. The last frame is the
body at rest and is held.
```

---

## Tiles and rooms

```
[shared rules, but a 4x4 GRID instead of a strip]
Canvas 1024 x 1024, sixteen 256x256 cells, clear transparent gaps between
pieces so each can be cut out separately.
TOP-DOWN dungeon tiles, <theme>.
Row 1: floor plain, floor cracked, floor with rubble, floor with a drain
Row 2: wall top edge, wall left edge, wall inner corner, wall outer corner
Row 3: pillar, closed door, open doorway, staircase down
Row 4: crate, barrel, brazier with flame, ancestor statue on a plinth
```

Generate one sheet per dungeon theme. Keeping the same 16-cell layout across
themes means reskinning a whole floor is one folder swap.

---

## After generating

Do not slice by hand. The same tooling used on Shadow cuts these
automatically: it drops the faint artefact rows that make tiles look like
they have gaps, trims each piece to its real content, normalises scale
across sheets, and pads the filenames so `frame10` does not sort before
`frame2`.

Hand the file over and say what it is.
