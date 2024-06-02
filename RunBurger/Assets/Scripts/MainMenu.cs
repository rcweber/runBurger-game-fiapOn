using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private AudioManager audioManager;
    [SerializeField] private GameObject shopContainer;
    [SerializeField] private GameObject shopBackground;
    [SerializeField] private GameObject buttonsContainer;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        shopContainer.gameObject.SetActive(false);
        shopBackground.gameObject.SetActive(false);
        buttonsContainer.gameObject.SetActive(true);

    }
    public void PlayGame() 
    {
        audioManager.StopPlaying();
        SceneManager.LoadSceneAsync("FirstPhase");
    }

    public void Shop()
    {
        shopContainer.gameObject.SetActive(true);
        shopBackground.gameObject.SetActive(true);
        buttonsContainer.gameObject.SetActive(false);
    }
    public void QuitGame() 
    {
        Application.Quit();
    }
    public void CloseBG()
    {
        shopContainer.gameObject.SetActive(false);
        shopBackground.gameObject.SetActive(false);
        buttonsContainer.gameObject.SetActive(true);
    }
}
