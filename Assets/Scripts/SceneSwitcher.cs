using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{

    //public static SceneSwitcher Instance { get; private set; }

    private void Awake()
    {
        //if(Instance == null)
        //{
        //    Instance = this;
        //}
        //else
        //{
        //    Destroy(this);
        //}

        PlayerStatEventManager.Instance.OnDeath += OpenDeathScreen;
    }
    private void OnDestroy()
    {
        PlayerStatEventManager.Instance.OnDeath -= OpenDeathScreen;
    }

    public static void LoadScene(int id)
    {
        //Stats.Instance.Save();
        //HintEventManager.Instance.SetVisibility(true);
        SceneManager.LoadScene(id);
    }
    public static void OpenLevel()
    {
        LoadScene(4);
    }
    public static void StartGame()
    {
        AudioManager.Instance.SetMusic(SoundClip.gameMusic);
        GameStateManager.Instance.SetState(GameState.Menu);
        GameStateManager.Instance.SetPauseState(false);
        OpenMap();
    }
    public static void OpenMap()
    {
        LoadScene(6);
    }

    public static void OpenDeathScreen()
    {
        AudioManager.Instance.StopAllMusic();
        AudioManager.Instance.PlaySound(SoundClip.loseSound);
        LoadScene(3);
    }
    public static void OpenMainMenu()
    {
        AudioManager.Instance.SetMusic(SoundClip.meunMusic);
        LoadScene(0);
    }
    public static void OpenWinScene()
    {
        AudioManager.Instance.SetMusic(SoundClip.meunMusic);
        AudioManager.Instance.PlaySound(SoundClip.winSound);
        LoadScene(7);
    }

    /// <summary>
    /// Close the game
    /// </summary>
    public static void ExitGame()
    {
        Stats.Instance.Save();
        Application.Quit();
    }

}
