using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "LightingPresetSO", menuName = "Scriptable Objects/LightingPresetSO")]
public class LightingPresetSO : ScriptableObject
{
    [Header("Skybox")]
    public Material skyboxMaterial;

    [Header("Directional Light")]
    public Color lightColor = Color.white;
    public float lightIntensity = 1f;
    public Vector3 lightRotation;

    [Header("Post Processing")]
    public VolumeProfile volumeProfile;

    [Header("Text Material")]
    public Material textMaterial;
}
