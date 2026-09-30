using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Turns the sliced sprite folders into animation clips and a working
/// animator, from a menu item. Run it again after re-slicing and it
/// overwrites cleanly.
///
/// Why generate instead of clicking: nine clips and their transitions is an
/// hour of dragging, and one wrong "Has Exit Time" tickbox produces a
/// character that plays its attack forever. Code gets it identical every
/// time, and the settings are readable here instead of buried in a window.
/// </summary>
public static class BuildAnimations
{
    const string SpriteRoot = "Assets/Sprites";
    const string OutRoot = "Assets/Animation";

    /// <summary>name, folder, frames per second, does it loop.</summary>
    struct Clip
    {
        public string Name, Folder;
        public float Fps;
        public bool Loop;
        public Clip(string n, string f, float fps, bool loop)
        { Name = n; Folder = f; Fps = fps; Loop = loop; }
    }

    static readonly Clip[] Player =
    {
        new Clip("Idle",   "player/idle",    8f,  true),
        new Clip("Walk",   "player/walk",   12f,  true),
        new Clip("Attack", "player/attack", 14f,  false),
        new Clip("Hurt",   "player/hurt",   14f,  false),
        new Clip("Death",  "player/death",   8f,  false),
    };

    static readonly Clip[] Slime =
    {
        new Clip("Idle",   "slime/idle",   8f,  true),
        new Clip("Walk",   "slime/hop",   10f,  true),
        new Clip("Hurt",   "slime/hurt",  14f,  false),
        new Clip("Death",  "slime/death",  9f,  false),
    };

    [MenuItem("Heirbound/Build Animations")]
    public static void Build()
    {
        Directory.CreateDirectory(OutRoot);
        BuildFor("Player", Player);
        BuildFor("Slime", Slime);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Heirbound: animators rebuilt in " + OutRoot);
    }

    static void BuildFor(string who, Clip[] clips)
    {
        var dir = $"{OutRoot}/{who}";
        Directory.CreateDirectory(dir);

        var made = new Dictionary<string, AnimationClip>();
        foreach (var c in clips)
        {
            var clip = MakeClip(c, $"{dir}/{who}_{c.Name}.anim");
            if (clip != null) made[c.Name] = clip;
        }
        if (made.Count == 0)
        {
            Debug.LogWarning($"Heirbound: no sprites found for {who}, skipped.");
            return;
        }
        MakeController($"{dir}/{who}.controller", made);
    }

    static AnimationClip MakeClip(Clip spec, string path)
    {
        var folder = $"{SpriteRoot}/{spec.Folder}";
        if (!Directory.Exists(folder))
        {
            Debug.LogWarning($"Heirbound: missing {folder}");
            return null;
        }

        // Directory.GetFiles returns Windows separators; AssetDatabase only
        // understands forward slashes, so the path has to be normalised.
        var sprites = Directory.GetFiles(folder, "*.png")
            .Select(p => p.Replace(System.IO.Path.DirectorySeparatorChar, '/'))
            .OrderBy(p => p)
            .Select(p => AssetDatabase.LoadAssetAtPath<Sprite>(p))
            .Where(s => s != null)
            .ToArray();

        if (sprites.Length == 0)
        {
            Debug.LogWarning($"Heirbound: no sprites imported yet in {folder}");
            return null;
        }

        var clip = new AnimationClip { frameRate = spec.Fps };

        // A sprite animation is one curve that swaps which Sprite the
        // SpriteRenderer is showing at each keyframe. Nothing else moves.
        var binding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite",
        };

        var keys = new ObjectReferenceKeyframe[sprites.Length];
        for (var i = 0; i < sprites.Length; i++)
            keys[i] = new ObjectReferenceKeyframe
            {
                time = i / spec.Fps,
                value = sprites[i],
            };

        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);

        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = spec.Loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        var existing = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        if (existing != null) EditorUtility.CopySerialized(clip, existing);
        else AssetDatabase.CreateAsset(clip, path);

        return AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
    }

    static void MakeController(string path, Dictionary<string, AnimationClip> clips)
    {
        AssetDatabase.DeleteAsset(path);
        var controller = AnimatorController.CreateAnimatorControllerAtPath(path);

        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("MoveX", AnimatorControllerParameterType.Float);
        controller.AddParameter("MoveY", AnimatorControllerParameterType.Float);
        foreach (var t in new[] { "Attack", "Hurt", "Death", "Dodge" })
            controller.AddParameter(t, AnimatorControllerParameterType.Trigger);

        var sm = controller.layers[0].stateMachine;
        var states = new Dictionary<string, AnimatorState>();
        foreach (var kv in clips)
        {
            var s = sm.AddState(kv.Key);
            s.motion = kv.Value;
            states[kv.Key] = s;
        }
        sm.defaultState = states["Idle"];

        // Idle <-> Walk, driven by how fast the thing is actually moving.
        if (states.ContainsKey("Walk"))
        {
            var toWalk = states["Idle"].AddTransition(states["Walk"]);
            toWalk.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
            toWalk.hasExitTime = false;
            toWalk.duration = 0.05f;

            var toIdle = states["Walk"].AddTransition(states["Idle"]);
            toIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");
            toIdle.hasExitTime = false;
            toIdle.duration = 0.05f;
        }

        // One-shots fire from Any State, so they interrupt whatever is
        // playing - getting hit mid-walk has to cut the walk off.
        foreach (var name in new[] { "Attack", "Hurt", "Death" })
        {
            if (!states.ContainsKey(name)) continue;

            var into = sm.AddAnyStateTransition(states[name]);
            into.AddCondition(AnimatorConditionMode.If, 0, name);
            into.hasExitTime = false;
            into.duration = 0f;
            // Without this, re-triggering restarts the clip from frame 0 and
            // the animation never visibly finishes.
            into.canTransitionToSelf = false;

            if (name == "Death") continue;   // death is the end, it stays put

            // hasExitTime = true means "play to the end, then leave", which
            // is exactly what a swing or a flinch needs.
            var back = states[name].AddTransition(states["Idle"]);
            back.hasExitTime = true;
            back.exitTime = 1f;
            back.duration = 0.05f;
        }

        EditorUtility.SetDirty(controller);
    }
}
