using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlockEnergyEffect : MonoBehaviour
{
    public GameObject obj; // script should be in this obj

    public GameObject energyObj;
    public TextMeshProUGUI energyText;

    public float duration;
    public int dy;

    private string EnergyFormat(float val)
    {
        int x = (int)(val * 10);
        if(x % 10 == 0)
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
        energyText.text = EnergyFormat(energy);
        RectTransform ert = energyObj.GetComponent<RectTransform>();
        LeanTween.value(energyObj, updateAlpha, 1f, 0f, duration);
        LeanTween.moveLocalY(energyObj, ert.localPosition.y + dy, duration).setEaseOutQuad().setOnComplete(StopAnimation);
    }

    void updateAlpha(float alpha)
    {
        energyText.alpha = alpha;
    }

    public void StopAnimation()
    {
        Destroy(gameObject); 
    }
}
