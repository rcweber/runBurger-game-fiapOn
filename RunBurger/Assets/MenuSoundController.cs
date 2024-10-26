using UnityEngine;
using UnityEngine.UI;

public class MenuSoundController : MonoBehaviour
{

    [SerializeField] private AudioClip bgmSceneAudioClip;
    [SerializeField] private float bgmAudioVolume = 0.5f;

    [SerializeField] private Image volumeOn;
    [SerializeField] private Image volumeOff;

    // Start is called before the first frame update
    void Start()
    {
        if (AudioManager.instance != null && bgmSceneAudioClip != null && AudioManager.instance.GetAudioStateOnOff())
        {
            AudioManager.instance.PlayBGM(bgmSceneAudioClip, bgmAudioVolume);
        }

        volumeOn.gameObject.SetActive(AudioManager.instance.GetAudioStateOnOff());
        volumeOff.gameObject.SetActive(!AudioManager.instance.GetAudioStateOnOff());
    }

    // Update is called once per frame
    void Update()
    {
        volumeOn.gameObject.SetActive(AudioManager.instance.GetAudioStateOnOff());
        volumeOff.gameObject.SetActive(!AudioManager.instance.GetAudioStateOnOff());
    }

    public void StopPlayingMainMenu()
    {
        AudioManager.instance.TurnAudioOnOff();

        if (!AudioManager.instance.GetAudioStateOnOff())
        {
            AudioManager.instance.StopPlaying();
        }

        if (AudioManager.instance.GetAudioStateOnOff())
        {
            AudioManager.instance.PlayBGM(bgmSceneAudioClip, bgmAudioVolume);
        }
    }
}
