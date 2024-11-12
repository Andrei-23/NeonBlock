using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.LookDev;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject MenuPanelObj;
    [SerializeField] private GameObject InventoryGrid;
    [SerializeField] private GameObject PieceViewPrefab;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI _lineEnergyValue;
    //[SerializeField] private TextMeshProUGUI _defaultComboValue;
    [SerializeField] private TextMeshProUGUI _increaseComboValue;
    [SerializeField] private TextMeshProUGUI _pieceCountValue;

    private Vector2 _initialInventorySizeDelta;

    private void Awake()
    {
        GameStateManager.Instance.OnPauseStateChanged += OnPauseStateChanged;
        PlayerStatEventManager.Instance.OnInventoryChanged += UpdateInventoryPieces;
        MenuPanelObj.SetActive(false);
    }
    private void OnDestroy()
    {
        GameStateManager.Instance.OnPauseStateChanged -= OnPauseStateChanged;
        PlayerStatEventManager.Instance.OnInventoryChanged -= UpdateInventoryPieces;
    }
    private void Start()
    {
        _initialInventorySizeDelta = InventoryGrid.GetComponent<RectTransform>().sizeDelta;
        UpdateInventoryPieces();
    }

    private void UpdateInventoryPieces()
    {
        foreach(Transform child in InventoryGrid.transform)
        {
            Destroy(child.gameObject);
        }
        int cells = 0;
        for (int i = PieceData.Instance.count - 1; i >= 0; i--)
        {
            int k = Stats.Instance.pieces[i];
            if (k == 0) continue;
            cells++;
            GameObject go = Instantiate(PieceViewPrefab, InventoryGrid.transform);
            Vector2 cellSize = InventoryGrid.GetComponent<GridLayoutGroup>().cellSize;
            go.GetComponentInChildren<PieceView>().SetSize(cellSize.x, cellSize.y);
            go.GetComponentInChildren<PieceView>().ResetPiece(i);
            go.GetComponentInChildren<PieceView>().showOnPause = true;
            go.GetComponentInChildren<TextMeshProUGUI>().text = (k == 1 ? "" : k.ToString());
        }

        // size in cells
        int gw = InventoryGrid.GetComponent<GridLayoutGroup>().constraintCount;
        int gh = cells / gw + ((cells % gw == 0) ? 0 : 1);

        GridLayoutGroup glg = InventoryGrid.GetComponent<GridLayoutGroup>();
        float yPerCell = glg.cellSize.y + glg.spacing.y;
        float sdy = gh * yPerCell + _initialInventorySizeDelta.y + 30;
        InventoryGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(0, sdy);

        //float prev_h = InventoryGrid.GetComponent<RectTransform>().rect.height;
        //InventoryGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(0, gh * 210 + 30 - prev_h);
        //rect.y = gh * 210 + 30;
        //InventoryGrid.GetComponent<RectTransform>().sizeDelta = rect;
    }

    private void Update()
    {
        _lineEnergyValue.text = Stats.Instance.GetDefaultlineCost().ToString();
        _increaseComboValue.text = Stats.Instance.combo_mult.ToString();
        _pieceCountValue.text = Stats.Instance.piece_count.ToString();
    }
    public void PauseGame()
    {
        SetPauseState(true);
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void UnpauseGame()
    {
        SetPauseState(false);
        EventSystem.current.SetSelectedGameObject(null);
    }
    private void SetPauseState(bool isPaused)
    {
        GameStateManager.Instance.SetPauseState(isPaused);
    }

    private void OnPauseStateChanged(bool is_paused)
    {
        bool pause = is_paused;
        Debug.Log("Pause: " + pause);
        MenuPanelObj.SetActive(pause);
        enabled = pause;

    }
}