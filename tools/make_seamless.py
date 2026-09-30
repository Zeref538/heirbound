"""Turn a tile that has a border painted on it into one that truly repeats.

The generator draws each floor tile as a framed slab: a dark rim all the way
round. Lay those on a grid and the rims line up into a lattice of dark lines
across the whole floor. No import setting fixes it, because the lines are
in the art.

Two steps, and the order matters:

1. Crop the rim away, leaving only the interior stonework.
2. Wrap-blend the edges, so the left edge continues into the right edge and
   the top into the bottom.

Step 2 is the real trick. Take the first few columns and cross-fade them
with the last few, then throw the last few away. The column that ends up on
the left is now blended to match the one that ends up on the right, so two
copies laid side by side meet with nothing to see.

Usage:
  python tools/make_seamless.py in.png -o out.png
  python tools/make_seamless.py in.png -o out.png --crop 0.12 --feather 0.18
"""

import argparse

import numpy as np
from PIL import Image


def crop_border(img, fraction):
    """Cut a fraction off every side, removing the painted-on rim."""
    w, h = img.size
    dx, dy = int(w * fraction), int(h * fraction)
    return img.crop((dx, dy, w - dx, h - dy))


def wrap_blend(a, feather):
    """Blend the start of each axis into its end so the image tiles.

    Works on one axis at a time by rotating the array, which keeps the maths
    in one place instead of writing it twice.
    """
    for axis in (0, 1):
        a = np.moveaxis(a, axis, 0)
        n = a.shape[0]
        f = max(1, min(int(n * feather), n // 2 - 1))

        head, tail = a[:f].astype(float), a[n - f:].astype(float)
        # 0 at the very edge means "use the tail here", rising to 1 means
        # "use the head". At column 0 the pixel becomes what used to sit
        # just before the discarded strip - which is its true neighbour.
        ramp = np.linspace(0.0, 1.0, f).reshape((f,) + (1,) * (a.ndim - 1))

        a = a[: n - f].copy()
        a[:f] = (head * ramp + tail * (1.0 - ramp)).astype(a.dtype)
        a = np.moveaxis(a, 0, axis)
    return a


def make_seamless(src, crop=0.12, feather=0.18):
    img = crop_border(Image.open(src).convert("RGBA"), crop)
    return Image.fromarray(wrap_blend(np.array(img), feather), "RGBA")


def seam_error(img):
    """How badly the edges fail to meet. 0 is perfect, under ~3 is invisible."""
    a = np.array(img.convert("RGB")).astype(float).mean(axis=2)
    return (
        float(np.abs(a[:, 0] - a[:, -1]).mean()),
        float(np.abs(a[0, :] - a[-1, :]).mean()),
    )


def main():
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("src")
    ap.add_argument("-o", "--out", required=True)
    ap.add_argument("--crop", type=float, default=0.12,
                    help="fraction cut off each side to lose the painted rim")
    ap.add_argument("--feather", type=float, default=0.18,
                    help="fraction of the image used for the wrap blend")
    args = ap.parse_args()

    before = seam_error(Image.open(args.src).convert("RGBA"))
    out = make_seamless(args.src, args.crop, args.feather)
    out.save(args.out)
    after = seam_error(out)

    print(f"{args.out}  {out.size[0]}x{out.size[1]}")
    print(f"  seam error left/right {before[0]:.1f} -> {after[0]:.1f}")
    print(f"  seam error top/bottom {before[1]:.1f} -> {after[1]:.1f}")


if __name__ == "__main__":
    main()
