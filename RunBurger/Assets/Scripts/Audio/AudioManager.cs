using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    [SerializeField] private AudioSource audioSource;

    public bool IsPlaying() => audioSource.isPlaying;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(AudioClip audioClip, float? bgmAudioVolume)
    {
        if (bgmAudioVolume != null) audioSource.volume = bgmAudioVolume.Value;
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void StopPlaying()
    {
        audioSource.Stop();
    }

    public void PlayOneShot(AudioClip audioClip, float volume = 0.5f)
    {
        audioSource.PlayOneShot(audioClip, volume);
    }

}