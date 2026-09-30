using UnityEditor;
using UnityEngine;

/// <summary>
/// Applies the same import settings to everything under Assets/Sprites,
/// automatically, the moment Unity sees the file.
///
/// An AssetPostprocessor is a script Unity runs during import. Doing this
/// here rather than by hand means seventy files cannot drift apart, and a
/// re-sliced sheet keeps its settings instead of silently reverting.
/// </summary>
public class SpriteImportSettings : AssetPostprocessor
{
    // How many pixels of art make one world unit. The player is roughly 440
    // pixels tall, so 256 puts him at about 1.7 units - near enough to a
    // human's height in metres, which keeps speeds and distances readable.
    const float CharacterPixelsPerUnit = 256f;

    // A floor tile's art is 242-244 pixels across (the sprite is 246 with a
    // pixel of slack). Set to the SMALLEST of those, so every tile comes out
    // very slightly over one unit and neighbours overlap by a hair.
    //
    // Deliberately not the exact size: a tile a fraction under one unit
    // leaves a hairline gap between tiles, and a grid of hairlines reads as
    // a visible lattice across the floor. A fraction of overlap is invisible.
    const float TilePixelsPerUnit = 242f;

    // The seamless floor in Assets/Sprites/floor is 155 pixels after its
    // painted-on border was cropped away. Two under that, so copies overlap
    // by a hair rather than leaving a hairline gap.
    const float FloorPixelsPerUnit = 153f;

    void OnPreprocessTexture()
    {
        // Unity always hands assetPath to us with forward slashes.
        if (!assetPath.Contains("Assets/Sprites/")) return;

        var importer = (TextureImporter)assetImporter;
        var isFloor = assetPath.Contains("/floor/");
        var isTile = isFloor || assetPath.Contains("/tiles/");

        importer.textureType = TextureImporterType.Sprite;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.maxTextureSize = 1024;

        // The pivot lives on a settings object, not on the importer itself,
        // so it has to be read out, changed, and written back.
        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);

        settings.spriteMode = (int)SpriteImportMode.Single;
        settings.spritePixelsPerUnit = isFloor ? FloorPixelsPerUnit
                                     : isTile  ? TilePixelsPerUnit
                                               : CharacterPixelsPerUnit;

        // The floor repeats, so its edges must not fade out. Clamp would
        // smear the last pixel; Repeat makes the texture wrap cleanly.
        if (isFloor) importer.wrapMode = TextureWrapMode.Repeat;

        // Bottom-centre pivot: the sprite's origin sits at its feet, so a
        // character stands on the ground instead of hovering, and a taller
        // pose grows upward rather than pushing the feet down.
        // Tiles pivot at their centre so they grid up cleanly.
        settings.spriteAlignment = (int)(isTile ? SpriteAlignment.Center
                                                : SpriteAlignment.BottomCenter);

        settings.alphaIsTransparency = true;
        settings.mipmapEnabled = false;

        // Bilinear, NOT point. This art is painted at high resolution and
        // shown small - point filtering would make every edge jagged. Point
        // is for art drawn at its final pixel size, which this is not.
        settings.filterMode = FilterMode.Bilinear;

        importer.SetTextureSettings(settings);
    }
}
