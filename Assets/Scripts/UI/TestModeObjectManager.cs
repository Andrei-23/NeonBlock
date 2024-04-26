using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestModeObjectManager : MonoBehaviour
{
    /// <summary>
    /// These objects active only in test mode.
    /// </summary>
    public List<GameObject> testModeObjects;

    private void Awake()
    {
        PlayerStatEventManager.Instance.OnTestModeChange += OnTestModeChange;
    }
    private void OnDestroy()
    {
        PlayerStatEventManager.Instance.OnTestModeChange -= OnTestModeChange;
    }

    public void OnTestModeChange(bool isActive)
    {
        foreach (GameObject obj in testModeObjects)
        {
            obj.SetActive(isActive);
        }
    }

    void Start()
    {
        OnTestModeChange(Stats.Instance.test_mode); // initial activation/disactivation
    }

    void Update()
    {
        
    }
}
