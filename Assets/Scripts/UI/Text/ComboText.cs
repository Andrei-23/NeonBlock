using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComboText : MonoBehaviour
{
    public GameObject panel; // this object
    public TextMeshProUGUI text; // combo text

    public float min_font_size;
    public float max_font_size;
    public float min_size_combo;
    public float max_size_combo;

    private bool isVisible = false;
    //private float cur_val = 0f;

    
    private void Awake()
    {
        PlayerStatEventManager.Instance.OnComboChange += UpdateTextAnim;
    }
    private void OnDestroy()
    {
        PlayerStatEventManager.Instance.OnComboChange -= UpdateTextAnim;
    }

    void Start()
    {
        UpdateText(false);
        LeanTween.value(text.gameObject, updateTextAlphaValue, 0f, 0f, 0f);
    }

    private void SetVisibility(bool v)
    {
        if (isVisible == v) return;
        isVisible = v;

        // add hide anim
        float a0 = text.alpha;
        if (isVisible)
        {
            LeanTween.value(text.gameObject, updateTextAlphaValue, a0, 1f, 0f);
        }
        else
        {
            LeanTween.value(text.gameObject, updateTextAlphaValue, a0, 0f, 0.4f);
        }
    }
    void updateTextAlphaValue(float alpha)
    {
        text.alpha = alpha;
    }

    public void UpdateTextAnim()
    {
        UpdateText(true);
    }
    public void UpdateText(bool animate)
    {
        int x = (int)Mathf.Round(Stats.Instance.combo * 10f);
        if (x != 0)
        {
            string val = (x / 10).ToString() + " . " + (x % 10).ToString();
            text.text = "<color=white>x</color>" + val;

            float v = Mathf.InverseLerp(min_size_combo, max_size_combo, Stats.Instance.combo);
            text.fontSize = Mathf.Lerp(min_font_size, max_font_size, v);
        }

        SetVisibility(x > 0);

        if (animate)
        {
            if (x <= 0)
            {
                LeanTween.scale(text.GetComponent<RectTransform>(), Vector3.one * 0.8f, 0.4f);
            }
            else
            {
                LeanTween.scale(text.GetComponent<RectTransform>(), Vector3.one * 1.05f, 0f);
                LeanTween.scale(text.GetComponent<RectTransform>(), Vector3.one, 0.2f).setEaseOutQuad();
            }
        }
    }
}
