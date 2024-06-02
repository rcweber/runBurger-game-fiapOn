using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopMenu : MonoBehaviour
{
    public void Back() {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
