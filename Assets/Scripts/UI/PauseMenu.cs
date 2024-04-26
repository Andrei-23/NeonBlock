using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject MenuPanelObj;
    public GameObject InventoryGrid;
    public GameObject PieceViewPrefab;
    //public bool gameScene; // set true if script runs in the gameScene

    private void Awake()
    {
        GameStateManager.Instance.SetState(GameState.Gameplay);
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
        MenuPanelObj.SetActive(false);
    }
    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }
    private void Start()
    {
        int cnt = 0;
        for (int i = 0; i < PieceData.Instance.count; i++)
        {
            int k = Stats.Instance.pieces[i];
            if (k == 0) continue;
            cnt++;
            GameObject go = Instantiate(PieceViewPrefab, InventoryGrid.transform);
            Vector2 cellSize = InventoryGrid.GetComponent<GridLayoutGroup>().cellSize;
            go.GetComponentInChildren<PieceView>().SetSize(cellSize.x, cellSize.y);
            go.GetComponentInChildren<PieceView>().ResetPiece(i);
            go.GetComponentInChildren<TextMeshProUGUI>().text = (k == 1 ? "" : k.ToString());
        }
        int gw = InventoryGrid.GetComponent<GridLayoutGroup>().constraintCount;
        int gh = cnt / gw + ((cnt % gw == 0) ? 0 : 1);
        float prev_h = InventoryGrid.GetComponent<RectTransform>().rect.height;
        InventoryGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(0, gh * 210 + 30 - prev_h);
        //rect.y = gh * 210 + 30;
        //InventoryGrid.GetComponent<RectTransform>().sizeDelta = rect;
    }

    public void PauseGame()
    {
        SetPauseState(true);
    }
    public void UnpauseGame()
    {
        SetPauseState(false);
    }
    private void SetPauseState(bool isPaused)
    {
        GameStateManager.Instance.SetState(isPaused ? GameState.Paused : GameState.Gameplay);
    }

    private void OnGameStateChanged(GameState newGameState)
    {
        bool pause = newGameState == GameState.Paused;
        Debug.Log(pause);
        MenuPanelObj.SetActive(pause);
        enabled = pause;

    }
}