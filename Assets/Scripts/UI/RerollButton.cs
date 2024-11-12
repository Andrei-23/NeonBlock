using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RerollButton : MonoBehaviour
{
    [SerializeField] private ShopPieceOption[] _options;
    [SerializeField] private int _cost;
    [SerializeField] private int _costIncrease;
    [SerializeField] private TextMeshProUGUI _costText;

    private void Start()
    {
        _costText.text = _cost.ToString();
    }

    public void TryActivate()
    {
        if(Stats.Instance.money >= _cost)
        {
            PlayerStatEventManager.Instance.AddMoney(-_cost);
            Activate();
        }
    }

    private void Activate()
    {
        foreach(ShopPieceOption option in _options)
        {
            option.gameObject.SetActive(true);
            option.Generate();
        }
        _cost += _costIncrease;
        _costText.text = _cost.ToString();
    }
}
