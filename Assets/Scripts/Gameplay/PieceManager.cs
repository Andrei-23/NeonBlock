using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Linq;
using Relic = RelicsManager.RelicType;
using System;

/// <summary>
/// This script controls generation and movement of player's pieces.
/// </summary>
public class PieceManager : MonoBehaviour
{
    private Gameplay gameplay;

    public PieceView[] next_pv; // views for next pieces
    public PieceView hold_pv; // view for hold piece

    //[HideInInspector] public int id = -1; // id of current piece
    [HideInInspector] public Piece piece = null; // current piece

    [HideInInspector] public int x = 0, y = 0; // pos of left bottom block
    [HideInInspector] public int w = 0, h = 0; // size of piece
    [HideInInspector] public int rot = 0;
    [HideInInspector] public bool isFixed = false; // true <=> can't rotate this piece

    [HideInInspector] public int nextCount; // = 3, amount of shown next pieces
    [HideInInspector] public Queue<Piece> nextPieces; // ids of next pieces
    [HideInInspector] public List<List<Block>> shape; // current piece with rotation

    private List<PieceInfo> pieces_sorted;
    private List<PieceInfo> pieces_shuffled;
    private int shuffle_cur_id;

    private Piece holdPiece = null; // id of held piece, -1 means no piece
    private bool holdUsed = false;

    private bool initial_reset = false;
    private List<List<Block>> glass => gameplay.glassManager.glass;
    private int glass_w => GlassManager.w;
    private int glass_h => GlassManager.h;
    
    private class PieceInfo
    {
        public bool isPlayerPiece = true; // false - from level, true - from inventory
        public int id = 0; // id in PieceData / id in level pieces list
        public PieceInfo(bool isPlayerPiece, int id)
        {
            this.id = id;
            this.isPlayerPiece = isPlayerPiece;
        }
    }
    public bool InBounds(int y, int x)
    {
        return x >= 0 && x < glass_w && y >= 0 && y < glass_h;
    }

    private void Awake()
    {
        RelicEvents.onRelicActive += OnRelicActive;
        gameplay = Camera.main.GetComponent<Gameplay>();

        pieces_sorted = new List<PieceInfo>(Stats.Instance.piece_count);
        pieces_shuffled = new List<PieceInfo>(Stats.Instance.piece_count);
        for(int i = 0; i < Stats.Instance.piece_count; i++)
        {
            pieces_sorted.Add(new PieceInfo (true, i));
        }
        for (int i = 0; i < gameplay.level.pieces.Count; i++)
        {
            pieces_sorted.Add(new PieceInfo (false, i));
        }

        nextCount = next_pv.Length;
        nextPieces = new Queue<Piece>(nextCount);
        for (int i = 0; i < nextCount; i++)
        {
            AddPieceInQueue();
        }
        foreach(PieceView pv in next_pv)
        {
            pv.SetDefaultRectSize();
        }
    }

    public void Launch()
    {
        SetNextQueueVisibility();
    }
    private void OnDestroy()
    {
        RelicEvents.onRelicActive -= OnRelicActive;
    }
    void Start()
    {
        CheckInitialReset();
    }
    private void CheckInitialReset()
    {
        if (!initial_reset)
        {
            initial_reset = true;
            ResetPiece();
        }
    }

    private void OnRelicActive(Relic type)
    {
        switch(type)
        {
            case Relic.Trashcan: RemoveHoldPiece(); break;
        }
    }

    private void SetNextQueueVisibility()
    {
        int amount = 1 + RelicsManager.Instance.GetCount(Relic.Telescope);
        if(amount > next_pv.Length)
        {
            Debug.LogWarning("Not enough pieces ini next queue");
        }
        for(int i = 0; i < next_pv.Length; i++)
        {
            next_pv[i].gameObject.SetActive(i < amount);
        }
        UpdateViews();
    }

    private void AddPieceInQueue()
    {
        if(shuffle_cur_id >= pieces_shuffled.Count || pieces_shuffled.Count == 0)
        {
            System.Random rand = new System.Random();
            pieces_shuffled = pieces_sorted.OrderBy(_ => rand.Next()).ToList();
            shuffle_cur_id = 0;
        }

        int id = pieces_shuffled[shuffle_cur_id].id;
        if (pieces_shuffled[shuffle_cur_id].isPlayerPiece)
        {
            int stats_id = Stats.Instance.GetPieceByPos(id);
            nextPieces.Enqueue(PieceData.Instance.GetPiece(stats_id));
        }
        else
        {
            nextPieces.Enqueue(gameplay.level.pieces[id].Clone());
        }
        shuffle_cur_id++;
    }

    public bool HoldPiece()
    {
        if (holdUsed)
        {
            if (!RelicsManager.Instance.IsActive(Relic.CloneMachine) || RelicsManager.Instance.GetValue(Relic.CloneMachine) == 0)
            {
                return false;
            }
            if(holdPiece == null)
            {
                Debug.LogWarning("No piece held");
                return false;
            }

            RelicsManager.Instance.SetValue(Relic.CloneMachine, 0);

            piece = holdPiece;
            SetPiece(piece);
            UpdateViews();
            ResetPosition();
            RelicEvents.Instance.HoldPiece();

            return true;
        }

        if(holdPiece == null)
        {
            holdPiece = piece.Clone();
            PlayerStatEventManager.Instance.GainEnergy(holdPiece.CountBlocks(Block.Type.SaveBlock) * 5);
            ResetPiece();
            UpdateViews();
            RelicEvents.Instance.HoldPiece();

            holdUsed = true; // after resetpiece
            return true;
        }
        // cant swap held pieces if has battery relic
        else if(!RelicsManager.Instance.IsActive(Relic.BatterySlot))
        {
            (piece, holdPiece) = (holdPiece, piece);
            PlayerStatEventManager.Instance.GainEnergy(holdPiece.CountBlocks(Block.Type.SaveBlock) * 5);
            SetPiece(piece);
            UpdateViews();
            ResetPosition();
            RelicEvents.Instance.HoldPiece();

            holdUsed = true;
            return true;
        }
        return false;
    }
    public void RemoveHoldPiece()
    {
        hold_pv.RemovePiece();
        holdPiece = null;
    }
    /// <summary>
    /// Set new piece, reset position generate next in queue back
    /// </summary>
    public void ResetPiece()
    {
        holdUsed = false;

        //Debug.Log(Stats.Instance.turn_cnt);
        if (Stats.Instance.turn_cnt >= Stats.Instance.turn_limit)
        {
            Stats.Instance.turn_cnt = 0;
        }

        SetPiece(nextPieces.Dequeue());
        AddPieceInQueue();
        UpdateViews();

        ResetPosition();
    }
    private void UpdateViews()
    {
        hold_pv.ResetPiece(holdPiece);
        int i = 0;
        foreach (Piece p in nextPieces)
        {
            if (i >= next_pv.Length) break;

            if (next_pv[i].gameObject.activeSelf)
            {
                next_pv[i].ResetPiece(p);
            }
            i++;
        }
    }

    public void SetPiece(Piece piece, int rot_ = 0)
    {
        this.piece = piece;
        rot = rot_;
        shape = piece.GetShape(rot_);
        isFixed = piece.HasBlock(Block.Type.Chained);
        h = w = piece.w;
    }

    private void Rotate(int drot)
    {
        if (isFixed) return;

        if(rot + drot + 4 < 0)
        {
            Debug.LogWarning("Bad drot value");
        }
        rot = (rot + drot + 4) % 4;
        shape = piece.GetShape(rot);
    }

    private bool TryRotate(int drot)
    {
        if (isFixed) return false;

        int leftDepth = 0;
        int rightDepth = 0;
     
        Rotate(drot);
        for (int i = 0; i < h; i++)
        {
            for(int j = 0; j < w; j++)
            {
                int cx = j + x;
                if (shape[i][j].IsSolid() && cx < 0) leftDepth = Mathf.Max(leftDepth, -cx);
                if (shape[i][j].IsSolid() && cx >= glass_w) rightDepth = Mathf.Max(rightDepth, cx - glass_w + 1);
            }
        }
        if(leftDepth > 0 && rightDepth > 0)
        {
            Debug.LogError("Piece inside both walls?");
            Rotate(-drot);
            return false;
        }

        int dx = 0;
        if (leftDepth > 0) dx = leftDepth;
        if (rightDepth > 0) dx = -rightDepth;

        Move(dx, 0);
        if (!CheckPosition())
        {
            Move(-dx, 0);
            Rotate(-drot);
            return false;
        }
        return true;
    }

    public bool TryRotateLeft() // Try to rotate counter-clockwise
    {
        return TryRotate(-1);
    }
    public bool TryRotateRight() // Try to rotate clockwise
    {
        return TryRotate(1);
    }

    public void Move(int dx_, int dy_) // Doesnt check if free
    {
        x += dx_;
        y += dy_;
        //Debug.Log("set position: " + x.ToString() + ", " + y.ToString());
    }
    public bool TryMove(int dx_, int dy_) // Checks if free
    {
        Move(dx_, dy_);
        if (!CheckPosition())
        {
            //Debug.Log("Invalid position");
            Move(-dx_, -dy_);
            return false;
        }
        return true;
    }

    public void TryPutMove(int dx_, int dy_) // if occupied by block/edge, put figure
    {
        if(!TryMove(dx_, dy_))
        {
            PutPiece();
        }
    }

    public bool IsOnSurface()
    {
        bool moved = TryMove(0, -1);
        if (moved)
        {
            Move(0, 1);
        }
        return !moved;
    }

    public void MoveDownMax()
    {
        while(TryMove(0, -1)) { }
    }
    public void Drop()
    {
        // Creating particles in all empty blocks on the piece path
        HashSet<Tuple<int, int>> path = new HashSet<Tuple<int, int>>();
        for (int it = 0; it < 20; it++)
        {
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    int x1 = j + x;
                    int y1 = i + y;
                    if (!shape[i][j].IsEmpty())
                    {
                        if (InBounds(y1, x1) && !glass[y1][x1].IsSolid())
                        {
                            path.Add(Tuple.Create(y1, x1));
                        }
                    }
                }
            }
            if (!TryMove(0, -1)) break;
        }
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                int x1 = j + x;
                int y1 = i + y;
                if (!shape[i][j].IsEmpty())
                {
                    if (InBounds(y1, x1))
                    {
                        path.Remove(Tuple.Create(y1, x1));
                    }
                }
            }
        }
        foreach(var tp in path)
        {
            gameplay.drawGlass.PlayDropParticles(tp.Item1, tp.Item2);
        }

        PutPiece();
    }

    public bool CheckPosition()
    {
        CheckInitialReset();

        for (int i = 0; i < h; i++)
        {
            for(int j = 0; j < w; j++)
            {
                int x1 = j + x;
                int y1 = i + y;
                if (shape[i][j].IsSolid())
                {
                    //Debug.Log(x1.ToString() + " " + y1.ToString());
                    if (!InBounds(y1, x1))
                    {
                        return false;
                    }
                    if (glass[y1][x1].IsSolid())
                    {
                        //Debug.Log(gl.glass[y1][x1].fill);
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public void PutPiece()
    {
        PlayerStatEventManager.Instance.MakeTurn();

        RelicEvents.Instance.PutPiece();
        gameplay.interactionManager.PutPiece();
        gameplay.drawGlass.Draw();
    }

    public void ResetPosition()
    {
        CheckInitialReset();

        x = (glass_w - w) / 2;
        y = glass_h - h;

        if (!CheckPosition())
        {
            Debug.LogWarning("Space for new figure is occupied");
        }
    }

}
