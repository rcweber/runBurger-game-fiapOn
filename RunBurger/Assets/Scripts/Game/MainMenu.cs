using System.Collections;
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
        GameSessionController.instance.SetGameSessionState(GameSessionController.GameSessionState.Game);
        GameSessionController.instance.SetGameSessionType(GameSessionController.GameSessionType.SinglePlayer);
        SceneManager.LoadSceneAsync("FirstPhase_BattleArena", LoadSceneMode.Single);
    }

    public void PlayerBattleArena()
    {
        audioManager.StopPlaying();
        GameSessionController.instance.SetGameSessionState(GameSessionController.GameSessionState.Game);
        GameSessionController.instance.SetGameSessionType(GameSessionController.GameSessionType.MultiPlayer);
        SceneManager.LoadSceneAsync("FirstPhase_BattleArena", LoadSceneMode.Single);
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

    // void OnDestroy()
    // {
    //     StartCoroutine(UnloadUnusedAssets());
    // }

    // private IEnumerator UnloadUnusedAssets()
    // {
    //     yield return null;
    //     yield return Resources.UnloadUnusedAssets();

    //     System.GC.Collect();
    //     StopAllCoroutines();
    // }
}
