using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllButtonClickSound : MonoBehaviour
{
    void Start()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(OnClick);
        }

    }
    private void OnClick()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.click);
    }
}
