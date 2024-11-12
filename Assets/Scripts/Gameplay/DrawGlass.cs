using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawGlass : MonoBehaviour
{

    private Gameplay gameplay;

    public GameObject blockNoColliderPrefab; // doesn't show info
    public GameObject blockPrefab; // shows info
    public GameObject gridObj;

    public GameObject laserObj;
    public GameObject laserWarningObj;

    public Transform effectParentObj; // instantiate effects here

    [Header("Effect Prefabs")]
    public GameObject lineEffectPrefab;
    public GameObject blockNumberPrefab;

    private GameObject[] blocks;
    private int n;
    private BlockDataManager blockDataManager;
    private GridLayoutGroup gridLayoutGroup;

    private List<List<Block>> glass => gameplay.glassManager.glass;
    private int glass_w => GlassManager.w;
    private int glass_h => GlassManager.h;
    private PieceManager pm => gameplay.pieceManager;

    private void Awake()
    {
        blockDataManager = BlockDataManager.Instance;
        gameplay = gameObject.GetComponent<Gameplay>();
        //PieceStateManager.Instance.OnPieceStateChanged += OnPieceStateChanged;
        PlayerStatEventManager.Instance.OnTurnCountChanged += UpdateLaserWarningVisibility;
    }
    private void OnDestroy()
    {
        //PieceStateManager.Instance.OnPieceStateChanged -= OnPieceStateChanged;
        PlayerStatEventManager.Instance.OnTurnCountChanged -= UpdateLaserWarningVisibility;
    }

    private bool isPieceInAnimation()
    {
        return PieceStateManager.Instance.CurrentPieceState == PieceState.Animation;
    }

    void Start()
    {
        gridLayoutGroup = gridObj.GetComponent<GridLayoutGroup>();

        CreateBlockObjects();
        UpdateLaserPosition();
        UpdateLaserWarningVisibility();
    }

    void UpdateLaserWarningVisibility()
    {
        int turns = Stats.Instance.turn_limit - Stats.Instance.turn_cnt;
        bool vis = turns > 0 && turns <= 3;
        laserWarningObj.transform.localPosition = new Vector3(0, vis ? 0f : 50000f);
    }

    public void UpdateLaserPosition(bool animation = false)
    {
        float y_to = 65 * (gameplay.level.laser - 7);
        if (animation)
        {
            laserObj.transform.LeanMoveLocalY(y_to, 0.8f).setEaseInOutCubic();
        }
        else
        {
            Vector2 apos = laserObj.GetComponent<RectTransform>().anchoredPosition;
            apos.y = y_to;
            laserObj.GetComponent<RectTransform>().anchoredPosition = apos;
        }
    }

    /// <summary>
    /// Converts block coords to position vector.
    /// </summary>
    private Vector2 CoordsToVector(int y, int x)
    {
        float w = gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x;
        float pos_x = (x - 5) * w + (w / 2);
        float pos_y = (y - 7) * w + (w / 2);
        return new Vector2(pos_x, pos_y);
    }

    public void PlayLineClearEffect(int line_y, float energy)
    {
        GameObject go = Instantiate(lineEffectPrefab, effectParentObj);
        Vector2 pos = CoordsToVector(line_y, 0);
        pos.x = 0;
        go.transform.localPosition = pos;
        go.GetComponent<LineClearEffect>().RunAnimation(energy);
    }

    public void PlayBlockEnergyEffect(float energy, int y, int x)
    {
        GameObject go = Instantiate(blockNumberPrefab, effectParentObj);
        Vector2 pos = CoordsToVector(y, x);
        go.transform.localPosition = pos;
        go.GetComponent<BlockEnergyEffect>().RunAnimation(energy);
    }

    public void ShowEnergyNumberOnBlock()
    {

    }
    void CreateBlockObjects()
    {
        foreach (Transform child in gridObj.transform)
        {
            Destroy(child.gameObject);
        }

        n = glass_w * glass_h;
        gridLayoutGroup.constraintCount = glass_w;
        blocks = new GameObject[n];
        for (int i = 0; i < n; i++)
        {
            if (gameplay.started)
            {
                blocks[i] = Instantiate(blockNoColliderPrefab, gridObj.transform);
            }
            else
            {
                blocks[i] = Instantiate(blockPrefab, gridObj.transform);
                blocks[i].GetComponent<BoxCollider2D>().size = gridLayoutGroup.cellSize;
                //if (blocks[i].GetComponent<BlockHintColliderScript>() != null)
                //{
                //    blocks[i].GetComponent<BlockHintColliderScript>().showOnPause = 
                //}
                blocks[i].GetComponent<BoxCollider2D>().size = gridLayoutGroup.cellSize;

            }
        }

        Draw();
    }

    void Update()
    {

    }

    public void Launch()
    {
        CreateBlockObjects();
        Draw();
    }
    public void Draw()
    {

        // glass
        for (int i = 0; i < glass_h; i++)
        {
            for (int j = 0; j < glass_w; j++)
            {
                DrawBlock(i * glass_w + j, glass[i][j]);
            }
        }

        // skip if animaton is running or game not started
        if (!isPieceInAnimation() && gameplay.started)
        {
            // where your piece drop
            int old_y = pm.y;
            pm.MoveDownMax();
            for (int i = 0; i < pm.h; i++)
            {
                for (int j = 0; j < pm.w; j++)
                {
                    if (pm.shape[i][j].type == Block.Type.Empty)
                    {
                        continue;
                    }
                    int y = pm.y + i;
                    int x = pm.x + j;
                    if (x >= 0 && x < glass_w && y >= 0 && y < glass_h && glass[y][x].IsEmpty())
                    {
                        int pos = y * glass_w + x;
                        DrawBlock(pos, new Block(Block.Type.LightEmpty));
                    }
                    //else
                    //{
                    //    if (fg.shape[i][j].type != Block.Special.Empty)
                    //    {
                    //        Debug.LogError("Incorrect block position");
                    //    }
                    //}
                }
            }
            pm.y = old_y;

            // drawing player's piece
            for (int i = 0; i < pm.h; i++)
            {
                for (int j = 0; j < pm.w; j++)
                {

                    if (pm.shape[i][j].type == Block.Type.Empty)
                    {
                        continue;
                    }
                    int y = pm.y + i;
                    int x = pm.x + j;

                    if (x >= 0 && x < glass_w && y >= 0 && y < glass_h)
                    {
                        int pos = y * glass_w + x;
                        if (pm.shape[i][j].type == Block.Type.Ghost)
                        {
                            if (glass[y][x].type == Block.Type.Ghost)
                            {
                                DrawBlock(pos, new Block(Block.Type.Default, 0));
                            }
                            else if (glass[y][x].IsEmpty())
                            {
                                DrawBlock(pos, pm.shape[i][j]);
                            }
                            else
                            {
                                //...
                            }
                        }
                        else
                        {
                            DrawBlock(pos, pm.shape[i][j]);
                        }
                    }
                    //else
                    //{
                    //    if (fg.shape[i][j].type != Block.Special.Empty)
                    //    {
                    //        Debug.LogError("Incorrect block position");
                    //    }
                    //}
                }
            }
        }
    }

    public void PlayDropParticles(int y, int x)
    {
        int pos = y * glass_w + x;
        BlockParticles bp = blocks[pos].GetComponent<BlockParticles>();
        bp.PlayDropParticles();
    }

    private void DrawBlock(int pos, Block b)
    {
        blockDataManager.SetBlock(blocks[pos], b);
    }
}
