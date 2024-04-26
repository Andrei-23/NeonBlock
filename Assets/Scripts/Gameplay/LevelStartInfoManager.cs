using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelStartInfoManager : MonoBehaviour
{
    private Gameplay gameplay;
    public GameObject PieceViewPrefab;
    public GameObject StartUIPanel;
    public GameObject GameplayUIPanel;
    public GameObject AddedPiecesPanel;
    public GameObject AddedPiecesGrid;
    public GameObject ClearFromTopWarning;
    public TextMeshProUGUI EnergyTask;

    private void Awake()
    {
        gameplay = GetComponent<Gameplay>();
    }
    void Start()
    {
        SetGameplayActive(false);
        SetAddedPiecesList();
        EnergyTask.text = "<color=white>Energy: </color>" + Stats.Instance.energy_task.ToString();
        ClearFromTopWarning.SetActive(gameplay.CheckDeleteDirectionTop());
    }

    public void Launch()
    {
        SetGameplayActive(true);
    }
    void SetGameplayActive(bool value)
    {
        StartUIPanel.SetActive(!value);
        GameplayUIPanel.SetActive(value);
    }
    void SetAddedPiecesList()
    {
        int cnt = gameplay.level.pieces.Count;
        AddedPiecesPanel.SetActive(cnt > 0);
        if (cnt > 0)
        {
            foreach (Piece np in gameplay.level.pieces)
            {
                GameObject go = Instantiate(PieceViewPrefab, AddedPiecesGrid.transform);
                Vector2 cellSize = AddedPiecesGrid.GetComponent<GridLayoutGroup>().cellSize;
                go.GetComponentInChildren<PieceView>().SetSize(cellSize.x, cellSize.y);
                go.GetComponentInChildren<PieceView>().ResetPiece(np);
                go.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.7f);
                // go.GetComponentInChildren<TextMeshProUGUI>().text = (k == 1 ? "" : k.ToString());
            }
            int gw = AddedPiecesGrid.GetComponent<GridLayoutGroup>().constraintCount;
            int gh = cnt / gw + ((cnt % gw == 0) ? 0 : 1);
            float rect_h = AddedPiecesGrid.GetComponent<RectTransform>().rect.height;
            AddedPiecesGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(0, gh * 210 + 30 - rect_h);
        }
    }
}
