using UnityEngine;

/// <summary>
/// Animates the sun (Directional Light) through a full day/night cycle.
/// Attach to the DirectionalLight. Assign skyboxMaterial in the Inspector
/// to swap the scene skybox on Play.
/// </summary>
[RequireComponent(typeof(Light))]
public class DayNightCycle : MonoBehaviour
{
    [Header("Time")]
    [Tooltip("Real-time seconds for one full day/night cycle.")]
    public float dayDurationSeconds = 120f;

    [Tooltip("0 = midnight, 0.25 = sunrise, 0.5 = noon, 0.75 = sunset")]
    [Range(0f, 1f)]
    public float timeOfDay = 0.25f;

    [Header("Skybox")]
    [Tooltip("FarlandSkies material to use as scene skybox.")]
    public Material skyboxMaterial;

    // ----------------------------------------------------------------
    //  Sun color gradient keyframes  (timeOfDay → Color)
    // ----------------------------------------------------------------
    private static readonly (float t, Color c)[] SunColors =
    {
        (0.00f, new Color(0.05f, 0.05f, 0.20f)),  // midnight  – dark blue
        (0.20f, new Color(1.00f, 0.45f, 0.15f)),  // sunrise   – deep orange
        (0.30f, new Color(1.00f, 0.90f, 0.75f)),  // morning   – warm white
        (0.50f, new Color(1.00f, 1.00f, 1.00f)),  // noon      – pure white
        (0.70f, new Color(1.00f, 0.90f, 0.70f)),  // afternoon – warm white
        (0.80f, new Color(1.00f, 0.40f, 0.10f)),  // sunset    – orange-red
        (0.90f, new Color(0.25f, 0.10f, 0.25f)),  // dusk      – purple-dark
        (1.00f, new Color(0.05f, 0.05f, 0.20f)),  // midnight  – dark blue (loop)
    };

    // ----------------------------------------------------------------
    //  Sun intensity keyframes  (timeOfDay → intensity)
    // ----------------------------------------------------------------
    private static readonly (float t, float v)[] Intensities =
    {
        (0.00f, 0.02f),  // midnight
        (0.20f, 0.50f),  // sunrise
        (0.30f, 0.90f),  // morning
        (0.50f, 1.20f),  // noon
        (0.70f, 0.90f),  // afternoon
        (0.80f, 0.50f),  // sunset
        (0.90f, 0.10f),  // dusk
        (1.00f, 0.02f),  // midnight
    };

    private Light _light;

    void Awake()
    {
        _light = GetComponent<Light>();
        ApplySkybox();
    }

    void Update()
    {
        timeOfDay = (timeOfDay + Time.deltaTime / dayDurationSeconds) % 1f;
        UpdateSun();
    }

    void UpdateSun()
    {
        // Rotate sun: at timeOfDay=0.25 → X=0 (horizon/sunrise)
        //             at timeOfDay=0.50 → X=90 (zenith/noon)
        //             at timeOfDay=0.75 → X=180 (horizon/sunset)
        float sunAngle = (timeOfDay - 0.25f) * 360f;
        transform.rotation = Quaternion.Euler(sunAngle, -30f, 0f);

        _light.color     = SampleColorGradient(SunColors, timeOfDay);
        _light.intensity = SampleFloatCurve(Intensities, timeOfDay);
    }

    void ApplySkybox()
    {
        // Try serialized reference first
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
            return;
        }

        // Fallback: load from known path when running in the Editor
#if UNITY_EDITOR
        const string path = "Assets/FarlandSkies/Skyboxes/CloudyCrown_01_Midday/CloudyCrown_Midday.mat";
        var mat = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat != null)
        {
            skyboxMaterial = mat;
            RenderSettings.skybox = mat;
        }
        else
        {
            Debug.LogWarning("[DayNightCycle] Could not load skybox from: " + path);
        }
#endif
    }

    // ----------------------------------------------------------------
    //  Helpers
    // ----------------------------------------------------------------
    static Color SampleColorGradient((float t, Color c)[] keys, float time)
    {
        for (int i = 1; i < keys.Length; i++)
        {
            if (time <= keys[i].t)
            {
                float f = Mathf.InverseLerp(keys[i - 1].t, keys[i].t, time);
                return Color.Lerp(keys[i - 1].c, keys[i].c, f);
            }
        }
        return keys[keys.Length - 1].c;
    }

    static float SampleFloatCurve((float t, float v)[] keys, float time)
    {
        for (int i = 1; i < keys.Length; i++)
        {
            if (time <= keys[i].t)
            {
                float f = Mathf.InverseLerp(keys[i - 1].t, keys[i].t, time);
                return Mathf.Lerp(keys[i - 1].v, keys[i].v, f);
            }
        }
        return keys[keys.Length - 1].v;
    }
}
