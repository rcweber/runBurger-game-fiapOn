using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource audioSource;

    public bool IsPlaying() => audioSource.isPlaying;
    public bool TurnAudioOnOff() => musicOn = !musicOn;
    public bool GetAudioStateOnOff() => musicOn;

    private bool musicOn = true;

    void Start()
    {
        if (audioSource != null) audioSource.loop = true;
    }

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

    public void PauseBGM()
    {
        audioSource.Pause();
    }

    public void ResumeBGM()
    {
        audioSource.UnPause();
    }

    public void PlaySFX(AudioClip audioClip, float volume = 0.5f)
    {
        audioSource.PlayOneShot(audioClip, volume);
    }
}
