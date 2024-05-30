using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{   
    public Player player;
    public Text timeText;
    public float timeCount;
    public bool timeOver = false;
    public bool startTime = false;
    public Text coinsText;
    public float coinsCount;

    private GlobalController globalController;


    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        globalController = FindAnyObjectByType<GlobalController>();
    }

    private AudioManager audioManager;

    void Update() {
        if (player != null)
            TimeCount();
    }

    public void RefreshScreen() { 

        timeText.text = timeCount.ToString("F0");
        coinsText.text = coinsCount.ToString("F0");
    }

    public void TimeCount() { 
    
        timeOver = false;

        if (!timeOver && timeCount > 0 && startTime) { 
        
            timeCount -= Time.deltaTime;
            globalController.SetTimeLeft(timeCount);
            RefreshScreen();

            if (timeCount <= 0) { 
                audioManager.StopPlaying();
                timeCount = 0;
                Destroy(GameObject.Find("Player"));
                Destroy(GameObject.Find("Enemy"));
                SceneManager.LoadScene("GameOverFire");
                timeOver = true;
            }
        }
    }
}
