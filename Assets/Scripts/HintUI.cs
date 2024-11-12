using System.Collections;
using UnityEngine;
using TMPro;
using Relic = RelicsManager.RelicType;

public class HintUI : MonoBehaviour
{

    [SerializeField] private GameObject _hintPivot;
    [SerializeField] private GameObject _hideOffsetObject;
    [SerializeField] private RectTransform _hintPanelRect;
    [SerializeField] private TextMeshProUGUI _descripton;

    private BlockDataManager _blockDataManager;
    private RelicsManager _relicsManager;
    private GameObject _trackedObj;
    private bool _isVisible = true;

    //private Block _block;
    //private Relic _relic;

    [SerializeField] private float _infoShowDelay;
    [SerializeField] private int _lineHeight;
    [SerializeField] private int _extraHeight;
    [SerializeField] private int _mininalHeight;

    //private Vector3 positionDelta = new Vector2(0f, 1f);
    //private bool isVisible = true;
    [SerializeField] private Vector3 _hideOffset;

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
        _blockDataManager = BlockDataManager.Instance;
        _relicsManager = RelicsManager.Instance;
        SetVisibility(false);
        SetHint(0, new Block(Block.Type.Empty), Relic.Null, null);
    }

    private void Update()
    {
        if (_trackedObj != null && !_trackedObj.activeInHierarchy)
        {
            //Debug.Log("disable hint pls");
            SetHint(0, new Block(Block.Type.Empty), Relic.Null, null);
            UpdateRectSize();
        }
        UpdatePosition();
    }
    void UpdatePosition()
    {
        if (_hintPivot.activeSelf && _trackedObj != null)
        {
            Vector2 pos = _trackedObj.transform.position; // global position of the object on the screen
            Vector2 posPixels = Camera.main.WorldToScreenPoint(pos); // in pixels
            _hintPivot.GetComponent<RectTransform>().position = pos;

            float dw = (int)(_trackedObj.GetComponent<RectTransform>().rect.width * 0.6f);
            float edge = 30f; // min distance between hint and screen border
            // default
            _hintPanelRect.pivot = new Vector2(0.5f, 0f);
            _hintPanelRect.localPosition = new Vector2(0, dw);

            // bottom edge
            if (posPixels.y + dw + _hintPanelRect.rect.height + edge >= Screen.height)
            {
                _hintPanelRect.pivot = new Vector2(0.5f, 1f);
                _hintPanelRect.localPosition = new Vector2(0, -dw);
            }

            float xmin = _hintPanelRect.rect.width / 2f + edge;
            float xmax = Screen.width - xmin;
            float y1 = posPixels.y;
            y1 = Mathf.Min(y1, Screen.height - _hintPanelRect.rect.height / 2f - edge);
            y1 = Mathf.Max(y1, _hintPanelRect.rect.height / 2f + edge);
            y1 -= posPixels.y;

            // right edge
            if (posPixels.x >= xmax)
            {
                _hintPanelRect.pivot = new Vector2(1f, 0.5f);
                _hintPanelRect.localPosition = new Vector2(-dw, y1);
            }
            // left edge
            if (posPixels.x <= xmin)
            {
                _hintPanelRect.pivot = new Vector2(0f, 0.5f);
                _hintPanelRect.localPosition = new Vector2(dw, y1);
            }
        }
    }

    private void UpdateRectSize()
    {
        int height = _descripton.textInfo.lineCount * _lineHeight + _extraHeight; // approx line cnt
        height = Mathf.Max(height, _mininalHeight);
        _hintPanelRect.sizeDelta = new Vector2(_hintPanelRect.sizeDelta.x, height);
    }

    private void SetVisibility(bool value)
    {
        if(value == _isVisible)
        {
            return;
        }
        _isVisible = value;
        
        if (_isVisible)
        {
            _hideOffsetObject.transform.localPosition -= _hideOffset;
        }
        else
        {
            _hideOffsetObject.transform.localPosition += _hideOffset;
        }
    }

    private void SetHint(int state, Block block, Relic relic, GameObject obj)
    {
        StopAllCoroutines();
        SetVisibility(true); // to change data in hint view
        //_block = block;
        //_relic = relic;
        
        string name = "";
        string description = "";

        if (state == 0)
        {
            SetVisibility(false);
            return;
        }
        if (state == 1)
        {
            name = _blockDataManager.GetBlockName(block.type);
            description = _blockDataManager.GetBlockDescription(block.type);
        }
        if (state == 2) {
            name = _relicsManager.GetName(relic);
            description = _relicsManager.GetDescription(relic);
        }

        //blockDataManager.SetBlock(blockObject, block);

        _descripton.text = "<color=white><uppercase>" + name + "</uppercase></color>\n" + description;
        //Debug.Log("1. Lines: " + _descripton.textInfo.lineCount.ToString());

        _trackedObj = obj;

        SetVisibility(false);
        StartCoroutine(waiter(_infoShowDelay));
    }

    IEnumerator waiter(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetVisibility(true);
        UpdateRectSize();
        UpdatePosition();
        //Debug.Log("3. Lines: " + _descripton.textInfo.lineCount.ToString());
    }

}
