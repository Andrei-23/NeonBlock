using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealButton : MonoBehaviour
{
    public void Heal(int health)
    {
        PlayerStatEventManager.Instance.AddHealth(health);
    }

    public void BuyHeal(int cost)
    {
        if(Stats.Instance.hp == Stats.Instance.max_hp) {
            Debug.Log("already full health");
            return;
        }
        const int health = 25; // can't pass 2 variables in button func
        int balance = Stats.Instance.money;
        if (balance >= cost)
        {
            Stats.Instance.money -= cost;
            Heal(health);
        }
        else
        {
            Debug.Log("not enough money");
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
