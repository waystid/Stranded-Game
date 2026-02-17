using UnityEngine;
using MoreMountains.TopDownEngine;

/// <summary>
/// Animal Crossing: New Horizons-style fixed isometric follow camera.
/// Fixed 45-degree diagonal yaw, ~38-degree pitch downward. Smooth follows player.
/// Disable CharacterRotateCamera on the player when using this.
/// Enable RotateInputBasedOnCameraDirection on the InputManager; this camera
/// automatically registers itself so WASD maps to camera-relative directions.
/// </summary>
public class ACNHCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Transform to follow. Auto-finds 'Player' tag if null.")]
    public Transform Target;

    [Header("Camera Angle")]
    [Tooltip("World-space offset from target. Drives angle + distance.\n" +
             "Default tuned for 16x16 play space. Increase for larger worlds.")]
    public Vector3 Offset = new Vector3(-11f, 12f, -11f);

    [Header("Smoothing")]
    [Tooltip("Lower = snappier. Higher = smoother but laggier.")]
    [Range(0.01f, 0.5f)]
    public float SmoothTime = 0.12f;

    [Header("Rotation (Fixed)")]
    [Tooltip("X pitch in degrees. ~38 gives the ACNH look.")]
    [Range(20f, 60f)]
    public float PitchAngle = 38f;

    [Tooltip("Y yaw in degrees. 45 = NW diagonal view.")]
    [Range(0f, 360f)]
    public float YawAngle = 45f;

    [Header("Field of View")]
    [Range(30f, 90f)]
    public float FieldOfView = 55f;

    // ── private ──────────────────────────────────────────────────────────────
    private Vector3 _velocity = Vector3.zero;
    private Camera _cam;

    void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    void Start()
    {
        if (Target == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                Target = playerObj.transform;
            else
                Debug.LogWarning("[ACNHCameraFollow] No target assigned and no GameObject with 'Player' tag found.");
        }

        if (_cam != null)
            _cam.fieldOfView = FieldOfView;

        // Snap immediately on start so there's no initial lerp across the level
        if (Target != null)
            transform.position = Target.position + Offset;

        ApplyFixedRotation();

        // Register with the TDE InputManager so RotateInputBasedOnCameraDirection
        // uses this camera's fixed YawAngle for camera-relative WASD movement.
        var inputManager = FindFirstObjectByType<InputManager>();
        if (inputManager != null)
            inputManager.SetCamera(_cam, true);
    }

    void LateUpdate()
    {
        // Retry finding the player each frame until found (handles LevelManager late-spawn)
        if (Target == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                Target = playerObj.transform;
                _velocity = Vector3.zero;
                transform.position = Target.position + Offset;
                ApplyFixedRotation();
            }
            return;
        }

        Vector3 desiredPosition = Target.position + Offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref _velocity,
            SmoothTime);

        ApplyFixedRotation();
    }

    private void ApplyFixedRotation()
    {
        transform.rotation = Quaternion.Euler(PitchAngle, YawAngle, 0f);
    }

    /// <summary>
    /// Instantly snap camera to target with zero smoothing (use after teleport / scene load).
    /// </summary>
    public void SnapToTarget()
    {
        if (Target == null) return;
        _velocity = Vector3.zero;
        transform.position = Target.position + Offset;
        ApplyFixedRotation();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // Live-preview angle/FOV changes in Scene view
        ApplyFixedRotation();
        var cam = GetComponent<Camera>();
        if (cam != null)
            cam.fieldOfView = FieldOfView;
    }
#endif
}
