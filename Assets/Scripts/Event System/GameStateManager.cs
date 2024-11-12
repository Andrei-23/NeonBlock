using System;
using System.Diagnostics;

public class GameStateManager
{
    private static GameStateManager _instance;
    public static GameStateManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new GameStateManager();
            }
            return _instance;
        }
    }

    public GameState CurrentGameState { get; private set; }
    public bool IsPaused { get; private set; }


    public delegate void GameStateChangeHandler(GameState newGameState);
    public event GameStateChangeHandler OnGameStateChanged;

    public delegate void PauseStateChangeHandler(bool is_paused);
    public event PauseStateChangeHandler OnPauseStateChanged;

    private GameStateManager()
    {
        CurrentGameState = GameState.Menu;
        IsPaused = false;
    }

    public void SetState(GameState newGameState)
    {
        if (newGameState == CurrentGameState) return;

        CurrentGameState = newGameState;
        OnGameStateChanged?.Invoke(newGameState);
        UpdateHintVisibility();
    }
    public void SetPauseState(bool is_paused)
    {
        if (is_paused == IsPaused) return;
        
        IsPaused = is_paused;
        OnPauseStateChanged?.Invoke(IsPaused);
        UpdateHintVisibility();
    }

    private void UpdateHintVisibility()
    {
        HintEventManager.Instance.SetVisibility(CurrentGameState != GameState.Gameplay || IsPaused);
    }
    public void SwitchPauseState()
    {
        SetPauseState(!IsPaused);
    }

}
