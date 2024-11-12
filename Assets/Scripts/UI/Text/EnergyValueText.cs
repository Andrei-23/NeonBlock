using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnergyValueText : MonoBehaviour
{
    public TextMeshProUGUI hpText;
    public Color defaultColor;
    public Color increaseColor;
    public Color completeColor;
    public float duration;

    private RectTransform hp_rt;

    private void Awake()
    {
        PlayerStatEventManager.Instance.OnEnergyIncrease += OnEnergyIncrease;
    }
    private void OnDestroy()
    {
        PlayerStatEventManager.Instance.OnEnergyIncrease -= OnEnergyIncrease;
    }
    private void Start()
    {
        hp_rt = hpText.GetComponent<RectTransform>();
        hpText.color = defaultColor;
        UpdateValue();
    }
    void UpdateValue()
    {
        int x = (int)(10 * Stats.Instance.energy);
        hpText.text = (x / 10f).ToString() + "/<color=white><size=40>" + Stats.Instance.energy_task.ToString();
    }

    void Update()
    {
        if(Stats.Instance.energy >= Stats.Instance.energy_task)
        {
            LeanTween.cancel(hpText.gameObject);
            hpText.color = completeColor;
        }
    }

    void OnEnergyIncrease(int amount)
    {
        hpText.color = increaseColor;
        LeanTween.value(hpText.gameObject, updateColor, increaseColor, defaultColor, duration); 
        UpdateValue();
    }
    void updateColor(Color color)
    {
        hpText.color = color;
    }

}
