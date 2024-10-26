using UnityEngine;

public class AudioControl : MonoBehaviour
{
    [SerializeField] private AudioClip bgmMusic;
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        audioManager.PlayBGM(bgmMusic, null);
    }
}
