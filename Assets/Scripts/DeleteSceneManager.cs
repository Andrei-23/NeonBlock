using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeleteSceneManager : MonoBehaviour
{
    public GameObject PieceViewPrefab;
    public GameObject InventoryGrid;
    public GameObject NotEnoughText;

    private void Start()
    {
        int cnt = Stats.Instance.piece_count;
        if(cnt <= 1)
        {
            NotEnoughText.SetActive(true);
            return;
        }
        for (int i = 0; i < PieceData.Instance.count; i++)
        {
            int k = Stats.Instance.pieces[i];
            if (k == 0) continue;
            for (int j = 0; j < k; j++)
            {
                GameObject go = Instantiate(PieceViewPrefab, InventoryGrid.transform);
                Vector2 cellSize = InventoryGrid.GetComponent<GridLayoutGroup>().cellSize;
                go.GetComponentInChildren<PieceView>().SetSize(cellSize.x, cellSize.y);
                go.GetComponentInChildren<PieceView>().ResetPiece(i);
                //go.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.7f);
                go.GetComponent<DeleteOption>().piece_id = i;
            }
        }
        int gw = InventoryGrid.GetComponent<GridLayoutGroup>().constraintCount;
        int gh = cnt / gw + ((cnt % gw == 0) ? 0 : 1);
        float prev_h = InventoryGrid.GetComponent<RectTransform>().rect.height;
        InventoryGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(0, gh * 210 + 30 - prev_h);
    }
}
