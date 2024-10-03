using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private AudioManager audioManager;
    // [SerializeField] private GameObject shopContainer;
    [SerializeField] private GameObject shopBackground;
    [SerializeField] private GameObject buttonsContainer;
    [SerializeField] private GameObject firstButton;


    void OnEnable()
    {
        Debug.Log("OnEnable called");
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            Debug.Log("No selected object");
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
    }

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        // shopContainer.gameObject.SetActive(false);
        if (shopBackground != null) shopBackground.SetActive(false);
        buttonsContainer.SetActive(true);

    }
    public void PlayGame() 
    {
        audioManager.StopPlaying();
        SceneManager.LoadSceneAsync("FirstPhase");
    }

    public void Shop()
    {
        // shopContainer.gameObject.SetActive(true);
        shopBackground.SetActive(true);
        buttonsContainer.SetActive(false);
    }   
    public void CloseBG()
    {
        // shopContainer.gameObject.SetActive(false);
        if (shopBackground != null) shopBackground.SetActive(false);
        buttonsContainer.SetActive(true);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
