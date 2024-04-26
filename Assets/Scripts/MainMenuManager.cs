using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject TutorialPanel;
    public GameObject SettingsPanel;

    int mode = 0; // 0 - default, 1 - tutorial, 2 - settings

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && mode != 0)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.click);
            SetMode(0);
        }
    }

    public void SetMode(int mode)
    {
        this.mode = mode;
        switch (mode)
        {
            case 0:
                TutorialPanel.SetActive(false);
                SettingsPanel.SetActive(false);
                break;
            case 1:
                TutorialPanel.SetActive(true);
                SettingsPanel.SetActive(false);
                break;
            case 2:
                TutorialPanel.SetActive(false);
                SettingsPanel.SetActive(true);
                break;
            default: break;
        }
    }
}
