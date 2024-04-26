using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{

    private void Awake()
    {
        PlayerStatEventManager.Instance.OnDeath += OpenDeathScreen;
    }
    private void OnDestroy()
    {
        PlayerStatEventManager.Instance.OnDeath -= OpenDeathScreen;
    }

    public void LoadScene(int id)
    {
        Stats.Instance.Save();
        HintEventManager.Instance.SetVisibility(true);
        SceneManager.LoadScene(id);
    }
    public void OpenLevel()
    {
        LoadScene(4);
    }
    public void StartGame()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Instance.main_music);
        OpenMap();
    }
    public void OpenMap()
    {
        LoadScene(6);
    }

    public void OpenDeathScreen()
    {
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySound(AudioManager.Instance.loseSound);
        LoadScene(3);
    }
    public void OpenMainMenu()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Instance.menu_music);
        LoadScene(0);
    }
    public void OpenWinScene()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Instance.menu_music);
        AudioManager.Instance.PlaySound(AudioManager.Instance.winSound);
        LoadScene(7);
    }

    /// <summary>
    /// Close the game
    /// </summary>
    public void ExitGame()
    {
        Stats.Instance.Save();
        Application.Quit();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
