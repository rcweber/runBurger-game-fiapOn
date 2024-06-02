using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }
    public void PlayGame() 
    {
        audioManager.StopPlaying();
        SceneManager.LoadSceneAsync("FirstPhase");
    }

    public void Shop()
    {
        SceneManager.LoadSceneAsync("Shop");
    }
    public void QuitGame() 
    {
        Application.Quit();
    }
}
