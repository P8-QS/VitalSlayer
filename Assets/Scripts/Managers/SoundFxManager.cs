using System.Collections;
using UnityEngine;

public class SoundFxManager : MonoBehaviour
{
    public static SoundFxManager Instance;

    public AudioSource soundFxObject;
    private AudioSource _musicSource;

    public AudioClip clickSound;


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
        if (clip == null)
        {
            Debug.LogWarning("SoundFxManager: Attempted to play a null clip.");
            return;
        }
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
    private Coroutine _crossfadeCoroutine;

    public void PlayMusic(AudioClip clip, float volume = 1f, bool continuePlayback = false, float fadeDuration = 0f)
    {
        // Already playing same clip and continuePlayback is requested
        if (continuePlayback && _musicSource.clip != null && _musicSource.clip.name == clip.name)
        {
            return;
        }

        // Start crossfade (or instant switch)
        if (_crossfadeCoroutine != null)
        {
            StopCoroutine(_crossfadeCoroutine);
        }

        _crossfadeCoroutine = StartCoroutine(CrossfadeMusic(clip, volume, fadeDuration));
    }

    private IEnumerator CrossfadeMusic(AudioClip newClip, float targetVolume, float duration)
    {
        if (duration <= 0f)
        {
            // Instant switch if no fade is needed
            _musicSource.Stop();
            _musicSource.clip = newClip;
            _musicSource.volume = targetVolume;
            _musicSource.Play();
            yield break;
        }

        float startVolume = _musicSource.volume;
        float time = 0f;

        // Fade out current music
        while (time < duration)
        {
            _musicSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        // Stop current music after fade-out is complete
        _musicSource.Stop();
        _musicSource.clip = newClip;
        _musicSource.Play();

        time = 0f;

        // Fade in the new music
        while (time < duration)
        {
            _musicSource.volume = Mathf.Lerp(0f, targetVolume, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        // Ensure we set the final volume value at the end of the fade-in
        _musicSource.volume = targetVolume;
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

    /// <summary>
    /// Play UI click sound.
    /// </summary>
    public void PlayClickSound()
    {
        PlaySound(clickSound, 0.5f);
    }
}