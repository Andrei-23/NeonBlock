using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyAddButton : MonoBehaviour
{
    public void OnClick()
    {
        PlayerStatEventManager.Instance.AddMoney(500);
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
