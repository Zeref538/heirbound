# Heirbound — working notes

Unity 6 (6000.5.10f1), URP 2D, top-down. Project lives in `HeirboundGame/`.
Design is in `DESIGN.md`; the out-of-scope list there is a contract.

Project-only rules; the global ones in `~/.claude/CLAUDE.md` also apply.
Status lives in [TODO.md](TODO.md).

## Do not open the editor to check something

Everything below runs from the terminal. Unity locks a project to one
instance, so **if the editor is open, every command here fails** with
"another Unity instance is running". That error means exactly what it says.

```bash
U="C:/Program Files/Unity/Hub/Editor/6000.5.10f1/Editor/Unity.exe"
P="$PWD/HeirboundGame"

# compile only - exit 0 means the C# is clean
"$U" -batchmode -quit -nographics -projectPath "$P" -logFile /tmp/u.log
grep -E "error CS[0-9]+" /tmp/u.log

# regenerate clips + animators from the sprite folders
"$U" -batchmode -quit -nographics -projectPath "$P" \
     -executeMethod BuildAnimations.Build -logFile /tmp/u.log

# rebuild the whole test scene from scratch
"$U" -batchmode -quit -nographics -projectPath "$P" \
     -executeMethod BuildScene.Build -logFile /tmp/u.log

# render a PNG. NOTE: no -nographics, drawing needs a graphics device
"$U" -batchmode -quit -projectPath "$P" \
     -executeMethod Screenshot.Capture -out shot.png -logFile /tmp/u.log
```

Errors never print to stdout. Always `grep` the log file.

## The screenshot tool has a known blind spot

`Screenshot.Capture` renders props and floor correctly, but **characters and
point lights did not appear** in headless captures as of 2026-09-20.
Disabling animators changed nothing; sorting order, positions, sprite
assignment and scale were all verified correct in the scene data.

Ruled out, each by a separate render, none of which changed the picture:

- the render path (switched `cam.Render()` to a URP `SingleCameraRequest`;
  the log confirms "rendered through URP" and the image is the same)
- lighting (global light forced to 4.0 — floor goes bright, cast still absent)
- sorting order (floor at -100 and at 0; other renderers forced to 999)
- depth (floor moved to z = 1, characters left at z = 0)
- animators (disabled — pixel-identical output)
- renderer state (colour, enabled, active, layer and scale all logged correct)

The one repeatable signal: **with the floor hidden, props render; with the
floor present, nothing but floor renders.** Characters never appeared in any
configuration. No theory tested so far explains that, so do not trust a
guess about it.

Treat a headless render as evidence about the floor and props at their
default order only. For anything involving characters or lighting, the
editor's Play mode is the source of truth. Do not report a character bug
from a headless screenshot alone.

## Art pipeline

Raw generator output goes in `source-art/`, never into Unity directly.
`tools/slice_sheet.py` cuts it up into `HeirboundGame/Assets/Sprites/`.

```bash
python tools/slice_sheet.py source-art/player/idle_side.png 6 -o HeirboundGame/Assets/Sprites/player/idle
python tools/slice_sheet.py source-art/tiles/stone_floor.png --grid 4x4 -o HeirboundGame/Assets/Sprites/tiles
python tools/test_slice_sheet.py     # six asserts, run after touching the slicer
```

Two rules the slicer encodes, and they are opposites:

- **Animation frames share one box.** The offset between frames *is* the
  movement. Trim each frame to its own art and the character jitters.
- **Tiles are trimmed individually, then centred.** Each tile is placed by
  the game, so keeping their relative offsets makes a floor jitter.

Strips are cut at the **least-ink column** near each boundary, not at equal
widths — generators ignore "equal cells", and a long sword leans into the
next frame, so often there is no empty gutter at all.

`tools/make_seamless.py` exists because the generator paints a dark border
round every floor tile. Laid on a grid those borders line up into a visible
lattice. It crops the rim and wrap-blends the edges. No Unity setting fixes
this — the lines are in the art.

## Import settings are code, not clicks

`Assets/Editor/SpriteImportSettings.cs` applies settings on import. Never set
them by hand in the Inspector; they will drift and a re-slice reverts them.

- Characters: 256 pixels per unit (player ≈ 1.7 units ≈ human height)
- Tiles: 242 — deliberately *under* the real size, so tiles overlap by a hair
  instead of leaving hairline gaps that read as a lattice
- Floor: 153, wrap mode Repeat
- **Bilinear, not Point.** This art is painted at ~440px and shown small.
  Point filtering is for art drawn at its final pixel size.

## Gotchas already paid for

- **`Light2D` must be created with `ObjectFactory.AddComponent`**, not
  `AddComponent`. A plain AddComponent leaves its provider null.
- **Sprites use `Sprite-Lit-Default`.** A scene with no `Light2D` renders
  dark sprites as solid black. Bright props survive it; a charcoal coat does
  not. Lighting is load-bearing here, not decoration.
- **`spriteAlignment` is on `TextureImporterSettings`**, not on
  `TextureImporter`. Read it out, change it, write it back.
- **Do not write C# through a bash heredoc.** `'\\'` arrives as `'\'` and
  the compiler says "too many characters in character literal". Use the
  Write/Edit tools for any file containing backslashes.

## Generating art

Prompts live in `docs/PROMPTS-STAGE1.md`, numbered in the order they must be
generated. Every sheet after the first attaches an earlier one as a scale
reference.

Observed failure modes, all of which have happened:

- **Scale drift.** Sheets came back at half size. The prompt now says to
  measure the reference head-to-boot and match it exactly.
- **Invented magic effects.** Purple energy trails appeared on every action
  pose unasked. The prompt now bans glow, aura, particles and colour trails.
- **Dropped props.** Both swords vanished on the rush and run sheets. The
  prompt now demands both blades visible in every frame.

Check a new sheet before keeping it: one row, nothing crossing cells, same
size as its reference, no drawn ground or shadow.
