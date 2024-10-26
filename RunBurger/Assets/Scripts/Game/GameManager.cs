using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static GameSessionController;
using static UnityEngine.InputSystem.InputAction;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Configurações de gerenciamento de dispositivos")]
    [SerializeField] private GameObject gameDisconectedControlsPausedCanvas;

    [Header("Configurações de Pausa do game")]
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject canvasToHide;

    [Header("Configurações de início do game")]
    [Tooltip("Canvas de início do game")]
    [SerializeField] private GameObject startGameCanvas;
    [Tooltip("Texto de contagem regressiva")]
    [SerializeField] private TextMeshProUGUI startCountDownText;
    [Tooltip("Esse é o texto mostrado na tela de contagem regressiva, será alterado de acordo com o Game Session Type")]
    [SerializeField] private TextMeshProUGUI textoTelaContagem;
    [Tooltip("Contagem regressiva para início do game")]
    [SerializeField] private float startCountDown = 5.0f;
    [SerializeField] private AudioClip startCountDownSound;
    [SerializeField] private AudioClip endCountDownSound;
    [SerializeField] private float startCountDownVolume = 0.6f;
    [Header("configurações de música da partida")]
    [SerializeField] private AudioClip bgmSceneAudioClip;
    [SerializeField] private float bgmSceneAudioVolume = 0.6f;

    [Header("Game Over Settings")]
    [SerializeField] private GameObject gameOverCanvas;

    public bool IsCountingDown => isCountingDown;
    public bool IsPaused => isPaused;

    private bool isCountingDown = false;
    private bool isPaused = false;
    private bool isMatchStarted = false;
    private AudioManager audioManager;
    private List<Player> players = new();
    private List<PlayerData> playerDatas = new();

    public bool GetMatchStarted => isMatchStarted;
    public EventHandler OnMatchStarted;
    public EventHandler OnMatchEnded;

    public void SetMatchStarted()
    {
        isMatchStarted = true;
        OnMatchStarted?.Invoke(this, EventArgs.Empty);
        if (AudioManager.instance.GetAudioStateOnOff()) StartCoroutine(BeginSceneMusic());
        // Obtendo o total de icones na cena
        GlobalController.instance.SetTotalInconeInScene(GameObject.FindGameObjectsWithTag("Coin").Length);
    }

    public void SetMatchEnded()
    {
        isMatchStarted = false;
        OnMatchEnded?.Invoke(this, EventArgs.Empty);
    }

    public void TimesUp()
    {
        var players = FindObjectsOfType<Player>();
        if (players != null) players.ToList().ForEach(player => player.Dead());
        playerDatas.ForEach(x => x.PlayerDied = true);
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);
    }

    void Update()
    {
        if (isMatchStarted)
        {
            CheckPlayersAlive();
        }
    }

    void Start()
    {
        Time.timeScale = 0f;
        BattleArenaPlayerManager.instance.OnPlayerJoined += OnPlayerJoined;

        audioManager = FindObjectOfType<AudioManager>();
        // Inicia a contagem regressiva
        StartCoroutine(CountdownToStart());

        if (GameSessionController.instance != null && GameSessionController.instance.GetGameSessionType() == GameSessionType.SinglePlayer)
        {
            if (textoTelaContagem != null) textoTelaContagem.SetText("PREPARADO?");
        }
        else if (GameSessionController.instance != null && GameSessionController.instance.GetGameSessionType() == GameSessionType.MultiPlayer)
        {
            if (textoTelaContagem != null) textoTelaContagem.SetText("PREPARADOS?");
        }
    }

    private void OnPlayerJoined(object sender, BattleArenaPlayerManager.OnPlayerJoinedEventArgs e)
    {
        // Adding player to the global controller list, to get in the last scene.
        players.Add(e.player);
        playerDatas.Add(new PlayerData() { playerIndex = e.player.GetPlayerIndex(), playerColor = e.player.GetPlayerColor(), PlayerCurrentLife = (int)e.player.GetPowerLife() });
        e.player.OnPlayerGetCoin += OnPlayerGetCoin;
        e.player.OnPleyerExit += OnPleyerExit;
        e.player.OnPlayerDied += OnPlayerDied;
        e.player.OnPlayerHurt += OnPlayerHurt;
    }

    private void OnPlayerHurt(object sender, Player.OnPlayerHurtEventArgs e)
    {
        var playerData = playerDatas.FirstOrDefault(playerData => playerData.playerIndex == ((Player)sender).GetPlayerIndex());
        if (playerData != null) playerData.PlayerCurrentLife = e.CurrentLife;
    }

    private void OnPlayerDied(object sender, EventArgs e)
    {
        var playerData = playerDatas.FirstOrDefault(playerData => playerData.playerIndex == ((Player)sender).GetPlayerIndex());
        if (playerData != null)
        {
            playerData.PlayerDied = true;
            playerData.PlayerCurrentTime = GameController.instance.GetTimeOnMoment;
        }
    }

    private void OnPleyerExit(object sender, Player.OnPlayerDoSomethingEventArgs e)
    {
        var playerData = playerDatas.FirstOrDefault(playerData => playerData.playerIndex == e.playerIndex);
        if (playerData != null)
        {
            playerData.totalCoints = e.CoinAmount;
            playerData.Exited = true;
            playerData.PlayerCurrentTime = GameController.instance.GetTimeOnMoment;
        }
    }

    private void OnPlayerGetCoin(object sender, Player.OnPlayerDoSomethingEventArgs e)
    {
        var playerData = playerDatas.FirstOrDefault(playerData => playerData.playerIndex == ((Player)sender).GetPlayerIndex());
        if (playerData != null) playerData.totalCoints += e.CoinAmount;
    }

    private void CheckPlayersAlive()
    {
        // Tem que verificar se existem jogadores ativos, ou seja, não morreram ou não saíram do tabuleiro
        var hasPlayerAlive = playerDatas.Any(playerData => !playerData.PlayerDied && !playerData.Exited);

        if (!hasPlayerAlive)
        {
            // Load the game over scene
            Debug.Log("Todos os jogadores morreram ou saíram");
            HandleGameOver();
        }
    }

    private void HandleGameOver()
    {
        SetMatchEnded();
        // Bloquear o jogo
        gameOverCanvas.SetActive(true);
        // Ler a cena de game over mas demorar uns segundos pra chamar
        AudioManager.instance.StopPlaying();
        GlobalController.instance.SetPlayerDatas(playerDatas);
        StartCoroutine(LoadGameOverFireScene());
    }

    private IEnumerator LoadGameOverFireScene()
    {
        yield return new WaitForSeconds(3.1f);
        Debug.Log("Carregando cena de game over");
        SceneManager.LoadSceneAsync("WinnerGameOverFire");
    }

    private IEnumerator BeginSceneMusic()
    {
        while (audioManager.IsPlaying())
        {
            yield return new WaitForSeconds(0.01f);
        }
        while (isPaused)
        {
            yield return new WaitForSeconds(0.01f);
        }
        if (bgmSceneAudioClip != null && !audioManager.IsPlaying() && !isPaused)
            audioManager.PlayBGM(bgmSceneAudioClip, bgmSceneAudioVolume);
    }

    private IEnumerator CountdownToStart()
    {
        // wating to load screen
        isCountingDown = true;
        yield return new WaitForSecondsRealtime(1f);

        while (startCountDown > 0)
        {
            if (startCountDownText != null)
            {
                // Atualiza o texto da contagem com arredondamento
                startCountDownText.SetText(Mathf.Ceil(startCountDown).ToString());
            }

            // Aguarda até o próximo frame e diminui o contador e executa um feedback sonoro
            if (startCountDownSound != null) AudioManager.instance.PlaySFX(startCountDownSound, startCountDownVolume);
            yield return new WaitForSecondsRealtime(1f);
            startCountDown -= 1f;
        }

        // Exibe a mensagem de início quando o tempo acabar
        if (startCountDownText != null)
        {
            startCountDownText.SetText("BORA!");
            if (endCountDownSound != null) AudioManager.instance.PlaySFX(endCountDownSound, startCountDownVolume);
        }

        // Aguarda mais 1 segundo antes de iniciar a partida
        yield return new WaitForSecondsRealtime(1.1f);
        isCountingDown = false;
        Time.timeScale = 1f;

        // Desativa o canvas de início
        if (startGameCanvas != null)
        {
            startGameCanvas.SetActive(false);
        }
        SetMatchStarted();
    }

    void OnEnable()
    {
        // Registra o callback para mudanças de dispositivos
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    void OnDisable()
    {
        // Remove o callback quando o objeto é desabilitado
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    // Função chamada quando há mudança de dispositivo (conexão/desconexão)
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Disconnected:
                Debug.Log("Dispositivo desconectado: " + device.displayName);
                PauseGameDisconectedControl();
                break;

            case InputDeviceChange.Reconnected:
                Debug.Log("Dispositivo reconectado: " + device.displayName);
                ResumeGameDisconectedControl();
                break;
        }
    }

    // Função de pausa
    private void PauseGameDisconectedControl()
    {
        Time.timeScale = 0f;
        if (gameDisconectedControlsPausedCanvas != null)
        {
            gameDisconectedControlsPausedCanvas.SetActive(true);
            audioManager.PauseBGM();
        }
    }

    // Função para retomar o jogo
    private void ResumeGameDisconectedControl()
    {
        Time.timeScale = 1f;
        if (gameDisconectedControlsPausedCanvas != null)
        {
            gameDisconectedControlsPausedCanvas.SetActive(false);
            audioManager.ResumeBGM();
        }
    }

    public void SetPauseCallbackContext(CallbackContext context)
    {
        if (context.performed)
        {
            isPaused = true;
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isCountingDown) return;
        isPaused = true;
        audioManager.PauseBGM();
        Time.timeScale = 0f;
        pauseCanvas.SetActive(true);
        if (canvasToHide != null) canvasToHide.SetActive(false);
    }

    public void ResumeGame()
    {
        audioManager.ResumeBGM();
        isPaused = false;
        Time.timeScale = 1f;
        pauseCanvas.SetActive(false);
        if (canvasToHide != null) canvasToHide.SetActive(true);
    }

    public void ReturningToMenu()
    {
        audioManager.StopPlaying();
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void ReloadrMatch()
    {
        audioManager.StopPlaying();
        SceneManager.LoadSceneAsync("FirstPhase_BattleArena", LoadSceneMode.Single);

        UnloadUnusedAssets();
    }

    private IEnumerator UnloadUnusedAssets()
    {
        yield return null;
        yield return Resources.UnloadUnusedAssets();

        System.GC.Collect();
    }
}
