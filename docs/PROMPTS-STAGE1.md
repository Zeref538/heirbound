# Stage 1 art — ten assets

Ten images, and nothing else until combat feels good.

Each block below is the same three things: **the filename** to save as, **what
it is for** in the game, and **the prompt** to paste.

Generate them **in the order they appear**. Each one attaches an earlier image
as a reference so the size and style stay locked — sheets generated cold drift
in scale, and rescaling afterwards is the rework `ART-PROMPTS.md` warns about.

Folders to make first:

```
source-art/player/
source-art/enemy/
source-art/tiles/
```

These hold the raw output. The sliced frames Unity actually uses land
somewhere else later, so the originals are never overwritten.

---

## 1. `source-art/player/idle_side.png`

**What it's for:** the player standing still. It is also **the reference image
for every other sheet in the project**, so this one is worth regenerating
until you like it. Everything else inherits its size and style.

```
TECHNICAL RULES:
- ONE horizontal strip, a single row, no grid, no second row.
- Divide the canvas into 6 equal square cells. One pose per cell, fully
  inside its cell, at least 20 pixels of empty space on all sides.
- No pose may touch, overlap or cross into a neighbouring cell.
- Fully transparent background (PNG with alpha). No background colour,
  no ground, no cast shadow, no border, no text, no frame numbers.
- TOP-DOWN view, camera looking down at roughly 60 degrees, the same
  angle in every frame.
- The character must be the SAME SIZE in every frame and in every sheet.
- The character stays centred in its cell. Do not move it across the
  strip - movement comes from the game, not the art.

Canvas 3072 x 512, six 512x512 cells.

CHARACTER: a young heir who fights with TWO swords, one in each hand.
Wears a long dark charcoal coat that reaches mid-calf, high stiff collar
turned up, the lower half split into two tails that move when he does.
Asymmetric fastening across the chest with dull brass buckles, one
leather pauldron and a bracer on the left arm only - inherited pieces
that do not match the coat. Dark trousers, scuffed boots, fingerless
gloves. Dark hair, no helmet, face mostly hidden by the downward camera
angle. The two swords are deliberately MISMATCHED, because they were
inherited from different people: the right hand holds a longer
straight-edged blade with a plain crossguard, the left a shorter
slightly curved blade with a ringed pommel. Muted, desaturated colours -
charcoal, steel grey, worn brown leather, one small accent of deep red
at the collar lining. Hand-painted pixel art, readable at small size,
dark fantasy. Grounded and worn, not glossy or anime-bright.

VIEW: facing to the right in profile, seen from above

ANIMATION: a gentle idle breathing loop. Small weight shift, the coat
settling. The first and last frames should flow into each other so the
loop does not jump.
```

---

## 2. `source-art/player/walk_side.png`

**What it's for:** plays whenever you are moving. This is the animation you
will stare at most while tuning move speed, so a bad walk makes good movement
feel wrong.

**Attach `idle_side.png`.**

```
TECHNICAL RULES:
- ONE horizontal strip, a single row, no grid, no second row.
- Divide the canvas into 8 equal square cells. One pose per cell, fully
  inside its cell, at least 20 pixels of empty space on all sides.
- No pose may touch, overlap or cross into a neighbouring cell.
- Fully transparent background (PNG with alpha). No background colour,
  no ground, no cast shadow, no border, no text, no frame numbers.
- TOP-DOWN view, camera looking down at roughly 60 degrees, the same
  angle in every frame.
- The character must be the SAME SIZE in every frame and in every sheet.
- The character stays centred in its cell. Do not move it across the
  strip - movement comes from the game, not the art.

Canvas 4096 x 512, eight 512x512 cells.

CHARACTER: same character as the attached reference image, identical
coat, both swords, colours and size.

VIEW: facing to the right in profile, seen from above

ANIMATION: one full walk cycle - contact, down, pass, up, then the same
on the other leg. The character walks in place, centred in every cell.
```

---

## 3. `source-art/player/attack_side.png`

**What it's for:** the attack. The single most important image in stage 1 —
"does hitting things feel good" is mostly decided by how frames 2 and 3 of
this sheet land. Frame 3, where the blades cross, is the moment the game
checks what you hit.

**Attach `idle_side.png`.**

```
TECHNICAL RULES:
- ONE horizontal strip, a single row, no grid, no second row.
- Divide the canvas into 5 equal square cells. One pose per cell, fully
  inside its cell, at least 20 pixels of empty space on all sides.
- No pose may touch, overlap or cross into a neighbouring cell.
- Fully transparent background (PNG with alpha). No background colour,
  no ground, no cast shadow, no border, no text, no frame numbers.
- TOP-DOWN view, camera looking down at roughly 60 degrees, the same
  angle in every frame.
- The character must be the SAME SIZE in every frame and in every sheet.
- The character stays centred in its cell. Do not move it across the
  strip - movement comes from the game, not the art.

Canvas 2560 x 512, five 512x512 cells.

CHARACTER: same character as the attached reference image, identical
coat, both swords, colours and size.

VIEW: facing to the right in profile, seen from above

ANIMATION: one dual-blade attack, both swords used. 1 wind-up with the
blades drawn back and apart, 2 committed, the right blade leading, 3 peak
of the cross - both blades at full extension crossing each other, with a
motion arc on each, 4 follow-through with the blades past each other and
the coat tails trailing, 5 returning to a low two-blade stance.
```

---

## 4. `source-art/player/hurt_side.png`

**What it's for:** plays for a fraction of a second when something hits you.
Short, but it is how the game tells you that you took damage without a number
on screen.

**Attach `idle_side.png`.**

```
TECHNICAL RULES:
- ONE horizontal strip, a single row, no grid, no second row.
- Divide the canvas into 3 equal square cells. One pose per cell, fully
  inside its cell, at least 20 pixels of empty space on all sides.
- No pose may touch, overlap or cross into a neighbouring cell.
- Fully transparent background (PNG with alpha). No background colour,
  no ground, no cast shadow, no border, no text, no frame numbers.
- TOP-DOWN view, camera looking down at roughly 60 degrees, the same
  angle in every frame.
- The character must be the SAME SIZE in every frame and in every sheet.
- The character stays centred in its cell. Do not move it across the
  strip - movement comes from the game, not the art.

Canvas 1536 x 512, three 512x512 cells.

CHARACTER: same character as the attached reference image, identical
coat, both swords, colours and size.

VIEW: facing to the right in profile, seen from above

ANIMATION: a damage flinch. 1 recoiling, 2 deepest recoil, 3 recovering.
```

---

## 5. `source-art/player/death_side.png`

**What it's for:** your character dying. In this game that is not a fail
screen, it is the thing the whole design is built on — later this is the
moment you pick an heirloom. The last frame holds, because the body stays on
the floor.

**Attach `idle_side.png`.**

```
TECHNICAL RULES:
- ONE horizontal strip, a single row, no grid, no second row.
- Divide the canvas into 6 equal square cells. One pose per cell, fully
  inside its cell, at least 20 pixels of empty space on all sides.
- No pose may touch, overlap or cross into a neighbouring cell.
- Fully transparent background (PNG with alpha). No background colour,
  no ground, no cast shadow, no border, no text, no frame numbers.
- TOP-DOWN view, camera looking down at roughly 60 degrees, the same
  angle in every frame.
- The character must be the SAME SIZE in every frame and in every sheet.
- The character stays centred in its cell. Do not move it across the
  strip - movement comes from the game, not the art.

Canvas 3072 x 512, six 512x512 cells.

CHARACTER: same character as the attached reference image, identical
coat, both swords, colours and size.

VIEW: facing to the right in profile, seen from above

ANIMATION: collapsing to the ground and going still. The last frame is
the body at rest and is held.
```

---

## 6. `source-art/enemy/idle_side.png`

**What it's for:** the one thing in the game you can hit. It is also the
slime's own reference sheet for the next three.

A slime instead of a person, on purpose: no legs means no walk cycle to get
wrong, and a blob is the same shape from every side — so unlike the player,
it never needs an up sheet or a down sheet. One set of art covers all eight
directions.

Note which image you attach here: **the player's idle**, and only to fix the
size. The prompt says outright to ignore the design and copy nothing but
scale. Without it the enemy comes out looking like it belongs in a different
game, and shrinking it afterwards mushes the pixels.

**Attach `source-art/player/idle_side.png`.**

```
TECHNICAL RULES:
- ONE horizontal strip, a single row, no grid, no second row.
- Divide the canvas into 6 equal square cells. One pose per cell, fully
  inside its cell, at least 20 pixels of empty space on all sides.
- No pose may touch, overlap or cross into a neighbouring cell.
- Fully transparent background (PNG with alpha). No background colour,
  no ground, no cast shadow, no border, no text, no frame numbers.
- TOP-DOWN view, camera looking down at roughly 60 degrees, the same
  angle in every frame.
- The character must be the SAME SIZE in every frame and in every sheet.
- The character stays centred in its cell. Do not move it across the
  strip - movement comes from the game, not the art.

Canvas 3072 x 512, six 512x512 cells.

CHARACTER: a small slime. A rounded blob of thick, semi-translucent
dark green ooze, wider than it is tall, with a heavy flattened base
where it rests on the ground. Two small simple dark eyes near the top,
no mouth, no limbs. A soft highlight on the upper left of the body so it
reads as wet. A few specks of grit and one small shard of old bone
suspended inside it - it has been eating whatever it finds down here.
About ONE THIRD the height of the character in the attached reference
image, which is the player - use that image only to judge scale, not
design. Same muted dark fantasy palette, hand-painted pixel art.

VIEW: facing to the right in profile, seen from above

ANIMATION: a soft idle wobble. The body squashes down and swells back
up like it is breathing, the surface jiggling a little late behind the
motion. Stays on the ground the whole time. The first and last frames
flow into each other so the loop does not jump.
```

---

## 7. `source-art/enemy/walk_side.png`

**What it's for:** the slime hopping toward you. Hopping is slower and more
readable than walking — you can see the wind-up and judge the gap, which is
what makes dodging past it feel like a decision instead of luck.

**Attach `source-art/enemy/idle_side.png`.**

```
TECHNICAL RULES:
- ONE horizontal strip, a single row, no grid, no second row.
- Divide the canvas into 8 equal square cells. One pose per cell, fully
  inside its cell, at least 20 pixels of empty space on all sides.
- No pose may touch, overlap or cross into a neighbouring cell.
- Fully transparent background (PNG with alpha). No background colour,
  no ground, no cast shadow, no border, no text, no frame numbers.
- TOP-DOWN view, camera looking down at roughly 60 degrees, the same
  angle in every frame.
- The character must be the SAME SIZE in every frame and in every sheet.
- The character stays centred in its cell. Do not move it across the
  strip - movement comes from the game, not the art.

Canvas 4096 x 512, eight 512x512 cells.

CHARACTER: same slime as the attached reference image, identical
shape, colour, eyes, internal specks and size.

VIEW: facing to the right in profile, seen from above

ANIMATION: one full hop cycle - squash down to gather, stretch upward
launching, airborne and rounded at the top, stretching downward falling,
squash flat on landing, then settling back to resting shape. Hops in
place, centred in every cell, and never leaves the bottom of the cell by
more than a third of its own height.
```

---

## 8. `source-art/enemy/hurt_side.png`

**What it's for:** the feedback that your blades connected. Three frames, and
they are most of the reason a hit reads as a hit rather than the slime
ignoring you.

There is no enemy *attack* sheet on purpose — in stage 1 the slime damages you
by touching you, so there is nothing to animate.

**Attach `source-art/enemy/idle_side.png`.**

```
TECHNICAL RULES:
- ONE horizontal strip, a single row, no grid, no second row.
- Divide the canvas into 3 equal square cells. One pose per cell, fully
  inside its cell, at least 20 pixels of empty space on all sides.
- No pose may touch, overlap or cross into a neighbouring cell.
- Fully transparent background (PNG with alpha). No background colour,
  no ground, no cast shadow, no border, no text, no frame numbers.
- TOP-DOWN view, camera looking down at roughly 60 degrees, the same
  angle in every frame.
- The character must be the SAME SIZE in every frame and in every sheet.
- The character stays centred in its cell. Do not move it across the
  strip - movement comes from the game, not the art.

Canvas 1536 x 512, three 512x512 cells.

CHARACTER: same slime as the attached reference image, identical
shape, colour, eyes, internal specks and size.

VIEW: facing to the right in profile, seen from above

ANIMATION: a damage flinch. 1 squashed hard sideways as if struck, 2
deepest deformation with the surface rippling, 3 wobbling back toward its
resting shape.
```

---

## 9. `source-art/enemy/death_side.png`

**What it's for:** the payoff. The last frame holds as a puddle on the ground
so the room remembers what you did in it.

**Attach `source-art/enemy/idle_side.png`.**

```
TECHNICAL RULES:
- ONE horizontal strip, a single row, no grid, no second row.
- Divide the canvas into 6 equal square cells. One pose per cell, fully
  inside its cell, at least 20 pixels of empty space on all sides.
- No pose may touch, overlap or cross into a neighbouring cell.
- Fully transparent background (PNG with alpha). No background colour,
  no ground, no cast shadow, no border, no text, no frame numbers.
- TOP-DOWN view, camera looking down at roughly 60 degrees, the same
  angle in every frame.
- The character must be the SAME SIZE in every frame and in every sheet.
- The character stays centred in its cell. Do not move it across the
  strip - movement comes from the game, not the art.

Canvas 3072 x 512, six 512x512 cells.

CHARACTER: same slime as the attached reference image, identical
shape, colour, eyes, internal specks and size.

VIEW: facing to the right in profile, seen from above

ANIMATION: bursting. 1 swelling, 2 over-inflated and tense, 3 splitting
open, 4 collapsing outward, 5 a spreading puddle, 6 a flat still puddle
with the bone shard left sitting in it. The last frame is held.
```

---

## 10. `source-art/tiles/stone_floor.png`

**What it's for:** the ground. On an empty grey background you genuinely
cannot tell whether your move speed is right, because nothing passes by to
give a sense of speed. A floor with some texture in it fixes that in one
image.

This one is a **4x4 grid, not a strip** — tiles are square and get laid
side by side, so they are drawn the way they are used.

Only row 1 is needed for stage 1. The other twelve cost nothing extra in the
same image and you will want them at stage 4 when rooms get built.

**Attach `source-art/player/idle_side.png`** so the stonework matches the
character's palette.

```
TECHNICAL RULES:
- A 4x4 GRID. Canvas 1024 x 1024, sixteen 256x256 cells, with clear
  transparent gaps between pieces so each can be cut out separately.
- One piece per cell, fully inside its cell. No piece may touch or cross
  into a neighbouring cell.
- Fully transparent background (PNG with alpha). No background colour,
  no border, no text, no labels.
- TOP-DOWN view, camera looking straight down.
- Every piece the same scale.
- The four floor tiles in row 1 must TILE SEAMLESSLY - the left edge
  continues the right edge, the top edge continues the bottom, so a grid
  of them shows no repeating seam.

TOP-DOWN dungeon tiles, cold grey stone, damp, lit by nothing in
particular. Muted dark fantasy, hand-painted pixel art, matching the
palette of the attached reference image.

Row 1: floor plain, floor cracked, floor with rubble, floor with a drain
Row 2: wall top edge, wall left edge, wall inner corner, wall outer corner
Row 3: pillar, closed door, open doorway, staircase down
Row 4: crate, barrel, brazier with flame, ancestor statue on a plinth
```

The seamless-tiling line is the one image models most often ignore. Check it
straight away: put four copies of the plain floor tile in a 2x2 square and
look for a visible cross where they meet. If it is there, regenerate row 1 on
its own.

---

---

# Movement extras

Space is currently the dodge. Pick ONE of these two, because they cannot
both live on the same key.

---

## 11. `source-art/player/rush_side.png`  — recommended

**What it's for:** the dash that Space already triggers in code. Five frames,
played once, no loop. It is a burst forward with brief invulnerability, so
the pose needs to read as *committed* — you cannot steer out of it, and the
art should say so.

Crucially this needs **no code change and no new button**. The animation
simply starts playing on the dash that already exists.

**Attach `idle_side.png`.**

```
TECHNICAL RULES:
- ONE horizontal strip, a single row, no grid, no second row.
- Divide the canvas into 5 equal square cells. One pose per cell, fully
  inside its cell, at least 20 pixels of empty space on all sides.
- No pose may touch, overlap or cross into a neighbouring cell.
- Fully transparent background (PNG with alpha). No background colour,
  no ground, no cast shadow, no border, no text, no frame numbers.
- TOP-DOWN view, camera looking down at roughly 60 degrees, the same
  angle in every frame.
- The character must be the SAME SIZE in every frame and in every sheet.
- The character stays centred in its cell. Do not move it across the
  strip - movement comes from the game, not the art.

Canvas 2560 x 512, five 512x512 cells.

CHARACTER: same character as the attached reference image, identical
coat, both swords, colours and size.

SIZE - this sheet failed on this before, so it is not optional: measure
the character in the attached reference from the top of the hair to the
bottom of the boot, and draw him EXACTLY that tall here. Not smaller
because the pose is low or leaning. The same body at the same size, just
in a different pose.

BOTH SWORDS MUST BE VISIBLE AND HELD, one in each hand, in every frame.
He never sheathes them and never drops them.

NO magic, NO glowing energy, NO purple, NO coloured trails, NO aura, NO
particles. The only effects allowed are faint grey motion smears behind a
moving blade or the coat hem. This is a grounded swordsman, not a mage.

VIEW: facing to the right in profile, seen from above

ANIMATION: a short forward dash. He stays UPRIGHT ON HIS FEET the whole
time, leaning forward maybe thirty degrees like a sprinter leaving the
blocks. He is NOT horizontal, NOT airborne, NOT flying, NOT diving. 1 a hard crouch low to the ground
gathering, both blades swept back behind him, 2 the launch - body driven
forward and almost horizontal, coat tails snapped straight out behind,
3 the fastest point, leaning forward with a faint grey speed smear behind
the trailing edge of the coat, 4 beginning to rise and slow, 5 settling
into a braced low stance, coat still moving. Feet stay near the ground
throughout and the body stays upright - he is committed and cannot turn.
```

---

## 12. `source-art/player/run_side.png`  — only if you want hold-to-run

**What it's for:** a faster movement loop that plays while a run key is held.
Eight frames, and it must loop as cleanly as the walk does.

**The cost, so it is not a surprise:** this needs a second key, because Space
is the dodge, and the dodge must keep its own button — it is your only
defence. It also means a third movement state in the animator, and later two
more sheets for the up and down views. The rush above costs none of that.

**Attach `walk_side.png`**, not the idle — a run is a walk pushed further,
and matching it keeps the two from looking like different characters.

```
TECHNICAL RULES:
- ONE horizontal strip, a single row, no grid, no second row.
- Divide the canvas into 8 equal square cells. One pose per cell, fully
  inside its cell, at least 20 pixels of empty space on all sides.
- No pose may touch, overlap or cross into a neighbouring cell.
- Fully transparent background (PNG with alpha). No background colour,
  no ground, no cast shadow, no border, no text, no frame numbers.
- TOP-DOWN view, camera looking down at roughly 60 degrees, the same
  angle in every frame.
- The character must be the SAME SIZE in every frame and in every sheet.
- The character stays centred in its cell. Do not move it across the
  strip - movement comes from the game, not the art.

Canvas 4096 x 512, eight 512x512 cells.

CHARACTER: same character as the attached reference image, identical
coat, both swords, colours and size.

SIZE - this sheet failed on this before, so it is not optional: measure
the character in the attached reference from the top of the hair to the
bottom of the boot, and draw him EXACTLY that tall here. Not smaller
because the pose is low or leaning. The same body at the same size, just
in a different pose.

BOTH SWORDS MUST BE VISIBLE AND HELD, one in each hand, in every frame.
He never sheathes them and never drops them.

NO magic, NO glowing energy, NO purple, NO coloured trails, NO aura, NO
particles. The only effects allowed are faint grey motion smears behind a
moving blade or the coat hem. This is a grounded swordsman, not a mage.

VIEW: facing to the right in profile, seen from above

ANIMATION: one full run cycle - contact, down, pass, up, then the same on
the other leg. Faster and heavier than a walk: the body leans forward,
the stride is longer, both feet leave the ground at the peak of each
pass, the arms drive harder and the coat tails stream backward instead of
hanging. Runs in place, centred in every cell.
```

---

## Before you keep any sheet

Throw it back and regenerate if any of these are true. All of them cost more
to fix later than to redo now:

- Two rows instead of one (tile sheet excepted — that one *is* a grid).
- A limb, a coat tail or either sword crossing into the next cell.
- Noticeably bigger or smaller than the reference image you attached.
- A drawn ground, shadow or border anywhere.
- The character sliding across the strip instead of staying centred.

## Not yet

The same five player sheets exist twice more, changing only the VIEW line:

- `facing toward the viewer, seen from above and slightly behind the head`
- `facing away from the viewer, seen from above, back of the head visible`

Those come after combat feels good. Left-facing is never drawn at all —
Unity mirrors the side sheet, which is the whole reason three directions
covers eight.
