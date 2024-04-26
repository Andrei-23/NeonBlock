using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Relic = RelicsManager.RelicType;

public class HintUI : MonoBehaviour
{
    //[SerializeField] private GameObject blockObject;
    private TextMeshProUGUI descripton;

    public GameObject HintPivot;
    private GameObject HintPanel;
    private BlockDataManager blockDataManager;
    private RelicsManager relicsManager;
    private GameObject trackedObj;

    private Block block;
    private Relic relic;

    private float infoShowDelay = 0.4f;
    //private Vector3 positionDelta = new Vector2(0f, 1f);
    //private bool isVisible = true;
    //private Vector3 hideVector = new Vector3(0, 10000);

    private void Awake()
    {
        HintEventManager.Instance.OnHintChange += SetHint;
    }
    private void OnDestroy()
    {
        HintEventManager.Instance.OnHintChange -= SetHint;
    }
    void Start()
    {
        HintPanel = HintPivot.transform.GetChild(0).gameObject;
        descripton = HintPanel.GetComponentInChildren<TextMeshProUGUI>();

        blockDataManager = BlockDataManager.Instance;
        relicsManager = RelicsManager.Instance;
        SetHint(0, new Block(Block.Type.Empty), Relic.Null, null);
    }

    private void Update()
    {
        if (trackedObj != null && !trackedObj.activeInHierarchy)
        {
            Debug.Log("disable hint pls");
            SetHint(0, new Block(Block.Type.Empty), Relic.Null, null);
        }
        UpdatePosition();   
    }
    void UpdatePosition()
    {
        if (HintPivot.activeSelf && trackedObj != null)
        {
            Vector2 pos = trackedObj.transform.position; // global position of the object on the screen
            Vector2 posPixels = Camera.main.WorldToScreenPoint(pos); // in pixels
            HintPivot.GetComponent<RectTransform>().position = pos;

            RectTransform panelRT = HintPanel.GetComponent<RectTransform>();
            float dw = (int)(trackedObj.GetComponent<RectTransform>().rect.width * 0.6f);
            float edge = 30f; // min distance between hint and screen border
            // default
            panelRT.pivot = new Vector2(0.5f, 0f);
            panelRT.localPosition = new Vector2(0, dw);

            // bottom edge
            if (posPixels.y + dw + panelRT.rect.height + edge >= Screen.height)
            {
                panelRT.pivot = new Vector2(0.5f, 1f);
                panelRT.localPosition = new Vector2(0, -dw);
            }

            float xmin = panelRT.rect.width / 2f + edge;
            float xmax = Screen.width - xmin;
            float y1 = posPixels.y;
            y1 = Mathf.Min(y1, Screen.height - panelRT.rect.height / 2f - edge);
            y1 = Mathf.Max(y1, panelRT.rect.height / 2f + edge);
            y1 -= posPixels.y;

            // right edge
            if (posPixels.x >= xmax)
            {
                panelRT.pivot = new Vector2(1f, 0.5f);
                panelRT.localPosition = new Vector2(-dw, y1);
            }
            // left edge
            if (posPixels.x <= xmin)
            {
                panelRT.pivot = new Vector2(0f, 0.5f);
                panelRT.localPosition = new Vector2(dw, y1);
            }

            //Debug.Log(pos);
            //Debug.Log(posPixels);
            //Debug.Log(dw);
            //Debug.Log(panelRT.rect.width);
            //Debug.Log(panelRT.position);
            //Debug.Log(panelRT.localPosition);
            //Debug.Log(Screen.width);
        }
    }
    private void SetInfoActive(bool value)
    {
        //if (isVisible == value) return;
        //isVisible = value;
        //if (value) InfoPanel.transform.position -= hideVector;
        //else InfoPanel.transform.position += hideVector;
        HintPivot.SetActive(value);
    }

    private void SetHint(int state, Block block, Relic relic, GameObject obj)
    {
        StopAllCoroutines();
        SetInfoActive(true); // to change data in hint view
        this.block = block;
        this.relic = relic;
        
        string name = "";
        string description = "";

        if (state == 0)
        {
            SetInfoActive(false);
            return;
        }
        if (state == 1)
        {
            name = blockDataManager.GetBlockName(block.type);
            description = blockDataManager.description[(int)block.type];
        }
        if (state == 2) {
            name = relicsManager.GetName(relic);
            description = relicsManager.GetDescription(relic);
        }

        //blockDataManager.SetBlock(blockObject, block);

        descripton.text = "<color=white><uppercase>" + name + "</uppercase></color>\n" + description;
        trackedObj = obj;

        SetInfoActive(false);
        StartCoroutine(waiter(infoShowDelay));
    }

    /*
    private void SetBlock(Block block, GameObject obj)
    {
        StopAllCoroutines();
        SetInfoActive(true);
        string name = blockDataManager.GetBlockName(block.type);
        if (this.block != block)
        {
            //Debug.Log("Selected block changed to \"" + name +"\".");
        }
        this.block = block;
        //blockDataManager.SetBlock(blockObject, block);
        descripton.text = "<color=white>" + name + "</color>\n" + blockDataManager.description[(int)block.type];
        trackedObj = obj;

        SetInfoActive(false);
        if (block.type != Block.Type.Empty)
        {
            StartCoroutine(waiter(infoShowDelay));
        }
    }
    */

    IEnumerator waiter(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetInfoActive(true);
        UpdatePosition();
    }

}
