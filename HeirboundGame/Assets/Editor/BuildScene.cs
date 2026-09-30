using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Builds the stage 1 test scene from scratch: a floor, the player, three
/// slimes, and a camera set up for top-down sorting.
///
/// This exists so the scene is reproducible. Hand-built scenes drift, and
/// "it worked yesterday" becomes unanswerable. Delete the scene, run this,
/// get the same thing back.
/// </summary>
public static class BuildScene
{
    const string ScenePath = "Assets/Scenes/Combat.unity";
    const string EnemyLayer = "Enemy";

    [MenuItem("Heirbound/Build Combat Scene")]
    public static void Build()
    {
        var enemyLayer = EnsureLayer(EnemyLayer);

        var scene = EditorSceneManager.NewScene(
            NewSceneSetup.EmptyScene, NewSceneMode.Single);

        MakeCamera();
        MakeLighting();
        MakeFloor(20, 14);

        var player = MakeCharacter("Player", "Player", Vector2.zero, 0.5f, 0.35f);
        MakeTorch(player.transform);

        var follow = Camera.main.gameObject.AddComponent<CameraFollow>();
        follow.target = player.transform;
        player.AddComponent<PlayerMovement>();
        var combat = player.AddComponent<PlayerCombat>();
        combat.hits = 1 << enemyLayer;          // the sword only sees enemies
        player.AddComponent<Health>().maxHealth = 12;

        for (var i = 0; i < 3; i++)
        {
            var slime = MakeCharacter("Slime", "Slime",
                new Vector2(3f + i * 1.6f, -1.5f + i * 1.4f), 0.55f, 0.3f);
            slime.layer = enemyLayer;
            slime.AddComponent<Health>().maxHealth = 6;
            slime.AddComponent<EnemyChase>();
        }

        Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(scene, ScenePath);
        Debug.Log("Heirbound: built " + ScenePath);
    }

    static void MakeCamera()
    {
        var go = new GameObject("Main Camera");
        go.tag = "MainCamera";
        var cam = go.AddComponent<Camera>();
        cam.orthographic = true;
        // 3.6 shows about 7 units top to bottom, so the 1.7-unit player fills
        // roughly a quarter of the screen. Pulled in from 5 because at that
        // distance a dark character on dark stone was unreadable.
        cam.orthographicSize = 3.6f;
        cam.backgroundColor = new Color(0.06f, 0.06f, 0.08f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.transform.position = new Vector3(0, 0, -10);

        // Top-down games need whatever is lower on screen drawn in front,
        // or the player walks "through" a slime that should be hiding him.
        // Sorting along the Y axis does that for free.
        cam.transparencySortMode = TransparencySortMode.CustomAxis;
        cam.transparencySortAxis = new Vector3(0, 1, 0);
    }

    /// <summary>
    /// A dim cold ambient light over everything.
    ///
    /// Sprites in this project use Sprite-Lit-Default, which means a scene
    /// with no Light2D in it renders them black. Bright props survive that;
    /// a character in a charcoal coat disappears completely. So lighting is
    /// not decoration here, it is the difference between seeing the player
    /// and not.
    ///
    /// Kept deliberately dim and slightly blue. Everything readable comes
    /// from the warm torch on the player, which is what makes a dungeon feel
    /// like a dungeon rather than a lit room with stone wallpaper.
    /// </summary>
    static void MakeLighting()
    {
        var go = new GameObject("Global Light");
        // ObjectFactory, not AddComponent: it runs the editor-side setup a
        // Light2D needs. A plain AddComponent leaves its internal provider
        // null, and a point light with a null provider renders nothing at
        // all while a global light still works - which is a confusing way
        // to find out.
        var light = ObjectFactory.AddComponent<Light2D>(go);
        light.lightType = Light2D.LightType.Global;
        light.color = new Color(0.66f, 0.72f, 0.88f);

        // Tuned against a measured render, not by eye: at 0.42 the average
        // frame sat at brightness 31 and the charcoal coat was unreadable
        // against the stone. This keeps the mood dark while leaving the
        // character legible even if the torch is off.
        light.intensity = 0.95f;
    }

    /// <summary>The warm pool of light the player carries.</summary>
    static void MakeTorch(Transform parent)
    {
        var go = new GameObject("Torch");
        go.transform.SetParent(parent);
        // Slightly above the feet, so the pool is centred on the body rather
        // than on the point the sprite pivots from.
        go.transform.localPosition = new Vector3(0, 0.8f, 0);

        // ObjectFactory, not AddComponent: it runs the editor-side setup a
        // Light2D needs. A plain AddComponent leaves its internal provider
        // null, and a point light with a null provider renders nothing at
        // all while a global light still works - which is a confusing way
        // to find out.
        var light = ObjectFactory.AddComponent<Light2D>(go);
        light.lightType = Light2D.LightType.Point;
        light.color = new Color(1f, 0.82f, 0.58f);
        light.intensity = 1.5f;
        light.pointLightInnerRadius = 1.4f;   // full brightness out to here
        light.pointLightOuterRadius = 6.5f;   // faded to nothing by here
        light.falloffIntensity = 0.7f;
        light.shadowIntensity = 0f;
    }

    /// <summary>A brazier's pool of firelight.</summary>
    static void MakeFireLight(Transform parent)
    {
        var go = new GameObject("Firelight");
        go.transform.SetParent(parent);
        go.transform.localPosition = new Vector3(0, 0.5f, 0);

        var light = ObjectFactory.AddComponent<Light2D>(go);
        light.lightType = Light2D.LightType.Point;
        light.color = new Color(1f, 0.64f, 0.34f);   // warmer than the torch
        light.intensity = 1.9f;
        light.pointLightInnerRadius = 0.6f;
        light.pointLightOuterRadius = 4.5f;
        light.falloffIntensity = 0.65f;
        light.shadowIntensity = 0f;
    }

    static void MakeFloor(int wide, int tall)
    {
        var floor = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/Sprites/floor/stone.png");

        if (floor == null)
        {
            Debug.LogWarning("Heirbound: Assets/Sprites/floor/stone.png missing. " +
                             "Run tools/make_seamless.py first.");
            return;
        }

        // One sprite, repeated. The generator's tiles each had a dark border
        // painted on, and a grid of those borders reads as a lattice across
        // the floor. This one had its border cropped off and its edges
        // wrap-blended, so copies meet with nothing to see.
        var parent = new GameObject("Floor").transform;
        for (var x = -wide / 2; x < wide / 2; x++)
        for (var y = -tall / 2; y < tall / 2; y++)
        {
            var go = new GameObject($"floor_{x}_{y}");
            go.transform.SetParent(parent);
            // z = 1 puts the floor a step further from the camera than
            // everything else. The 2D renderer has its depth buffer on, and
            // with every object sitting at z = 0 the floor draws first and
            // then wins the depth test against the characters standing on
            // it - so they silently never appear. Sorting order alone does
            // not save you from that; depth is a separate test.
            go.transform.position = new Vector3(x, y, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = floor;

            // Behind everything else. Belt and braces with the z = 1 above:
            // sorting order decides draw order, z decides depth, and the 2D
            // renderer has its depth buffer on.
            sr.sortingOrder = -100;
        }

        ScatterProps(wide, tall);
    }

    /// <summary>
    /// Sprinkles a few crates and barrels about. Variety now comes from
    /// objects sitting ON the floor rather than from swapping floor squares,
    /// because a swapped square brings its own border back with it.
    /// </summary>
    static void ScatterProps(int wide, int tall)
    {
        // Row 4 of the tile sheet: crate, barrel, brazier, statue.
        var props = Enumerable.Range(12, 4)
            .Select(i => AssetDatabase.LoadAssetAtPath<Sprite>(
                $"Assets/Sprites/tiles/frame_{i:00}.png"))
            .Where(s => s != null)
            .ToArray();

        if (props.Length == 0) return;

        var parent = new GameObject("Props").transform;
        var rng = new System.Random(7);     // fixed seed, same layout every run

        for (var i = 0; i < 8; i++)
        {
            var at = new Vector3(
                rng.Next(-wide / 2 + 1, wide / 2 - 1),
                rng.Next(-tall / 2 + 1, tall / 2 - 1), 0);

            // Keep the middle clear so props never spawn on top of the player.
            if (at.magnitude < 2.5f) continue;

            var pick = rng.Next(props.Length);
            var go = new GameObject("Prop");
            go.transform.SetParent(parent);
            go.transform.position = at;
            go.AddComponent<SpriteRenderer>().sprite = props[pick];

            // The brazier is index 2 of the row, and it has a lit flame drawn
            // on it. A drawn flame that casts no light looks painted on, so
            // it gets a real light - and those pools are what break up a big
            // dark floor into somewhere worth walking towards.
            if (pick == 2) MakeFireLight(go.transform);
        }
    }

    static GameObject MakeCharacter(string name, string animator,
                                    Vector2 at, float width, float height)
    {
        var go = new GameObject(name);
        go.transform.position = at;

        var sr = go.AddComponent<SpriteRenderer>();
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(
            $"Assets/Animation/{animator}/{animator}.controller");

        if (controller != null)
        {
            go.AddComponent<Animator>().runtimeAnimatorController = controller;
            // Show the first frame of idle in the editor, so the object is
            // not an invisible dot before you press Play.
            var idle = AssetDatabase.LoadAssetAtPath<AnimationClip>(
                $"Assets/Animation/{animator}/{animator}_Idle.anim");
            if (idle != null)
            {
                var binding = AnimationUtility.GetObjectReferenceCurveBindings(idle).FirstOrDefault();
                var keys = AnimationUtility.GetObjectReferenceCurve(idle, binding);
                if (keys != null && keys.Length > 0) sr.sprite = keys[0].value as Sprite;
            }
        }
        else
        {
            Debug.LogWarning($"Heirbound: no animator for {name}. " +
                             "Run Heirbound > Build Animations first.");
        }

        var body = go.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // The collider sits at the feet, not over the whole sprite. In a
        // top-down game the body is where the character touches the ground -
        // a collider covering the head would stop him walking past things
        // his feet clear easily.
        var col = go.AddComponent<CapsuleCollider2D>();
        col.direction = CapsuleDirection2D.Horizontal;
        col.size = new Vector2(width, height);
        col.offset = new Vector2(0, height / 2f);

        return go;
    }

    /// <summary>Adds a layer by name if it is not already there.</summary>
    static int EnsureLayer(string name)
    {
        var existing = LayerMask.NameToLayer(name);
        if (existing >= 0) return existing;

        var tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var layers = tagManager.FindProperty("layers");

        // 0-7 are Unity's own. User layers start at 8.
        for (var i = 8; i < layers.arraySize; i++)
        {
            var slot = layers.GetArrayElementAtIndex(i);
            if (!string.IsNullOrEmpty(slot.stringValue)) continue;
            slot.stringValue = name;
            tagManager.ApplyModifiedProperties();
            return i;
        }
        Debug.LogError("Heirbound: no empty layer slots left.");
        return 0;
    }
}
