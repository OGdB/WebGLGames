using UnityEngine;

public class AudioEffectPlayer : MonoBehaviour
{
    private static AudioSource[] audioSources;
    private static int lastAudioSource = 0;

    private void Awake()
    {
        audioSources = GetComponentsInChildren<AudioSource>();
    }

    public static void PlaySoundEffect(AudioClip clip, Vector3 worldPosition = default, float volume = 1f)
    {
        var currentSource = audioSources[lastAudioSource];
        currentSource.transform.position = worldPosition;
        currentSource.spatialBlend = worldPosition != default ? 1 : 0;
        currentSource.PlayOneShot(clip, volume);

        // Play next effect with next source.
        lastAudioSource++;
        lastAudioSource %= audioSources.Length;
    }
}
