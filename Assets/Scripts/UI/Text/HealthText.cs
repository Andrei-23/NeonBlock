using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthText : MonoBehaviour
{
    public TextMeshProUGUI hpText;
    public Color defaultColor;
    public Color damageColor;
    public Color healColor;

    [SerializeField] private Color curColor;
    private float fade_delay = 0.5f;
    private float fade_timer = 1f;

    private void Awake()
    {
        PlayerStatEventManager.Instance.OnHealthIncrease += OnHeal;
        PlayerStatEventManager.Instance.OnHealthDecrease += OnDamage;
        curColor = defaultColor;
    }
    private void OnDestroy()
    {
        PlayerStatEventManager.Instance.OnHealthIncrease -= OnHeal;
        PlayerStatEventManager.Instance.OnHealthDecrease -= OnDamage;
    }
    void Update()
    {
        hpText.text = Stats.Instance.hp.ToString();
        Color c = curColor + (defaultColor - curColor) * Mathf.Min(1f, fade_timer / fade_delay);
        hpText.color = c;
        fade_timer += Time.deltaTime;
    }

    void OnHeal(int amount)
    {
        curColor = healColor;
        fade_timer = 0f;
    }
    void OnDamage(int amount)
    {
        curColor = damageColor;
        fade_timer = 0f;
    }
}
