using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PauseButtonListener : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.click);
            OnClick();
        }
    }

    public void OnClick()
    {
        GameStateManager.Instance.SwitchState();
    }
}
