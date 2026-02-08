using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver
    }

    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetState(GameState.Menu);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Menu:
                Time.timeScale = 1f;
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                break;

            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
        }
    }
    public void TogglePause()
    {
        if (CurrentState == GameState.Playing)
            SetState(GameState.Paused);
        else if (CurrentState == GameState.Paused)
            SetState(GameState.Playing);
    }
}