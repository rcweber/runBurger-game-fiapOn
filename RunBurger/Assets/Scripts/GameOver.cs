using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] private Button ButtonPlayAgain;
    [SerializeField] private Button ButtonExit;
    public void PlayGame() {

        SceneManager.LoadScene("FirstPhase");
    }
    public void QuitGame() {

        Application.Quit();
    }
}