using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoosePieceOption : MonoBehaviour
{
    //private UpgradeManager um;
    //private FigureData fd;

    //public GridLayoutGroup gl;

    //public PieceData.Rarity rarity; // can do all options epic or bad
    public bool isBad;
    public PieceView pv;
    public GameObject edgePanel;
    private SceneSwitcher sceneSwitcher;

    [HideInInspector] public int figure_id;

    private void Awake()
    {
        sceneSwitcher = Camera.main.GetComponent<SceneSwitcher>();
    }
    void Start()
    {

    }
    
    public void Generate()
    {
        PieceData.Rarity rarity = isBad ? PieceData.Rarity.Bad : PieceData.Instance.GetRandomRarity();
        figure_id = PieceData.Instance.GetRandomPieceId(rarity);
        pv.ResetPiece(figure_id);

        Color edge_color = PieceData.Instance.GetRarityColor(rarity);
        edgePanel.GetComponent<Image>().color = edge_color;
    }
    public void Choose()
    {
        Stats.Instance.AddPiece(figure_id);
        AudioManager.Instance.PlaySound(AudioManager.Instance.selectItem);
        sceneSwitcher.OpenMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
