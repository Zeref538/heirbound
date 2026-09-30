"""Self-check for slice_sheet.py. Run it: python tools/test_slice_sheet.py

Builds fake sheets in memory, slices them, and asserts the things that
actually break animations: wrong frame count, frames of different sizes,
and lost alignment between frames.
"""

import pathlib
import sys
import tempfile

from PIL import Image

# So the test runs from anywhere, not just from inside tools/.
sys.path.insert(0, str(pathlib.Path(__file__).parent))
import slice_sheet as slice_sheet_module  # noqa: E402
from slice_sheet import ALPHA_FLOOR, slice_sheet  # noqa: E402


def strip(n, cell=64):
    """A strip of n cells, each with a small blob in a different spot.

    The blobs move around inside their cells on purpose - that is what a real
    animation does, and it is exactly what a naive per-frame trim destroys.
    """
    img = Image.new("RGBA", (cell * n, cell), (0, 0, 0, 0))
    for i in range(n):
        x = cell * i + 10 + (i % 3) * 4
        y = 12 + (i % 4) * 3
        img.paste((200, 30, 30, 255), (x, y, x + 20, y + 24))
    return img


def run(img, **kw):
    with tempfile.TemporaryDirectory() as d:
        src = f"{d}/sheet.png"
        img.save(src)
        # .copy() forces the pixels into memory and closes the file. Without
        # it Pillow reads lazily, Windows sees the PNG still open, and the
        # temp dir refuses to delete (WinError 32).
        return [Image.open(p).copy() for p in slice_sheet(src, f"{d}/out", **kw)]


def test_strip_count_and_uniform_size():
    frames = run(strip(6), rows=1, cols=6)
    assert len(frames) == 6, len(frames)
    sizes = {f.size for f in frames}
    assert len(sizes) == 1, f"frames differ in size: {sizes}"


def test_alignment_survives_trim():
    """Blobs sit at different offsets; their spacing must be unchanged."""
    n = 6
    frames = run(strip(n), rows=1, cols=n)
    before = [(10 + (i % 3) * 4, 12 + (i % 4) * 3) for i in range(n)]
    after = [f.getbbox()[:2] for f in frames]
    shift = (before[0][0] - after[0][0], before[0][1] - after[0][1])
    for i, (b, a) in enumerate(zip(before, after)):
        assert (b[0] - a[0], b[1] - a[1]) == shift, f"frame {i} drifted"


def test_faint_pixels_are_erased():
    img = Image.new("RGBA", (64, 64), (0, 0, 0, 0))
    img.paste((9, 9, 9, ALPHA_FLOOR - 1), (0, 0, 64, 2))  # noise row
    img.paste((255, 255, 255, 255), (20, 20, 40, 40))     # real art
    f = run(img, rows=1, cols=1)[0]
    assert f.size == (20, 20), f"noise row survived the trim: {f.size}"


def test_grid_rows_share_one_square_box():
    """Tiles in a row must come out identical and square, or a floor seams."""
    img = Image.new("RGBA", (256, 256), (0, 0, 0, 0))
    # Row 0: four tiles of slightly different sizes, each offset differently -
    # exactly how a generator draws them.
    for c, (pad, size) in enumerate([(4, 52), (9, 48), (6, 50), (11, 46)]):
        x, y = c * 64 + pad, pad
        img.paste((80, 80, 90, 255), (x, y, x + size, y + size))

    frames = run(img, rows=4, cols=4, trim=False)
    assert len(frames) == 16, len(frames)

    row0 = {f.size for f in frames[:4]}
    assert len(row0) == 1, f"floor tiles differ in size: {row0}"
    w, h = row0.pop()
    assert w == h, f"tile is not square: {w}x{h}"
    assert w < 64, f"padding survived: {w} should be tighter than the 64px cell"

    # Every tile must be centred in its square, or variants jitter against
    # each other when laid side by side.
    for i, f in enumerate(frames[:4]):
        b = f.getbbox()
        assert abs(b[0] - (f.width - b[2])) <= 1, f"tile {i} is off-centre: {b}"


def blobs(frame):
    import numpy as np
    from scipy import ndimage
    return ndimage.label(np.array(frame.getchannel("A")) > 8)[1]


def test_edge_bleed_dropped_but_droplets_kept():
    """A sliver on the frame edge goes; a small blob in the middle stays."""
    f = Image.new("RGBA", (64, 64), (0, 0, 0, 0))
    f.paste((255, 255, 255, 255), (20, 10, 50, 55))   # the figure
    f.paste((255, 255, 255, 255), (0, 30, 3, 34))     # bleed, on the left edge
    f.paste((255, 255, 255, 255), (12, 5, 16, 9))     # droplet, floating
    assert blobs(f) == 3

    out = slice_sheet_module.drop_bleed(f)
    assert blobs(out) == 2, "expected the edge sliver gone and the droplet kept"


def test_bleed_removal_spares_a_big_figure_on_the_edge():
    """A pose that genuinely runs off the edge is not bleed - keep it."""
    f = Image.new("RGBA", (64, 64), (0, 0, 0, 0))
    f.paste((255, 255, 255, 255), (30, 10, 60, 55))   # figure
    f.paste((255, 255, 255, 255), (0, 10, 20, 55))    # big, touching the edge
    assert blobs(slice_sheet_module.drop_bleed(f)) == 2


if __name__ == "__main__":
    for name, fn in sorted(globals().items()):
        if name.startswith("test_"):
            fn()
            print("ok:", name)
    print("all passed")
