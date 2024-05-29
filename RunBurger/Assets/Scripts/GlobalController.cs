using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GlobalController : MonoBehaviour
{
    public static GlobalController instance;

    private float totalIcons = 0f;
    private float timeLeft = 0f;
    public float totalCoinsInScene = 0f;

    public float GetTotalCoins() => totalIcons;
    public float GetTimeLeft() => timeLeft;
    public float GetTotalCoinsInScene() => totalCoinsInScene;

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
        totalCoinsInScene = GameObject.FindGameObjectsWithTag("Coin").Length;
    }

    public void AddCoin(float coin)
    {
        totalIcons += coin;
    }

    public void SetTimeLeft(float timeLeft)
    {
        this.timeLeft = timeLeft;
    }
}
