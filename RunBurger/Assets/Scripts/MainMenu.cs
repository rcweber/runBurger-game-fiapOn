using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    private AudioManager audioManager;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void PlayGame() {
        audioManager.StopPlaying();
        SceneManager.LoadSceneAsync("FirstPhase");
    }
    public void QuitGame() {

        Application.Quit();
    }
}
