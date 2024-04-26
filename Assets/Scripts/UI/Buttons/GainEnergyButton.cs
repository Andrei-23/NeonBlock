using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainEnergyButton : MonoBehaviour
{
    public void OnClick(int amount)
    {
        PlayerStatEventManager.Instance.GainEnergy(amount);
    } 
}
