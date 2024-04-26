using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class MoneyText : MonoBehaviour
{
    private TextMeshProUGUI money_text;
    void Awake()
    {
        money_text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        money_text.text = Stats.Instance.money.ToString();
    }
}
