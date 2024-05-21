using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    //[SerializeField] private Button ButtonPlay;
    //[SerializeField] private Button ButtonExit;
    public void PlayGame() {

        SceneManager.LoadScene("FirstPhase");
    }
    public void QuitGame() {

        Application.Quit();
    }
}
