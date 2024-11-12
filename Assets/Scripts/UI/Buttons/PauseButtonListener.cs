using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseButtonListener : MonoBehaviour
{
    private PlayerInput _playerInput;
    private InputAction _pauseAction;
    private void Start()
    {
        _playerInput = Camera.main.GetComponent<PlayerInput>();
        _pauseAction = _playerInput.actions["Pause"];
    }
    void Update()
    {
        if (_pauseAction.WasPressedThisFrame())
        {
            // buttons have sound already
            AudioManager.Instance.PlaySound(SoundClip.UIclick);
            OnClick();
        }
    }

    public void OnClick()
    {
        GameStateManager.Instance.SwitchPauseState();
    }
}
