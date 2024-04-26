using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestModeToggle : MonoBehaviour
{
    public void OnValueChanged(bool value)
    {
        PlayerStatEventManager.Instance.SetTestMode(value);
        Debug.Log("Test mode: " + (value ? "ON" : "OFF"));
    }
    void Start()
    {
        gameObject.GetComponent<Toggle>().isOn = Stats.Instance.test_mode;
    }

    void Update()
    {
        
    }
}
