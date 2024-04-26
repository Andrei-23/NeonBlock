using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TurnText : MonoBehaviour
{
    private TextMeshProUGUI tm;
    private void Awake()
    {
        tm = GetComponent<TextMeshProUGUI>();
        PlayerStatEventManager.Instance.OnTurnCountChanged += UpdateText;
    }
    private void Start()
    {
        UpdateText();
    }
    private void OnDestroy()
    {
        PlayerStatEventManager.Instance.OnTurnCountChanged -= UpdateText;
    }
    void Update()
    {
        UpdateText();
    }

    void UpdateText()
    {
        tm.text = (Stats.Instance.turn_limit - Stats.Instance.turn_cnt).ToString();
    }
}
