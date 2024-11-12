using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllButtonScript : MonoBehaviour
{
    void Start()
    {
        //Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        //foreach (Button button in buttons)
        //{
        //    button.onClick.AddListener(OnClick);
        //    button.gameObject.AddComponent<ButtonScript>();
        //}

    }
    private void OnClick()
    {
        AudioManager.Instance.PlaySound(SoundClip.UIclick);
    }
}
