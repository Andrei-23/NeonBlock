using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour
{
    public Button button;
    
    private void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.click);
    }
}
