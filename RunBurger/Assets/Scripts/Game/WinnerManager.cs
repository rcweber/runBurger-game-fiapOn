using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinnerManager : MonoBehaviour
{   

    public static WinnerManager instance;
    [SerializeField] private Button buttonPlayAgain;
    [SerializeField] private Button buttonExit;
    [SerializeField] Animator anim;

    public void Start() {

        anim = GetComponent<Animator>();
    }

    public void PlayGame() {

        SceneManager.LoadScene("FirstPhase_BattleArena");
    }
    public void QuitGame() {

        Application.Quit();
    }   
}
