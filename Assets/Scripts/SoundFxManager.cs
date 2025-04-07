using UnityEngine;
using UnityEngine.Serialization;

public class SoundFxManager : MonoBehaviour
{
    public static SoundFxManager Instance;

    public AudioSource soundFxObject;
    private AudioSource _musicSource;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Setup music source
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// Plays a sound effect at position of the transform object.
    /// </summary>
    public void PlaySound(AudioClip clip, Transform parent, float volume = 1f)
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
    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        AudioSource audioSource = Instantiate(soundFxObject, transform);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource.gameObject, clip.length);
    }

    /// <summary>
    /// Plays background music. Stops any currently playing track.
    /// </summary>
    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (_musicSource.isPlaying)
        {
            _musicSource.Stop();
        }

        _musicSource.clip = clip;
        _musicSource.volume = volume;
        _musicSource.Play();
    }

    /// <summary>
    /// Stops the currently playing music.
    /// </summary>
    public void StopMusic()
    {
        if (_musicSource.isPlaying)
        {
            _musicSource.Stop();
        }
    }
}