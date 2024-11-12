using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    public GameObject TutorialPanel;
    public GameObject SettingsPanel;
    public GameObject ControlsPanel;
    public ControlsMenu controlsMenu;

    int mode = 0; // 0 - default, 1 - tutorial, 2 - settings, 3 - controls

    private void Update()
    {
        if (!controlsMenu.isKeySelectActive() && Input.GetKeyDown(KeyCode.Escape) && mode != 0)
        {
            AudioManager.Instance.PlaySound(SoundClip.UIclick);
            SetMode(0);
        }
    }

    public void SetMode(int mode)
    {
        this.mode = mode;

        TutorialPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        ControlsPanel.SetActive(false);

        switch (mode)
        {
            case 0:
                break;
            case 1:
                TutorialPanel.SetActive(true);
                break;
            case 2:
                SettingsPanel.SetActive(true);
                break;
            case 3:
                ControlsPanel.SetActive(true);
                break;
            default: break;
        }

        EventSystem.current.SetSelectedGameObject(null);
    }
}
