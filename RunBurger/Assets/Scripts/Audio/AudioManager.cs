using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private Image volumeOn;
    [SerializeField] private Image volumeOff;
    private bool audioOn = true;
    private bool audioOff = false;
    public bool IsPlaying() => audioSource.isPlaying;
    public bool TurnAudioOnOff() => audioOn = !audioOn;

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

        if (volumeOn != null && volumeOff != null)
        {
            volumeOn.gameObject.SetActive(audioOn);
            volumeOff.gameObject.SetActive(audioOff);
        }
    }
    private void Update()
    {
        if (volumeOn != null && volumeOff != null) { 
            volumeOn.gameObject.SetActive(audioOn);
            volumeOff.gameObject.SetActive(audioOff);
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

    public void StopPlayingMainMenu()
    {
        audioOn = !audioOn;
        audioOff = !audioOff;

        if (audioOff)
            audioSource.Stop();

        if (audioOn)
            audioSource.Play();
    }

    public void PlayOneShot(AudioClip audioClip, float volume = 0.5f)
    {
        audioSource.PlayOneShot(audioClip, volume);
    }
}
