using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Relic = RelicsManager.RelicType;


public class RelicView : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI valueText;
    
    [HideInInspector] public Relic type;

    private void Awake()
    {
        RelicEvents.onRelicInfoUpdate += UpdateView;
    }
    private void OnDestroy()
    {
        RelicEvents.onRelicInfoUpdate -= UpdateView;
    }
    void Start()
    {
        //UpdateView();
    }

    // Change views with matching relic type
    public void UpdateView(Relic changed_type)
    {
        if (changed_type != type) return;
        UpdateView();
    }
    public void UpdateView()
    {
        GetComponent<RelicHintColliderScript>().relic = type;
        GetComponent<BoxCollider2D>().size = GetComponent<RectTransform>().rect.size;

        image.sprite = RelicsManager.Instance.GetSprite((int)type);

        // skip if no text attached
        if(countText != null)
        {
            int x = RelicsManager.Instance.GetCount(type);
            countText.text = (x == 1 ? string.Empty : x.ToString());
        }

        if(valueText != null)
        {
            int x = RelicsManager.Instance.GetValue(type);
            if (RelicsManager.Instance.relicData[(int)type].show_value)
            {
                valueText.text = x.ToString();
            }
            else
            {
                valueText.text = string.Empty;
            }
        }
    }
    public void SetView(int newType)
    {
        SetView((Relic)newType);
    }
    public void SetView(Relic newType)
    {
        type = newType;
        UpdateView();
    }
} 
