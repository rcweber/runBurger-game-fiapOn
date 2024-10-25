using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattleArenaPlayerManager : MonoBehaviour
{
    [SerializeField] private int playerCount = 2;

    [Tooltip("Cor do player 1")]
    [SerializeField] private Color playerOneColor = default;
    [Tooltip("Cor do player 2")]
    [SerializeField] private Color playerTwoColor = Color.red;

    [Header("Prefabs do player")]
    [SerializeField] private GameObject playerOnePrefab;
    [SerializeField] private GameObject playerTwoPrefab;

    [Header("Configurações de bloqueio do jogador")]
    [Tooltip("Bloqueador inicial do player 1")]
    [SerializeField] private GameObject barInitialBlockerPlayerOne;
    [Tooltip("Bloqueador inicial do player 2")]
    [SerializeField] private GameObject barInitialBlockerPlayerTwo;

    // Events
    public event EventHandler<OnPlayerJoinedEventArgs> OnPlayerJoined;
    public class OnPlayerJoinedEventArgs : EventArgs
    {
        public Player player;
        public int playerIndex;
    }

    private GameObject[] spawnPoints;

    // Instance
    public static BattleArenaPlayerManager instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // Points to spwan players
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        if (playerOneColor != null)
        {
            // Setting up the player one indicator color
            if (!ColorUtility.TryParseHtmlString("#066EFF", out playerOneColor))
            {
                Debug.LogError("Failed to parse player one color");
            }
        }

        if (playerTwoColor != null)
        {
            // Setting up the player one indicator color
            if (!ColorUtility.TryParseHtmlString("#FF2806", out playerTwoColor))
            {
                Debug.LogError("Failed to parse player one color");
            }
        }
    }

    private void CreatePlayerOne()
    {
        // Faz o join do Player 1 com o esquema correto
        PlayerInputManager.instance.playerPrefab = playerOnePrefab;
        var playerOne = PlayerInputManager.instance.JoinPlayer(0, -1);

        if (playerOne != null)
        {
            // Define a posição inicial do player
            playerOne.transform.position = spawnPoints[0].transform.position;
            playerOne.transform.parent = transform;

            // Define a cor e o índice do player
            playerOne.GetComponent<Player>().SetPlayerColor(playerOneColor);
            playerOne.GetComponent<Player>().SetPlayerDeviceInput(playerOne);
            playerOne.GetComponent<Player>().SetPlayerIndex(0);

            // Dispara o evento de criação do player
            OnPlayerJoined?.Invoke(this, new OnPlayerJoinedEventArgs { player = playerOne.GetComponent<Player>(), playerIndex = 0 });

            // Liberando o bloqueio do jogador
            if (barInitialBlockerPlayerOne != null) barInitialBlockerPlayerOne.SetActive(false);
        }
    }

    private void CreatePlayerTwo()
    {
        // Faz o join do Player 2 com o esquema correto
        PlayerInputManager.instance.playerPrefab = playerTwoPrefab;
        var playerTwo = PlayerInputManager.instance.JoinPlayer(1, -1);

        if (playerTwo != null)
        {
            // Define a posição inicial do player
            playerTwo.transform.position = spawnPoints[1].transform.position;
            playerTwo.transform.parent = transform;

            // Define a cor e o índice do player
            playerTwo.GetComponent<Player>().SetPlayerColor(playerTwoColor);
            playerTwo.GetComponent<Player>().SetPlayerDeviceInput(playerTwo);
            playerTwo.GetComponent<Player>().SetPlayerIndex(1);

            // Dispara o evento de criação do player
            OnPlayerJoined?.Invoke(this, new OnPlayerJoinedEventArgs { player = playerTwo.GetComponent<Player>(), playerIndex = 1 });

            // Liberando o bloqueio do jogador
            if (barInitialBlockerPlayerTwo != null) barInitialBlockerPlayerTwo.SetActive(false);
        }
    }


    void Start()
    {

        if (GameSessionController.instance != null && GameSessionController.instance.GetGameSessionType() == GameSessionController.GameSessionType.SinglePlayer)
        {
            CreatePlayerOne();
        }
        else if (GameSessionController.instance != null && GameSessionController.instance.GetGameSessionType() == GameSessionController.GameSessionType.MultiPlayer)
        {
            CreatePlayerOne();
            CreatePlayerTwo();
        }
        else
        {
            Debug.Log("GameSessionController not found or game session type not set");
            CreatePlayerOne();
        }
    }


}
