using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceView : MonoBehaviour
{
    public bool lockMaterial = false;
    public GridLayoutGroup grid;
    public GameObject frameObj;
    public GameObject blockPrefub;
    [SerializeField] private bool _fixedFrame = false; // if true, use default frame

    //public Sprite[] sprites;
    //private FigureData fd;

    private float obj_w = 30f;
    private float obj_h = 70f;

    //[HideInInspector] public int id = 0;
    [HideInInspector] public int w = 3;
    [HideInInspector] public int h = 3;
    [HideInInspector] public List<List<Block>> shape;

    public bool showOnPause;
    public bool tryPerfectSize;
    //[HideInInspector] public bool hideOnGame;
    // if false, invisible in gameplay state, even on pause. Use for pause menu and inventory

    private GameObject[] blocks;

    private BlockDataManager blockDataManager;
    private float widthMult = 0.88f;
    //private int cur_color = 0;


    private void Awake()
    {
        blockDataManager = BlockDataManager.Instance;
        SetDefaultRectSize();
    }

    private void Start()
    {

    }

    // set piece width proportional to the rect width
    public void SetDefaultRectSize()
    {
        obj_w = GetComponent<RectTransform>().rect.width * widthMult;
        obj_h = GetComponent<RectTransform>().rect.height * widthMult;
    }
    
    // set custom size of piece (e.g. when rect is inside gridLayout)
    public void SetSize(float width, float height)
    {
        //obj_w = width;
        //obj_h = height;
        obj_w = width * widthMult;
        obj_h = height * widthMult;
    }

    public void ResetPiece(int id)
    {
        ResetPiece(PieceData.Instance.GetPiece(id));
    }
    //private void ResetPiece(List<List<Block>> shape_)
    //{
    //    if (blockDataManager == null)
    //    {
    //        blockDataManager = BlockDataManager.Instance;
    //    }
    //    shape = shape_;
    //    h = shape.Count;
    //    w = shape[0].Count;
        
    //    UpdateView();
    //}
    public void ResetPiece(Piece piece)
    {
        if (blockDataManager == null)
        {
            blockDataManager = BlockDataManager.Instance;
        }
        if (piece == null)
        {
            RemovePiece();
            return;
        }

        shape = piece.shape;
        h = shape.Count;
        w = shape[0].Count;

        if(frameObj != null && !_fixedFrame)
        {
            if (!lockMaterial)
            {
                frameObj.GetComponent<Image>().material = RarityMaterials.Instance.GetRarityFrameMaterial(piece.rarity);
            }
            frameObj.GetComponent<Image>().sprite = RarityMaterials.Instance.GetRarityFrameSprite(piece.rarity);
        }
        UpdateView();
    }

    private void UpdateView()
    {

        float w0 = 34f;
        float cell_w = w0;
        if(!tryPerfectSize || w >= 4 || cell_w * w > Mathf.Min(obj_w, obj_h))
        {
            cell_w = Mathf.Min(obj_w, obj_h) / w;
        }
        else
        {
            for(int i = 2; i < 5; i++)
            {
                if(w0 * i * w < Mathf.Min(obj_w, obj_h))
                {
                    cell_w = w0 * i;
                }
            }
        }

        grid.constraintCount = w;
        grid.spacing = new Vector2(cell_w, cell_w) / 17f;
        grid.cellSize = grid.spacing * 16f;
        //grid.cellSize = new Vector2(Mathf.Min(obj_w / w, obj_w * 0.4f), Mathf.Min(obj_h / h, obj_h * 0.4f));

        foreach (Transform child in grid.transform)
        {
            Destroy(child.gameObject);
        }

        blocks = new GameObject[w * h];
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                int pos = i * w + j;
                blocks[pos] = Instantiate(blockPrefub, grid.transform);
                if (blocks[pos].GetComponent<BlockHintColliderScript>() != null)
                {
                    blocks[pos].GetComponent<BlockHintColliderScript>().showOnPause = showOnPause;
                    //blocks[pos].GetComponent<BlockHintColliderScript>().hideOnGame = hideOnGame;
                }
                BoxCollider2D bc;
                if (blocks[pos].TryGetComponent(out bc))
                {
                    bc.size = grid.cellSize;
                }
                DrawBlock(pos, shape[i][j]);
            }
        }
    }

    public void RemovePiece()
    {
        if (blockDataManager == null)
        {
            blockDataManager = BlockDataManager.Instance;
        }
        h = w = 1;
        grid.constraintCount = w;

        foreach (Transform child in grid.transform)
        {
            Destroy(child.gameObject);
        }

        blocks = new GameObject[1];
    }

    private void DrawBlock(int pos, Block b)
    {
        blockDataManager.SetBlock(blocks[pos], b);
    }

    void Update()
    {
        
    }
}
