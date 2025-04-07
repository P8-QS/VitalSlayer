using UnityEngine;


public class SoundFxManager : MonoBehaviour
{
    public static SoundFxManager Instance;

    public AudioSource soundFxObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Plays a sound effect at position of the transform object.
    /// </summary>
    /// <param name="clip">Audio clip to play</param>
    /// <param name="parent">Transform object to play the sound at</param>
    /// <param name="volume">Volume of the sound in range of 0.0 to 1.0</param>
    public void PlaySound(AudioClip clip, Transform parent, float volume)
    {
        AudioSource audioSource = Instantiate(soundFxObject, parent);

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();

        Destroy(audioSource.gameObject, clip.length);
    }

    /// <summary>
    /// Plays a sound effect globally.
    /// </summary>
    /// <param name="clip">Audio clip to play</param>
    /// <param name="volume">Volume of the sound in range of 0.0 to 1.0</param>
    public void PlaySound(AudioClip clip, float volume)
    {
        AudioSource audioSource = Instantiate(soundFxObject, transform);

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();

        Destroy(audioSource.gameObject, clip.length);
    }
}