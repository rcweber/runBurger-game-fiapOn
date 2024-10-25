using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverCanvasSinglePlayer;
    [SerializeField] private GameObject gameOverCanvasMultiPlayer;

    [SerializeField] private AudioClip bgmWinnerMusic;
    [SerializeField] private float bgmAudioClipVolume = 0.55f;

    [Header("Configurações Single Player")]
    [SerializeField] private GameObject gameOverSinglePlayerStats;

    [Header("Configurações Multiplayer")]
    [SerializeField] private GameObject gameOverMultiPlayerStatsPlayerOne;
    [SerializeField] private GameObject gameOverMultiPlayerStatsPlayerTwo;

    private bool localDebug = false;

    private List<PlayerData> playerDatas;

    void Start()
    {
        // Starting the BGM
        if (AudioManager.instance != null && bgmWinnerMusic != null) AudioManager.instance.PlayBGM(bgmWinnerMusic, bgmAudioClipVolume);

        localDebug = GameSessionController.instance == null;

        if (localDebug)
        {
            gameOverCanvasSinglePlayer.SetActive(true);
            gameOverCanvasMultiPlayer.SetActive(false);
            return;
        }

        playerDatas = GlobalController.instance.GetPlayerDatas();
        PlayerData playerOneStats = null;
        PlayerData playerTwoStats = null;

        if (playerDatas.Count > 1)
        {
            playerOneStats = playerDatas[0];
            playerTwoStats = playerDatas[1];
        }
        else if (playerDatas.Count == 1)
        {
            playerOneStats = playerDatas[0];
        }

        // Decide qual o tipo de partida para exibir o canvas certo
        switch (GameSessionController.instance.GetGameSessionType())
        {
            case GameSessionController.GameSessionType.SinglePlayer:
                // Se for single player, exibe o canvas de game over
                gameOverCanvasSinglePlayer.SetActive(true);
                gameOverCanvasMultiPlayer.SetActive(false);
                if (playerOneStats != null)
                {
                    gameOverSinglePlayerStats.GetComponent<GameOverPlayerStats>().SetPlayerStats(playerOneStats);
                }
                break;
            default:
                // Se for multiplayer, exibe o canvas de game over multiplayer
                gameOverCanvasMultiPlayer.SetActive(true);
                gameOverCanvasSinglePlayer.SetActive(false);
                if (playerOneStats != null && playerTwoStats != null)
                {
                    gameOverMultiPlayerStatsPlayerOne.GetComponent<GameOverPlayerStats>().SetPlayerStats(playerOneStats, playerOneStats.playerColor);
                    gameOverMultiPlayerStatsPlayerTwo.GetComponent<GameOverPlayerStats>().SetPlayerStats(playerTwoStats, playerTwoStats.playerColor);
                }
                break;
        }
    }

    public void GoToMainMenu()
    {
        if (AudioManager.instance != null) AudioManager.instance.StopPlaying();
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayGame()
    {
        if (AudioManager.instance != null) AudioManager.instance.StopPlaying();
        SceneManager.LoadScene("FirstPhase");
    }
    public void QuitGame()
    {
        if (AudioManager.instance != null) AudioManager.instance.StopPlaying();
        Application.Quit();
    }
}
