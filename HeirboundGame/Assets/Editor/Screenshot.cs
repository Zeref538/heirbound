using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Renders the scene to a PNG without opening the editor window, so a look
/// can be checked from the command line:
///
///   Unity.exe -batchmode -quit -projectPath . \
///     -executeMethod Screenshot.Capture -out shot.png
///
/// Note there is no -nographics. Unity cannot draw anything without a
/// graphics device, and a screenshot is the one job that needs one.
/// </summary>
public static class Screenshot
{
    public static void Capture()
    {
        var path = ArgOr("-out", "shot.png");
        var scene = ArgOr("-scene", "Assets/Scenes/Combat.unity");
        var width = int.Parse(ArgOr("-width", "960"));
        var height = int.Parse(ArgOr("-height", "540"));

        EditorSceneManager.OpenScene(scene, OpenSceneMode.Single);

        var cam = UnityEngine.Object.FindFirstObjectByType<Camera>();
        if (cam == null)
        {
            Debug.LogError("Screenshot: no camera in " + scene);
            return;
        }

        // A RenderTexture is an off-screen canvas the GPU draws into. The
        // camera paints there instead of a window, then the pixels are read
        // back and written out as a normal PNG.
        var ortho = ArgOr("-ortho", "");
        if (ortho != "") cam.orthographicSize = float.Parse(ortho);

        var lightBoost = ArgOr("-light", "");
        if (lightBoost != "")
            foreach (var l in UnityEngine.Object.FindObjectsByType<Light2D>(
                         FindObjectsSortMode.None))
                if (l.lightType == Light2D.LightType.Global)
                    l.intensity = float.Parse(lightBoost);

        if (ArgOr("-forcevisible", "") != "")
            foreach (var r in UnityEngine.Object.FindObjectsByType<SpriteRenderer>(
                         FindObjectsSortMode.None))
                if (r.transform.parent == null || r.transform.parent.name != "Floor")
                {
                    Debug.Log($"Screenshot: force {r.name} colour={r.color} " +
                              $"enabled={r.enabled} active={r.gameObject.activeInHierarchy} " +
                              $"layer={LayerMask.LayerToName(r.gameObject.layer)} " +
                              $"scale={r.transform.lossyScale}");
                    r.color = Color.white;
                    r.sortingOrder = 999;
                }

        if (ArgOr("-noanim", "") != "")
            foreach (var an in UnityEngine.Object.FindObjectsByType<Animator>(
                         FindObjectsSortMode.None))
                an.enabled = false;

        if (ArgOr("-hidefloor", "") != "")
        {
            var floor = GameObject.Find("Floor");
            if (floor != null) floor.SetActive(false);
        }

        foreach (var r in UnityEngine.Object.FindObjectsByType<SpriteRenderer>(
                     FindObjectsSortMode.None))
            if (r.transform.parent == null || r.transform.parent.name != "Floor")
                Debug.Log($"Screenshot: renderer {r.name} sprite={r.sprite?.name} " +
                          $"order={r.sortingOrder} layer={r.sortingLayerID} " +
                          $"material={r.sharedMaterial?.name} vis={r.isVisible}");

        var rt = new RenderTexture(width, height, 24);
        cam.targetTexture = rt;

        // cam.Render() is the OLD built-in render path. This project runs URP,
        // where that call skips most of the pipeline's own passes - which is
        // why characters and point lights never appeared in these captures
        // while plain props did. A render request hands the job to URP
        // properly and everything goes through the 2D renderer.
        try
        {
            var request = new UniversalRenderPipeline.SingleCameraRequest { destination = rt };
            cam.SubmitRenderRequest(request);
            Debug.Log("Screenshot: rendered through URP");
        }
        catch (Exception e)
        {
            Debug.LogWarning("Screenshot: URP render request failed (" + e.Message +
                             "), falling back to the built-in path - expect " +
                             "missing sprites and no point lights.");
            cam.Render();
        }

        RenderTexture.active = rt;
        var shot = new Texture2D(width, height, TextureFormat.RGB24, false);
        shot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        shot.Apply();

        cam.targetTexture = null;
        RenderTexture.active = null;

        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)));
        File.WriteAllBytes(path, shot.EncodeToPNG());
        Debug.Log($"Screenshot: wrote {path} ({width}x{height})");
    }

    static string ArgOr(string flag, string fallback)
    {
        var args = Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length - 1; i++)
            if (args[i] == flag) return args[i + 1];
        return fallback;
    }
}
