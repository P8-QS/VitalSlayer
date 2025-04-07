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

    public void PlaySound(AudioClip clip, Transform parent, float volume)
    {
        AudioSource audioSource = Instantiate(soundFxObject, parent);
        
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        
        Destroy(audioSource.gameObject, clip.length);
    }

    public void PlaySoundAtGlobal(AudioClip clip, float volume)
    {
        AudioSource audioSource = Instantiate(soundFxObject, transform);
        
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        
        Destroy(audioSource.gameObject, clip.length);
    }
    
    
    
}
