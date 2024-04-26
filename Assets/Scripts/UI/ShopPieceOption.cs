using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Relic = RelicsManager.RelicType;

public class ShopPieceOption : MonoBehaviour
{
    //private UpgradeManager um;
    //private FigureData fd;

    //public GridLayoutGroup gl;

    //public Color commonColor;
    //public Color rareColor;
    //public Color epicColor;
    //public Color legendaryColor;
    //public Color badColor;
    //public Color specialColor;

    public PieceView pv;
    public TextMeshProUGUI costText;

    [HideInInspector] public int default_cost;
    [HideInInspector] public int cost;
    [HideInInspector] public int figure_id;

    void Start()
    {
        Generate();
    }
    private void Awake()
    {
        RelicEvents.onRelicActive += OnRelicActive;
    }
    private void OnDestroy()
    {
        RelicEvents.onRelicActive -= OnRelicActive;
    }
    private void OnRelicActive(Relic type)
    {
        switch(type)
        {
            case Relic.Discount: UpdateCost(); break;
        }
    }
    private void UpdateCost()
    {
        CountCost();
        costText.text = cost.ToString();
    }
    private void CountCost()
    {
        cost = default_cost;
        float mult = 1.0f;
        if (RelicsManager.Instance.IsActive(Relic.Discount))
        {
            int count = RelicsManager.Instance.GetCount(Relic.Discount);
            mult *= Mathf.Pow(0.9f, count);
        }
        if (mult != 1f)
        {
            cost = (int)(cost * mult);
        }
    }
    public void Generate()
    {

        PieceData.Rarity rarity = PieceData.Instance.GetRandomRarity();
        figure_id = PieceData.Instance.GetRandomPieceId(rarity);
        pv.ResetPiece(figure_id);

        int min_cost = 1, max_cost = 10;
        switch (rarity)
        {
            case PieceData.Rarity.Common:
                min_cost = 20;
                max_cost = 50;
                break;
            case PieceData.Rarity.Rare:
                min_cost = 40;
                max_cost = 80;
                break;
            case PieceData.Rarity.Epic:
                min_cost = 90;
                max_cost = 150;
                break;
            case PieceData.Rarity.Legendary:
                min_cost = 140;
                max_cost = 220;
                break;
            case PieceData.Rarity.Bad:
                min_cost = 20;
                max_cost = 50;
                break;
            case PieceData.Rarity.Special:
                min_cost = 50;
                max_cost = 100;
                break;
        }
        default_cost = UnityEngine.Random.Range(min_cost, max_cost);
        CountCost();
        costText.text = cost.ToString();
    }
    public void Buy()
    {
        int balance = Stats.Instance.money;
        if (balance >= cost)
        {
            Stats.Instance.money -= cost;
            Stats.Instance.AddPiece(figure_id);
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("not enough money");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
