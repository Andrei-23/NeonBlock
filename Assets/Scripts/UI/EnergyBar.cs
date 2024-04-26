using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnergyBar : MonoBehaviour
{
    public GameObject energyBar;
    public Color fullColor;
    private void Awake()
    {
        PlayerStatEventManager.Instance.OnEnergyIncrease += OnValueIncreased;
    }
    private void OnDestroy()
    {
        PlayerStatEventManager.Instance.OnEnergyIncrease -= OnValueIncreased;
    }
    void OnValueIncreased(int amount)
    {
        float h = Mathf.Min(1, (float)Stats.Instance.energy / Stats.Instance.energy_task);
        energyBar.transform.LeanScaleY(h, 0.5f).setEaseOutQuad();
        if((float)Stats.Instance.energy >= Stats.Instance.energy_task)
        {
            energyBar.GetComponent<Image>().color = fullColor;
        }
    }
}
