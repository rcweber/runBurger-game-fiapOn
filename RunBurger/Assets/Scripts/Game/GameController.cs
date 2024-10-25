using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public static GameController instance;

    [Header("Configurações de tempo e moedas")]
    public Text timeText;
    public float timeCount;
    public bool timeOver = false;
    public bool startTime = false;
    public Text coinsText;
    public float coinsCount;


    [Header("Player One")]
    [SerializeField] private GameObject playerOneVisual;
    [SerializeField] private Image playerOneLifeBar;
    [SerializeField] private Image playerOneVelocitytBoost;
    [SerializeField] private Image[] playerOneImages;

    [Header("Player Two")]
    [SerializeField] private GameObject playerTwoVisual;
    [SerializeField] private Image playerTwoLifeBar;
    [SerializeField] private Image playerTwoVelocitytBoost;
    [SerializeField] private Image[] playerTwoImages;

    [Header("configurações de música da partida")]
    [SerializeField] private AudioClip bgmSceneAudioClip;

    private GlobalController globalController;
    private Player playerOne;
    private Player playerTwo;
    private AudioManager audioManager;

    public float GetTimeOnMoment => timeCount;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        globalController = FindAnyObjectByType<GlobalController>();

        // Listen to the player joined event
        BattleArenaPlayerManager.instance.OnPlayerJoined += Player_OnPlayerJoined;
        GameManager.instance.OnMatchStarted += GameManagerController_OnGameStart;

        if (playerOneVisual != null) playerOneVisual.SetActive(false);
        if (playerTwoVisual != null) playerTwoVisual.SetActive(false);
    }



    void Update()
    {
        RefreshTimeCount();
        RefreshCoinsCount();
        RefreshTheLifeBar();
        RefreshVeolicytBoost();
    }

    private void RefreshTimeCount()
    {
        if (playerOne != null || playerTwo != null)
            TimeCount();

    }

    private void RefreshCoinsCount()
    {
        if (playerOne != null)
        {
            if (playerOneVisual.TryGetComponent<PlayerCanvasVisual>(out var playerCanvasVisual))
            {
                playerCanvasVisual.SetCoinsCount(playerOne.GetCollectedCoins());
            }
        }
        if (playerTwo != null)
        {
            if (playerTwoVisual.TryGetComponent<PlayerCanvasVisual>(out var playerCanvasVisual))
            {
                playerCanvasVisual.SetCoinsCount(playerTwo.GetCollectedCoins());
            }
        }
    }

    private void RefreshTheLifeBar()
    {
        if (playerOne != null) UpdateTheLifeBar(playerOne, playerOneLifeBar);
        if (playerTwo != null) UpdateTheLifeBar(playerTwo, playerTwoLifeBar);
    }

    private void UpdateTheLifeBar(Player player, Image lifeBar)
    {
        lifeBar.fillAmount = player.GetPowerLife() / 9.99f;
    }

    private void RefreshVeolicytBoost()
    {
        if (playerOne != null) UpdateBoostVelocity(playerOne, playerOneVelocitytBoost);
        if (playerTwo != null) UpdateBoostVelocity(playerTwo, playerTwoVelocitytBoost);

    }

    private void UpdateBoostVelocity(Player player, Image bootVelocityImage)
    {
        if (player.GetIsBoosting())
            bootVelocityImage.fillAmount = player.GetBoostTimer() / player.GetBoostDuration();
        else if (player.GetIsInCooldown())
            bootVelocityImage.fillAmount = (player.GetBoostCooldown() - player.GetBoostCooldownTimer()) / player.GetBoostCooldown();
        else bootVelocityImage.fillAmount = 1;
    }

    public void TimeCount()
    {
        timeOver = false;

        if (!timeOver && timeCount > 0 && startTime)
        {
            timeCount -= Time.deltaTime;
            globalController.SetTimeLeft(timeCount);
            ShowingTime();
            if (timeCount <= 0)
            {
                audioManager.StopPlaying();
                timeCount = 0;
                Destroy(GameObject.Find("Player"));
                Destroy(GameObject.Find("Enemy"));
                // TODO: Ver como vai ficar a tela final
                // SceneManager.LoadScene("GameOverFire");
                timeOver = true;
            }
        }
    }

    private void ShowingTime()
    {
        int minutes = Mathf.FloorToInt(timeCount / 60); // Divide o tempo total por 60 para obter os minutos
        int seconds = Mathf.FloorToInt(timeCount % 60); // Usa o resto da divisão por 60 para obter os segundos

        // Exibe o tempo formatado como "mm:ss"
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void Player_OnPlayerJoined(object sender, BattleArenaPlayerManager.OnPlayerJoinedEventArgs e)
    {
        if (e.playerIndex == 0) playerOne = e.player;
        if (e.playerIndex == 1) playerTwo = e.player;

        ConfigurePlayerVisuals();
        ConfigurePlayerEvents();
    }

    private void GameManagerController_OnGameStart(object sender, System.EventArgs e)
    {
        startTime = true;
    }

    private void ConfigurePlayerEvents()
    {
        if (playerOne != null)
        {
            playerOne.OnPlayerDied += Player_OnPlayerDied;
        }
        if (playerTwo != null)
        {
            playerTwo.OnPlayerDied += Player_OnPlayerDied;
        }
    }

    private void Player_OnPlayerDied(object sender, EventArgs e)
    {
        var graphics = new List<Image>();
        if ((Player)sender == playerOne)
        {
            graphics = playerOneImages.ToList();

        }
        if ((Player)sender == playerTwo)
        {
            graphics = playerTwoImages.ToList();
        }

        SetTransparencyInChildren(0.1f, graphics);
    }
    public void SetTransparencyInChildren(float alpha, List<Image> graphics)
    {
        foreach (Graphic graphic in graphics)
        {
            Color color = graphic.color;
            color.a = Mathf.Clamp(alpha, 0f, 1f); // Garante que o alpha fique entre 0 e 1
            graphic.color = color;
        }
    }

    private void ConfigurePlayerVisuals()
    {
        if (playerOne != null)
        {
            if (playerOneVisual.TryGetComponent<PlayerCanvasVisual>(out var playerCanvasVisual))
            {
                if (playerOneVisual != null) playerOneVisual.SetActive(true);
                playerCanvasVisual.SetPlayerColor(playerOne.GetPlayerColor());
                playerOneLifeBar.color = playerOne.GetPlayerColor();
            }
        }
        if (playerTwo != null)
        {
            if (playerTwoVisual.TryGetComponent<PlayerCanvasVisual>(out var playerCanvasVisual))
            {
                if (playerTwoVisual != null) playerTwoVisual.SetActive(true);
                playerCanvasVisual.SetPlayerColor(playerTwo.GetPlayerColor());
                playerTwoLifeBar.color = playerTwo.GetPlayerColor();
            }
        }
    }
}
