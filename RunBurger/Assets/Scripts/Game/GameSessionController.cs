using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSessionController : MonoBehaviour
{
    public enum GameSessionState
    {
        MainMenu,
        Game,
        GameOver
    }

    public enum GameSessionType
    {
        SinglePlayer,
        MultiPlayer
    }


    public static GameSessionController instance;

    private GameSessionState gameSessionState;
    private GameSessionType gameSessionType;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else Destroy(gameObject);
    }

    public void SetGameSessionState(GameSessionState state)
    {
        gameSessionState = state;
    }
    public GameSessionState GetGameSessionState() => gameSessionState;


    public void SetGameSessionType(GameSessionType type)
    {
        gameSessionType = type;
    }
    public GameSessionType GetGameSessionType() => gameSessionType;
    
}
