using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public void PlayGame() {

        SceneManager.LoadScene("FirstPhase");
    }
    public void QuitGame() {

        Application.Quit();
    }
}
