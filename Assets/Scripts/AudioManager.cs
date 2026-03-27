using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip daytimeSource;
    [SerializeField] private AudioClip nighttimeSource;

    private AudioSource audioSource;

    public void ApplyAudioSettings(bool isDaytime)
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioManager: No AudioSource component found on the GameObject.");
                return;
            }
        }
        audioSource.clip = isDaytime ? daytimeSource : nighttimeSource;
        if (isDaytime)
        {
            audioSource.volume = 0.1f; // Set volume for daytime
        }
        else
        {
            audioSource.volume = 1f; // Set volume for nighttime
        }
        audioSource.loop = true;
        audioSource.Play();
    }
}
