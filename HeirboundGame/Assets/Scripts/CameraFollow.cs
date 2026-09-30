using UnityEngine;

/// <summary>
/// Keeps the camera on the player, a little behind where they actually are.
///
/// The lag is the point. A camera nailed rigidly to the player makes the
/// world feel like it is sliding around them; a camera that catches up makes
/// the player feel like they are moving through the world. It is the same
/// trick as a cameraman following a runner rather than riding on their back.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Tooltip("Seconds for the camera to catch up. Higher is looser.")]
    public float smoothing = 0.12f;

    [Tooltip("Nudges the view up so the player sits slightly low on screen.")]
    public float lookAhead = 0.4f;

    Vector3 velocity;

    void LateUpdate()
    {
        // LateUpdate, not Update: the player has already moved this frame, so
        // the camera follows the final position instead of last frame's, and
        // the picture does not judder.
        if (target == null) return;

        var wanted = new Vector3(
            target.position.x,
            target.position.y + lookAhead,
            transform.position.z);

        transform.position = Vector3.SmoothDamp(
            transform.position, wanted, ref velocity, smoothing);
    }
}
