using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


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
    public void SetTotalInconeInScene(float totalCoinsInScene) => this.totalCoinsInScene = totalCoinsInScene;

    private List<PlayerData> playerDatas = new();

    void Awake()
    {
        if (instance == null)
        {
            this.ResetCounts();
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else Destroy(gameObject);
    }


    void Start()
    {
        ResetCounts();
    }

    public void ResetCounts()
    {
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

    public void SetBlockWholeGame(bool block)
    {
        isGameBlocked = block;
    }

    public List<PlayerData> GetPlayerDatas()
    {
        return playerDatas;
    }

    public void SetPlayerDatas(List<PlayerData> playerDatas)
    {
        this.playerDatas = playerDatas;
    }
}
