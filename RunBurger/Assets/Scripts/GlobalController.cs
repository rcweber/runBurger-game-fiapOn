using UnityEngine;
using UnityEngine.AI;

public class GlobalController : MonoBehaviour
{
    public static GlobalController instance;

    public float totalIcons = 0f;
    public float timeLeft = 0f;
    public float totalCoinsInScene = 0f;
    public bool isGameBlocked = false;

    public float GetTotalCoins() => totalIcons;
    public float GetTimeLeft() => timeLeft;
    public float GetTotalCoinsInScene() => totalCoinsInScene;
    public bool GetIsGameBlocked() => isGameBlocked;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        ResetCounts();

        totalCoinsInScene = GameObject.FindGameObjectsWithTag("Coin").Length;
    }

    public void ResetCounts() {
        isGameBlocked = false;
        totalIcons = 0f;
        timeLeft = 0f;
    }

    public void AddCoin(float coin)
    {
        totalIcons += coin;
    }

    public void SetTimeLeft(float timeLeft)
    {
        this.timeLeft = timeLeft;
    }

    public void SetBlockWholeGame(bool block) {
        isGameBlocked = block;
    }
}
