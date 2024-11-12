using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class LineClearEffect : MonoBehaviour
{
    public GameObject obj; // script should be in this obj

    public GameObject energyObj;
    public TextMeshProUGUI energyText;

    public float lineDuration;
    public float textDuration;

    private int complete_cnt = 0;
    private string EnergyFormat(float val)
    {
        int x = (int)(val * 10);
        if (x % 10 == 0)
        {
            return (x / 10).ToString();
        }
        else
        {
            return val.ToString("0.0");
        }
    }
    public void RunAnimation(float energy)
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector3 size_start = rt.sizeDelta;
        Vector3 size_delta = new Vector3(-32, 0, 0);

        LeanTween.size(rt, size_start, 0f);
        LeanTween.size(rt, size_start + size_delta, lineDuration);
        LeanTween.alpha(rt, 1f, 0f);
        LeanTween.alpha(rt, 0f, lineDuration).setOnComplete(StopAnimation);
        
        energyText.text = EnergyFormat(energy);
        RectTransform ert = energyObj.GetComponent<RectTransform>();
        LeanTween.value(energyObj, updateAlpha, 1f, 0f, textDuration);
        LeanTween.moveLocalX(energyObj, ert.localPosition.x - 64, textDuration).setEaseOutQuad().setOnComplete(StopAnimation);
    }

    void updateAlpha(float alpha)
    {
        energyText.alpha = alpha;
    }

    public void StopAnimation()
    {
        complete_cnt++;
        if (complete_cnt == 2)
        {
            Destroy(gameObject);
        }
    }
}
