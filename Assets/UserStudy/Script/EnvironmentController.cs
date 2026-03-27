using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class EnvironmentController : MonoBehaviour
{
    [SerializeField] private Light directionalLight;
    [SerializeField] private Volume globalVolume;

    public void ApplyPreset(LightingPresetSO preset)
    {
        // Skybox
        RenderSettings.skybox = preset.skyboxMaterial;

        // Directional Light
        directionalLight.color = preset.lightColor;
        directionalLight.intensity = preset.lightIntensity;
        directionalLight.transform.rotation = Quaternion.Euler(preset.lightRotation);

        // Volume Profile (Post Processing)
        globalVolume.profile = preset.volumeProfile;

        // Force update
        DynamicGI.UpdateEnvironment();
    }

    public void ApplyTextMaterial(LightingPresetSO preset, TextMeshPro text)
    {
        text.fontMaterial = preset.textMaterial;
    }
}
