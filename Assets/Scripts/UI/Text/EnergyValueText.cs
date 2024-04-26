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

    private float fade_delay = 0.5f;
    private float fade_timer = 1f;

    private void Awake()
    {
        PlayerStatEventManager.Instance.OnEnergyIncrease += OnDamage;
    }
    private void OnDestroy()
    {
        PlayerStatEventManager.Instance.OnEnergyIncrease -= OnDamage;
    }
    private void Start()
    {
        hpText.text = Stats.Instance.energy.ToString() + "/" + Stats.Instance.energy_task.ToString();
    }
    void Update()
    {
        if(Stats.Instance.energy >= Stats.Instance.energy_task)
        {
            hpText.color = completeColor;
        }
        else
        {
            Color c = increaseColor + (defaultColor - increaseColor) * Mathf.Min(1f, fade_timer / fade_delay);
            hpText.color = c;
            fade_timer += Time.deltaTime;
        }
    }

    void OnDamage(int amount)
    {
        hpText.text = Stats.Instance.energy.ToString() + "/<color=white><size=40>" + Stats.Instance.energy_task.ToString();
        fade_timer = 0f;
    }
}
