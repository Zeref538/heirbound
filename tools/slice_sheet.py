"""Cut a generated sprite sheet into numbered PNG frames Unity can import.

Two modes, because the art comes in two shapes:

  strip  - one row of N cells, one animation. Frames are trimmed together to
           a single shared box so the character does not jitter.
  grid   - R x C cells of separate objects (the tile sheet). Cells are kept
           at full size, because trimming a floor tile breaks its seamless
           edges.

Usage:
  python tools/slice_sheet.py source-art/player/idle_side.png 6 -o out/idle
  python tools/slice_sheet.py source-art/tiles/stone_floor.png --grid 4x4 -o out/tiles
"""

import argparse
import pathlib

from PIL import Image

# Pixels this faint are generation noise, not art. They are what makes a
# trimmed frame look like it has an invisible border, and what makes tiles
# look like they have gaps between them.
ALPHA_FLOOR = 8


def clean(img):
    """Force near-transparent pixels to fully transparent."""
    a = img.getchannel("A").point(lambda v: 0 if v < ALPHA_FLOOR else v)
    img.putalpha(a)
    return img


def cells(sheet, rows, cols):
    """Cut the sheet into equal rows x cols pieces, reading left to right."""
    w, h = sheet.width // cols, sheet.height // rows
    return [
        sheet.crop((c * w, r * h, (c + 1) * w, (r + 1) * h))
        for r in range(rows)
        for c in range(cols)
    ]


def find_cuts(sheet, n, window=0.4):
    """Where to cut a strip into n frames, without assuming even spacing.

    Generators do not respect "equal cells". Poses drift, and a long weapon
    from one frame leans into the next, so there is often no empty column to
    split on at all.

    So instead of cutting at a fixed width, look near where each boundary
    ought to be and take the column carrying the LEAST ink. An empty gutter
    wins outright; where frames touch, the thinnest point is a sword tip, and
    clipping a few pixels off a tip beats leaving a stray blade floating in
    the next frame.
    """
    import numpy as np

    ink = np.array(sheet.getchannel("A"), dtype=np.int32).sum(axis=0)
    step = sheet.width / n
    cuts = []
    for i in range(1, n):
        centre = int(i * step)
        half = max(1, int(step * window))
        lo, hi = max(1, centre - half), min(sheet.width - 1, centre + half)
        # Ties go to the column nearest the expected boundary, which keeps
        # frames evenly sized when a whole gutter is empty.
        span = ink[lo:hi]
        best = min(range(len(span)), key=lambda j: (span[j], abs(lo + j - centre)))
        cuts.append(lo + best)
    return [0] + cuts + [sheet.width]


def strip_cells(sheet, n):
    edges = find_cuts(sheet, n)
    return [
        drop_bleed(sheet.crop((edges[i], 0, edges[i + 1], sheet.height)))
        for i in range(n)
    ]


# A blob this much smaller than the main figure, sitting on a frame's side
# edge, is bleed from the neighbouring pose rather than art of its own.
BLEED_MAX_SHARE = 0.03


def drop_bleed(frame):
    """Erase leftovers of the neighbouring pose from a frame's side edges.

    A cut through a sword tip leaves a sliver of that sword in the next
    frame, which reads in game as a blade fragment floating beside the
    character. Anything touching the left or right edge and far smaller than
    the main figure is that.

    Deliberately ignores the top and bottom edges, and anything floating in
    the middle - the slime's flung droplets are real art and must survive.
    """
    import numpy as np
    from scipy import ndimage

    a = np.array(frame.getchannel("A"))
    labels, count = ndimage.label(a > 8)
    if count < 2:
        return frame

    sizes = ndimage.sum(a > 8, labels, range(1, count + 1))
    biggest = sizes.max()
    edge_labels = set(labels[:, 0]) | set(labels[:, -1])

    kill = [
        i + 1
        for i in range(count)
        if (i + 1) in edge_labels and sizes[i] < biggest * BLEED_MAX_SHARE
    ]
    if not kill:
        return frame

    a[np.isin(labels, kill)] = 0
    out = frame.copy()
    out.putalpha(Image.fromarray(a))
    return out


def union_box(frames):
    """The smallest box that contains the art in every frame.

    Cropping all frames to this one box is the trick that kills dead space
    without introducing jitter - trim each frame to its own content and the
    sprite bounces around as the pose changes.
    """
    boxes = [f.getbbox() for f in frames]
    boxes = [b for b in boxes if b]
    if not boxes:
        return None
    return (
        min(b[0] for b in boxes),
        min(b[1] for b in boxes),
        max(b[2] for b in boxes),
        max(b[3] for b in boxes),
    )


def square(box):
    """Grow a box to a square, centred on what it already covers.

    Floor tiles have to be exactly as wide as they are tall, or a grid of
    them drifts apart in one direction and overlaps in the other.
    """
    x0, y0, x1, y1 = box
    side = max(x1 - x0, y1 - y0)
    cx, cy = (x0 + x1) // 2, (y0 + y1) // 2
    return (cx - side // 2, cy - side // 2, cx - side // 2 + side, cy - side // 2 + side)


def slice_sheet(src, out_dir, rows=1, cols=None, trim=True, even=False):
    sheet = clean(Image.open(src).convert("RGBA"))
    if rows == 1 and not even:
        frames = strip_cells(sheet, cols)
        box = union_box(frames) if trim else None
        if box is None and trim:
            raise SystemExit("every frame is empty - wrong file, or alpha was flattened")
        if box:
            frames = [f.crop(box) for f in frames]
    else:
        frames = [clean(c) for c in cells(sheet, rows, cols)]
        # Tiles are NOT animation frames, and the difference matters.
        #
        # Frames of one animation share a single box, because the offset
        # between them IS the movement. Tiles are separate objects that each
        # get placed by the game, so each one is trimmed to its own art and
        # then centred. Keeping their relative offsets - which is what a
        # shared box does - is what makes a floor visibly jitter.
        #
        # Within a row they are padded to one common square, so the four
        # floor variants are interchangeable and a floor shows no seam.
        trimmed = []
        for r in range(rows):
            row = [f.crop(f.getbbox()) if f.getbbox() else f
                   for f in frames[r * cols:(r + 1) * cols]]
            side = max([max(f.size) for f in row] or [1])
            out = []
            for f in row:
                canvas = Image.new("RGBA", (side, side), (0, 0, 0, 0))
                canvas.paste(f, ((side - f.width) // 2, (side - f.height) // 2))
                out.append(canvas)
            trimmed += out
        frames = trimmed

    out_dir = pathlib.Path(out_dir)
    out_dir.mkdir(parents=True, exist_ok=True)
    written = []
    for i, f in enumerate(frames):
        # Zero-padded so frame_10 sorts after frame_02, not before it.
        p = out_dir / f"frame_{i:02d}.png"
        f.save(p)
        written.append(p)
    return written


def main():
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("sheet")
    ap.add_argument("count", nargs="?", type=int, help="cells in a strip")
    ap.add_argument("--grid", help="rows x cols, e.g. 4x4 (tile sheets)")
    ap.add_argument("-o", "--out", required=True)
    ap.add_argument("--even", action="store_true",
                    help="force equal-width cells instead of finding the gaps")
    args = ap.parse_args()

    if args.grid:
        rows, cols = (int(v) for v in args.grid.lower().split("x"))
        trim = False  # tiles must keep their full cell or seamless edges break
    elif args.count:
        rows, cols, trim = 1, args.count, True
    else:
        ap.error("give a frame count for a strip, or --grid RxC for a tile sheet")

    written = slice_sheet(args.sheet, args.out, rows, cols, trim, args.even)
    size = Image.open(written[0]).size
    print(f"{len(written)} frames -> {args.out}  ({size[0]}x{size[1]} each)")


if __name__ == "__main__":
    main()
