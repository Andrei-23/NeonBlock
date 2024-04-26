using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RerollButton : MonoBehaviour
{
    public ShopPieceOption[] options;
    public int cost;
    public TextMeshProUGUI costText;

    private void Start()
    {
        costText.text = cost.ToString();
    }

    public void TryActivate()
    {
        if(Stats.Instance.money >= cost)
        {
            PlayerStatEventManager.Instance.AddMoney(-cost);
            Activate();
        }
    }

    private void Activate()
    {
        foreach(ShopPieceOption option in options)
        {
            option.gameObject.SetActive(true);
            option.Generate();
        }
    }
}
