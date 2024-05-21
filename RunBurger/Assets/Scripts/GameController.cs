using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{   

    public static GameController instance;
    public Player player;
    public Text timeText;
    public float timeCount;
    public bool timeOver = false;
    public bool startTime = false;
    void Update()
    {   
        if (player != null)
            TimeCount();
    }

    public void RefreshScreen() { 
    
        timeText.text = timeCount.ToString("F0");    
    }

    public void TimeCount() { 
    
        timeOver = false;

        if (!timeOver && timeCount > 0 && startTime) { 
        
            timeCount -= Time.deltaTime;
            RefreshScreen();

            if (timeCount <= 0) { 
                
                timeCount = 0;
                Destroy(GameObject.Find("Player"));
                Destroy(GameObject.Find("Enemy"));
                SceneManager.LoadScene("GameOverFire");
                timeOver = true;
            }
        }
    }
}
