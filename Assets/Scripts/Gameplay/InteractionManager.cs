using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

using Relic = RelicsManager.RelicType;

public class InteractionManager : MonoBehaviour
{
    //private List<int> addMoneyArr = new List<int> { 10, 30, 60, 100, 200, 300 }; // how much money to add on different combos

    private Gameplay gameplay;

    // sorts blocks in priority order before executing function on all these blocks
    private List<BlockCompareStruct> blockSortList;

    private int extra_damage = 0; // sum of damage from different sources(not lines?)
    private bool stop_flag = false; // true means animation stop hit required
    private int remove_cnt = 0; // amount of lines to delete

    private int clear_lines_cnt = 0; // how much line deleted at once

    //private int total_attack = 0;
    private int total_damage = 0; // to player

    private RelicsManager RM => RelicsManager.Instance;
    private List<List<Block>> glass => gameplay.glassManager.glass;
    private int glass_w => GlassManager.w;
    private int glass_h => GlassManager.h;

    private bool enoughEnergy => Stats.Instance.energy >= Stats.Instance.energy_task;
    public bool InBounds(int y, int x)
    {
        return x >= 0 && x < glass_w && y >= 0 && y < glass_h;
    }

    private struct BlockCompareStruct
    {
        public int x, y;
        public Block block;

        public BlockCompareStruct(int y, int x, Block block)
        {
            this.x = x;
            this.y = y;
            this.block = block;
        }
    }

    void Awake()
    {
        blockSortList = new List<BlockCompareStruct>();
        gameplay = GetComponent<Gameplay>();

        Stats.Instance.ResetLevelStats();
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }



    /// <summary>
    /// Put piece, then update glass and check for full lines.
    /// Run animation where required.
    /// </summary>
    public IEnumerator PutPieceWaiter()
    {
        // Put piece
        if (PutAllBlocks()) yield return StartCoroutine(waiter());

        // Delay after explotions/ deleting lines
        if (UpdateClearedBlocks()) yield return StartCoroutine(waiter());

        // Update glass
        if (UpdateGlass()) yield return StartCoroutine(waiter());

        // Activate some blocks once per turn
        if (StartActivate()) yield return StartCoroutine(waiter());

        // Update glass
        if (UpdateGlass()) yield return StartCoroutine(waiter());

        bool noClears = true;
        // try to clear lines then update
        while (!enoughEnergy)
        {
            // Find and clear lines, stop if no lines found
            // Update combo
            clear_lines_cnt = 0;
            TryClearLines();
            if (clear_lines_cnt == 0) break;
            noClears = false;

            // SFX
            if (stop_flag)
            {
                // Play line clear sound
                AudioManager.Instance.PlayClearSound(Stats.Instance.combo);

                yield return StartCoroutine(waiter(0.3f));
            }
            for (int i = 0; i < clear_lines_cnt; i++)
            {
                RelicEvents.Instance.ClearLine();
            }

            // Relics
            if (RM.IsActive(Relic.Square) && clear_lines_cnt == 4)
            {
                //int x = RM.IsActive(Relic.Geometry) ? 2 : 1;
                RM.IncreaseValue(Relic.Square, 1);
            }
            if (RM.IsActive(Relic.Triangle) && clear_lines_cnt == 3)
            {
                GainEnergy(RM.GetValue(Relic.Triangle) * 3);
            }

            // Energy
            float val = Stats.Instance.combo * clear_lines_cnt * GetLineCost();
            GainEnergy(val);

            // Delay after explotions/ deleting lines
            if (UpdateClearedBlocks()) yield return StartCoroutine(waiter(0.2f));

            // Update glass
            if (UpdateGlass()) yield return StartCoroutine(waiter());
        }

        // Reset combo if no clears
        if (noClears && !RM.IsActive(Relic.Fuse))
        {
            PlayerStatEventManager.Instance.ResetCombo();
        }

        // Before next turn
        if (EndActivate()) yield return StartCoroutine(waiter());

        // Update glass
        if (UpdateGlass()) yield return StartCoroutine(waiter());

        // Check win
        if (enoughEnergy)
        {
            gameplay.LevelComplete();
            //PieceStateManager.Instance.SetState(PieceState.Default);
            yield break;
        }

        // Move laser
        if (CheckMoveLaser())
        {
            gameplay.drawGlass.UpdateLaserPosition(true);
            yield return StartCoroutine(waiter(0.8f));
        }

        // Deal damage if blocks are too high
        if (CheckDamageLine())
        {
            // Play delete sound
            AudioManager.Instance.PlaySound(SoundClip.lineDamage);
            CameraShake.Instance.ShakeCamera(0.3f, 0.3f);
            yield return StartCoroutine(waiter());
            UpdateClearedBlocks();
            yield return StartCoroutine(waiter());
        }

        AttackPlayer();

        PieceStateManager.Instance.SetState(PieceState.Default); // Stop animation freeze
        if (PlayerStatEventManager.Instance.CurrentLiveState == PlayerLiveState.Dead)
        {
            yield break;
        }

        gameplay.pieceManager.ResetPiece();

        gameplay.drawGlass.Draw();
    }
    

    /// <summary>
    /// Returns actual line cost
    /// </summary>
    /// <returns></returns>
    public int GetLineCost()
    {
        int cost = Stats.Instance.GetDefaultlineCost(true);

        // matrix-relied relics
        {
            int block_count = 0;
            int empty_count = 0;
            int under_laser_cnt = 0;

            for (int y = 0; y < glass_h; y++)
            {
                for (int x = 0; x < glass_w; x++)
                {
                    if (glass[y][x].type == Block.Type.Empty && y < gameplay.level.laser) empty_count++;
                    if (glass[y][x].type != Block.Type.Empty) block_count++;
                }
            }
            for (int x = 0; x < glass_w; x++)
            {
                if (!glass[gameplay.level.laser - 1][x].IsEmpty()) under_laser_cnt++;
            }

            cost += (block_count / 10) * RM.GetCount(Relic.NetworkUpgrade);
            cost += (empty_count / 20) * RM.GetCount(Relic.RecyclingProtocol);
            cost += under_laser_cnt * RM.GetCount(Relic.LaserRoulette);
        }
        if (RM.IsActive(Relic.BatterySlot))
        {
            cost *= 2;
        }
        if (RM.IsActive(Relic.Museum) && RM.GetValue(Relic.Museum) >= 25)
        {
            cost *= 2;
        }
        if (FindBlockType(Block.Type.Protector))
        {
            cost = 1;
        }

        cost = Mathf.Max(cost, 0);
        return cost;
    }
    /// <summary>
    /// Applied to energy from lines
    /// </summary>
    //private void GainEnergyFromLine(float value)
    //{
    //    PlayerStatEventManager.Instance.GainEnergy(value);
    //}

    /// <summary>
    /// Applied to all energy sources
    /// </summary>
    private void GainEnergy(float value)
    {
        PlayerStatEventManager.Instance.GainEnergy(value);
    }
    private void GainBlockEnergy(float energy, int y, int x)
    {
        if(energy == 0f)
        {
            return;
        }
        gameplay.drawGlass.PlayBlockEnergyEffect(energy, y, x);
        GainEnergy(energy);
    }

    private void AttackPlayer()
    {
        PlayerStatEventManager.Instance.DamagePlayer(total_damage);
        total_damage = 0;
    }
    private bool FindBlockType(Block.Type type)
    {
        for(int i = 0; i < glass_h; i++)
        {
            for(int j = 0; j < glass_w; j++)
            {
                if (glass[i][j].type == type) return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Activate special blocks on piece put, returns true if special block acivated.
    /// </summary>
    public bool PutBlock(int y, int x, Block b)
    {
        if (!InBounds(y, x))
        {
            Debug.LogError("block out of bounds");
            return false;
        }

        List<int> dx, dy;
        bool flag = false;

        switch (b.type)
        {
            
            case Block.Type.Bush:
                dx = new List<int> { -1, 0, 1, 0 };
                dy = new List<int> { 0, -1, 0, 1, };
                if (RM.IsActive(Relic.Fertilizer))
                {
                    dx = new List<int> { -1, 0, 1, 1, 1, 0, -1, -1 };
                    dy = new List<int> { 1, 1, 1, 0, -1, -1, -1, 0 };
                }
                for (int i = 0; i < dx.Count; i++)
                {
                    int nx = dx[i] + x;
                    int ny = dy[i] + y;
                    if (InBounds(ny, nx) && !glass[ny][nx].IsSolid())
                    {
                        glass[ny][nx] = b.Clone();
                        flag = true;
                    }
                }
                return flag;

            //case Block.Type.Shocker:
            //    gl.glass[y][x] = b;
            //    Shock(y, x);
            //    break;

            case Block.Type.Grenade:
            case Block.Type.Laser:
                DestroyBlock(y, x);
                return true;


            case Block.Type.Downloader:
                for (int ny = y - 1; ny >= 0; ny--)
                {
                    if (InBounds(ny, x) && !glass[ny][x].IsSolid())
                    {
                        flag = true;
                        glass[ny][x] = new Block(Block.Type.Default, 6);
                    }
                }
                return flag;

            case Block.Type.Mirror:
                {
                    int nx = glass_w - 1 - x;

                    if (!glass[y][nx].IsSolid())
                    {
                        glass[y][nx] = new Block(Block.Type.Mirror);
                        return true;
                    }
                    return false;
                }

            case Block.Type.Cleaner:
                glass[y][x] = new Block(Block.Type.Default, 8);
                dx = new List<int> { -1, -1, -1, 0, 1, 1, 1, 0 };
                dy = new List<int> { -1, 0, 1, 1, 1, 0, -1, -1 };
                for (int i = 0; i < dx.Count; i++)
                {
                    int nx = dx[i] + x;
                    int ny = dy[i] + y;

                    //if (InBounds(ny, nx)) Debug.Log(glass[ny][nx].type);

                    if (!InBounds(ny, nx)) continue;
                    Block nb = glass[ny][nx];
                    if (nb.IsEmpty() || nb.type == Block.Type.Default || nb.type == Block.Type.Cleaner)
                    {
                        continue;
                    }
                    glass[ny][nx] = new Block(Block.Type.Default, 8);
                }
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// If ghost block, turn to default block and shock adjacent blocks.
    /// </summary>
    //private void Shock(int y, int x)
    //{
    //    if (glass[y][x].type == Block.Type.Ghost)
    //    {
    //        glass[y][x].type = Block.Type.Default;
    //    }
    //    List<int> dx = new List<int> { -1, 0, 1, 0 };
    //    List<int> dy = new List<int> { 0, -1, 0, 1, };
    //    for (int i = 0; i < 4; i++)
    //    {
    //        int nx = dx[i] + x;
    //        int ny = dy[i] + y;
    //        if (InBounds(ny, nx) && glass[ny][nx].type == Block.Type.Ghost)
    //        {
    //            Shock(ny, nx);
    //        }
    //    }
    //}

    /// <summary>
    /// Delete block on line clear.
    /// </summary>
    private void ClearBlock(int y, int x) // on line delete
    {
        if (!InBounds(y, x))
        {
            Debug.LogError("block out of bounds");
            return;
        }

        Block.Type type = glass[y][x].type;
        switch (type)
        {
            // Dont delete block:
            case Block.Type.Bedrock: return;
            case Block.Type.Metal: return;
            case Block.Type.Stone: return;

            case Block.Type.Bomb:
                DestroyBlock(y, x);
                break;

            case Block.Type.Coin:
                PlayerStatEventManager.Instance.AddMoney(5);
                break;
            case Block.Type.ExtraEnergy:
                GainBlockEnergy(2, y, x);
                break;
            case Block.Type.Heal:
                PlayerStatEventManager.Instance.HealPlayer(2);
                break;

            case Block.Type.Fire:
                total_damage += 4;
                break;
            case Block.Type.Cursed:
                total_damage += 10;
                break;

            case Block.Type.TempEnergy:
                GainBlockEnergy(10, y, x);
                break;
            //case Block.Type.TempHeal:
            //    PlayerStatEventManager.Instance.HealPlayer(5);
            //    break;
            //case Block.Type.TempFire:
            //    total_damage += 10;
            //    break;

            case Block.Type.Empty:
                Debug.LogWarning("Clearing empty block");
                break;

            default:
                break;
        }
        glass[y][x] = new Block(Block.Type.Cleared);
    }

    /// <summary>
    /// Explode block. Most blocks not activate. Returns true if block activated.
    /// </summary>
    private bool DestroyBlock(int y, int x)
    {
        if (!InBounds(y, x))
        {
            Debug.LogError("block out of bounds");
            return false;
        }

        // Expltion area
        List<int> expl_dx = new List<int> { -1, -1, -1, 0, 1, 1, 1, 0 };
        List<int> expl_dy = new List<int> { -1, 0, 1, 1, 1, 0, -1, -1 };
        if (RM.IsActive(Relic.BombUpgrade))
        {
            expl_dx = new List<int> { -1, 0, 1, -2, -1, 0, 1, 2, -2, -1, 1, 2, -2, -1, 0, 1, 2, -1, 0, 1 };
            expl_dy = new List<int> { -2, -2, -2, -1, -1, -1, -1, -1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 2, 2, 2 };
        }

        Block.Type type = glass[y][x].type;
        switch (type)
        {
            // Dont delete:
            case Block.Type.Cleared: return false;
            case Block.Type.Exploded: return false;

            case Block.Type.Bedrock: return false;
            case Block.Type.Metal: return false;

            case Block.Type.Bomb:
            case Block.Type.Grenade:
                CameraShake.Instance.ShakeCamera(0.2f, 0.2f);
                RelicEvents.Instance.ExplotionCaused();
                glass[y][x] = new Block(Block.Type.Exploded);
                
                for (int i = 0; i < expl_dx.Count; i++)
                {
                    int nx = expl_dx[i] + x;
                    int ny = expl_dy[i] + y;
                    if (InBounds(ny, nx))
                    {
                        DestroyBlock(ny, nx);
                    }
                }
                break;

            case Block.Type.Laser:
                RelicEvents.Instance.ExplotionCaused();
                glass[y][x] = new Block(Block.Type.Exploded);
                if (RM.IsActive(Relic.LaserSplitter))
                {
                    for (int nx = 0; nx < glass_w; nx++)
                    {
                        if (x != nx && InBounds(y, nx))
                        {
                            DestroyBlock(y, nx);
                        }
                    }
                    for (int ny = 0; ny < glass_h; ny++)
                    {
                        if (y != ny && InBounds(ny, x))
                        {
                            DestroyBlock(ny, x);
                        }
                    }
                }
                else
                {
                    for (int ny = y - 1; ny >= 0; ny--)
                    {
                        if (InBounds(ny, x))
                        {
                            DestroyBlock(ny, x);
                        }
                    }
                }
                return true;
            case Block.Type.Cursed:
                //do dmg idk
                break;

            case Block.Type.Empty:
                break;

            default:
                if (RM.IsActive(Relic.Detonator))
                {
                    GainBlockEnergy(RM.GetCount(Relic.Detonator), y, x);
                }
                break;
        }
        glass[y][x] = new Block(Block.Type.Exploded);
        return true;
    }


    /// <summary>
    /// Update special block, returns true if special block was updated
    /// </summary>
    private bool UpdateBlock(int y, int x)
    {
        Block.Type type = glass[y][x].type;

        switch (type)
        {
            case Block.Type.Sand:
                int ny = y;
                while (ny > 0 && !glass[ny - 1][x].IsSolid())
                {
                    --ny;
                }
                if (ny != y)
                {
                    glass[ny][x] = glass[y][x];
                    glass[y][x] = new Block(' ');
                    return true;
                }
                return false;

            case Block.Type.VirusBlock:
                return UpdateVirusBlock(y, x);
        }
        return false;
    }

    /// <summary>
    /// Spread virus block in the area.
    /// </summary>
    /// <returns>Returns true if sth changed (failed).</returns>
    bool UpdateVirusBlock(int sy, int sx)
    {
        List<int> to_x = new List<int> { sx };
        List<int> to_y = new List<int> { sy };
        int ci = 0;
        bool change_flag = false;
        glass[sy][sx].Set(Block.Type.Empty);
        List<int> dx = new List<int> { 1, -1, 0, 0 };
        List<int> dy = new List<int> { 0, 0, 1, -1 };
        while (ci < to_x.Count)
        {
            int x = to_x[ci];
            int y = to_y[ci];
            ci++;
            if (glass[y][x].type == Block.Type.VirusBlock) continue;
            if (glass[y][x].type != Block.Type.Empty)
            {
                Debug.LogError("Trying to modify non-empty block to virus");
                continue;
            }

            // check if touched top
            if(y + 1 == glass_h) {
                RemoveVirusBlock(y, x);
                return true;
            }

            glass[y][x] = new Block(Block.Type.VirusBlock);
            for (int i = 0; i < 4; i++)
            {
                int nx = dx[i] + x;
                int ny = dy[i] + y;
                if (InBounds(ny, nx) && glass[ny][nx].type == Block.Type.Empty)
                {
                    to_x.Add(nx);
                    to_y.Add(ny);
                    change_flag = true;
                }
            }

        }
        return change_flag;
    }

    void RemoveVirusBlock(int sy, int sx)
    {
        List<int> to_x = new List<int> { sx };
        List<int> to_y = new List<int> { sy };
        int ci = 0;
        glass[sy][sx].Set(Block.Type.VirusBlock);
        List<int> dx = new List<int> { 1, -1, 0, 0 };
        List<int> dy = new List<int> { 0, 0, 1, -1 };
        while (ci < to_x.Count)
        {
            int x = to_x[ci];
            int y = to_y[ci];
            ci++;
            if (glass[y][x].type != Block.Type.VirusBlock) continue;

            glass[y][x] = new Block(Block.Type.Empty);
            for (int i = 0; i < 4; i++)
            {
                int nx = dx[i] + x;
                int ny = dy[i] + y;
                if (InBounds(ny, nx) && glass[ny][nx].type == Block.Type.VirusBlock)
                {
                    to_x.Add(nx);
                    to_y.Add(ny);
                }
            }

        }
    }

    /// <summary>
    /// Remove block without activation. True if you took extra damage.
    /// </summary>
    private bool DamageDeleteBlock(int y, int x)
    {
        Block.Type type = glass[y][x].type;

        if (type == Block.Type.Bedrock) return false;

        if (type == Block.Type.Metal || type == Block.Type.Stone || type == Block.Type.Cursed)
        {
            // (REMOVED)
            //extra_damage += 2;
            //glass[y][x] = new Block(Block.Type.RemovedSpeical);
            //return true;
        }

        glass[y][x] = new Block(Block.Type.Removed);
        return false;
    }

    /// <summary>
    /// Remove cleared/exploded blocks and push down blocks above
    /// </summary>
    bool UpdateClearedBlocks()
    {
        bool flag = false;
        List<int> del_column_cnt = new List<int>(glass_w);
        for (int i = 0; i < glass_w; i++)
        {
            del_column_cnt.Add(0);
        }

        // might add actual animation here
        for (int x = 0; x < glass_w; x++)
        {
            for (int y = 0; y < glass_h; y++)
            {
                //if (glass[y][x].type == Block.Type.Exploded)
                //{
                //    PlayerStatEventManager.Instance.GainEnergy(RM.GetCount(RelicsManager.RelicType.Detonator));
                //}
                if (glass[y][x].type == Block.Type.Exploded || glass[y][x].type == Block.Type.Cleared)
                {
                    flag = true;
                    del_column_cnt[x]++;
                    glass[y][x] = new Block(Block.Type.Empty);
                }
                else if (glass[y][x].type == Block.Type.Removed || glass[y][x].type == Block.Type.RemovedSpeical)
                {
                    del_column_cnt[x]++;
                    glass[y][x] = new Block(Block.Type.Empty);
                }
                else if (del_column_cnt[x] > 0)
                {
                    glass[y - del_column_cnt[x]][x] = glass[y][x];
                    glass[y][x] = new Block(Block.Type.Empty);
                }
            }
        }
        return flag;
    }

    /// <summary>
    /// Activate blocks that should be activated once per turn
    /// </summary>
    /// <returns>true if sth changed</returns>
    bool StartActivate()
    {
        bool flag = false;
        for (int x = 0; x < glass_w; x++)
        {
            for (int y = 0; y < glass_h; y++)
            {
                switch (glass[y][x].type)
                {
                    case Block.Type.Miner:
                        PlayerStatEventManager.Instance.AddMoney(1);
                        break;

                    case Block.Type.SolarPanel:
                        if (RM.IsActive(Relic.DoubleSun))
                        {
                            GainBlockEnergy(1, y, x);
                            break;
                        }
                        bool blocks_up = false;
                        for(int y1 = y + 1; y1 < glass_h; y1++)
                        {
                            if (!glass[y1][x].IsEmpty())
                            {
                                blocks_up = true;
                                break;
                            }
                        }
                        if (!blocks_up)
                        {
                            GainBlockEnergy(1, y, x);
                        }
                        break;

                    default:
                        break;
                }
            }
        }
        return flag;
    }

    /// <summary>
    /// Deactivate all blocks that should be active only during this turn.
    /// </summary>
    /// <returns>true if sth changed</returns>
    bool EndActivate()
    {
        bool flag = false;
        for (int x = 0; x < glass_w; x++)
        {
            for (int y = 0; y < glass_h; y++)
            {
                Block b = glass[y][x];
                switch (b.type)
                {
                    case Block.Type.TempBlock:
                        glass[y][x] = new Block(Block.Type.Empty);
                        flag = true;
                        break;

                    case Block.Type.TempEnergy:
                        glass[y][x] = new Block(Block.Type.Default, 1);
                        flag = true;
                        break;
                    //case Block.Type.TempHeal:
                    //    glass[y][x] = new Block(Block.Type.Default);
                    //    flag = true;
                    //    break;
                    //case Block.Type.TempFire:
                    //    glass[y][x] = new Block(Block.Type.Default);
                    //    flag = true;
                    //    break;

                    default:
                        break;
                }
            }
        }

        // check if matrix is empty
        if (RM.IsActive(Relic.CleanContract))
        {
            bool isEmpty = true;
            for (int x = 0; x < glass_w; x++)
            {
                for (int y = 0; y < glass_h; y++)
                {
                    if (!glass[y][x].IsEmpty())
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (!isEmpty) break;
            }
            if (isEmpty)
            {
                PlayerStatEventManager.Instance.GainEnergy(30);
            }
        }
        return flag;
    }
    public void PutPiece()
    {
        PieceStateManager.Instance.SetState(PieceState.Animation);
        StartCoroutine(PutPieceWaiter());
    }

    IEnumerator waiter(float delay = 0.5f)
    {
        gameplay.drawGlass.Draw();
        // if called, set pieceState to Animation so gameplay is frozen.
        yield return new WaitForSeconds(delay);
    }
    /// <summary>
    /// Creates list of blocks in piece and calls PutBlock in priority order.
    /// </summary>
    private bool PutAllBlocks()
    {
        bool flag = false;
        blockSortList.Clear();
        for (int i = 0; i < gameplay.pieceManager.h; i++)
        {
            for (int j = 0; j < gameplay.pieceManager.w; j++)
            {
                int x = gameplay.pieceManager.x + j;
                int y = gameplay.pieceManager.y + i;
                if (InBounds(y, x))
                {
                    Block b = gameplay.pieceManager.shape[i][j];
                    switch (b.type)
                    {
                        case Block.Type.Empty: break;

                        //case Block.Type.Mirror:
                        //    glass[y][x] = gameplay.pieceManager.shape[i][j];
                        //    if (!glass[y][glass_w - 1 - x].IsSolid())
                        //        glass[y][glass_w - 1 - x] = gameplay.pieceManager.shape[i][j];
                        //    break;

                        case Block.Type.Ghost:
                            if (glass[y][x].type == Block.Type.Ghost)
                            {
                                glass[y][x] = new Block(Block.Type.Default, 0);
                            }
                            else if (glass[y][x].type == Block.Type.Empty)
                            {
                                glass[y][x] = b;
                            }
                            break;

                        default:
                            glass[y][x] = gameplay.pieceManager.shape[i][j];
                            blockSortList.Add(new BlockCompareStruct(y, x, glass[y][x]));
                            break;
                    }
                }
            }
        }

        blockSortList.Sort(CompareBlocksPrior);
        foreach (BlockCompareStruct bcs in blockSortList)
        {
            if (PutBlock(bcs.y, bcs.x, glass[bcs.y][bcs.x]))
            {
                flag = true;
            }
        }
        return flag;
    }

    /// <summary>
    /// Updates all blocks in glass in priority order.
    /// </summary>
    private bool UpdateGlass()
    {

        bool flag = false;
        blockSortList.Clear();
        for (int y = 0; y < glass_h; y++)
        {
            for (int x = 0; x < glass_w; x++)
            {
                blockSortList.Add(new BlockCompareStruct(y, x, glass[y][x]));
            }
        }

        blockSortList.Sort(CompareBlocksPrior);
        foreach (BlockCompareStruct bcs in blockSortList)
        {
            if (UpdateBlock(bcs.y, bcs.x))
            {
                flag = true;
            }
        }

        return flag;
    }

    private void UpdateCombo()
    {
        if (Stats.Instance.combo == 0f)
        {
            PlayerStatEventManager.Instance.SetCombo(Stats.Instance.combo_default);
        }
        else
        {
            PlayerStatEventManager.Instance.AddCombo(Stats.Instance.combo_mult);
        }
        PlayerStatEventManager.Instance.AddCombo((clear_lines_cnt - 1) * Stats.Instance.combo_mult);

        if(clear_lines_cnt > 1 && RM.IsActive(Relic.ParallelModule))
        {
            PlayerStatEventManager.Instance.AddCombo(clear_lines_cnt * 0.2f);
        }
    }


    /// <summary>
    /// Delete full lines with animation, activate some blocks.
    /// </summary>
    private void TryClearLines()
    {
        List<int> fullLines = FindFullLines();
        int cnt = fullLines.Count;
        clear_lines_cnt += cnt;
        stop_flag = true;
        if (cnt == 0)
        {
            stop_flag = false;
            return;
        }

        // Count combo
        UpdateCombo();

        // clear effects
        foreach (int i in fullLines)
        {
            float val = Stats.Instance.combo * GetLineCost();
            gameplay.drawGlass.PlayLineClearEffect(i, val);
        }

        blockSortList.Clear();
        for (int i = 0; i < cnt; i++)
        {
            int y = fullLines[i];
            for (int x = 0; x < glass_w; x++)
            {
                blockSortList.Add(new BlockCompareStruct(y, x, glass[y][x]));
            }
        }

        // sorting block by priority so different types called correctly
        blockSortList.Sort(CompareBlocksPrior);

        foreach (BlockCompareStruct bcs in blockSortList)
        {
            ClearBlock(bcs.y, bcs.x);
        }
    }

    private bool CheckMoveLaser()
    {
        if(Stats.Instance.turn_cnt >= Stats.Instance.turn_limit && gameplay.level.laser > 1)
        {
            gameplay.level.laser--;
            return true;
        }
        return false;
    }

    /// <summary>
    /// If after block clear there are blocks above damage line, deal damage to player.
    /// </summary>
    private bool CheckDamageLine()
    {
        blockSortList.Clear();
        int max_height = -1;
        for (int i = gameplay.level.laser; i < glass_h; i++)
        {
            for (int j = 0; j < glass_w; j++)
            {
                // don't trigger on bedrock & ghosts
                if (glass[i][j].IsSolid() && glass[i][j].type != Block.Type.Bedrock)
                {
                    max_height = i;
                }
            }
        }
        if (max_height == -1) return false;

        remove_cnt = max_height - gameplay.level.laser + 1;
        int line_damage = remove_cnt * 10;

        bool clear_bottom = gameplay.CheckDeleteDirectionTop();
        for (int i = 0; i < remove_cnt; i++)
        {
            for (int j = 0; j < glass_w; j++)
            {
                DamageDeleteBlock(i + (clear_bottom ? gameplay.level.laser : 0), j);;
            }
        }

        float mult = 1f;
        if (RM.IsActive(Relic.Resistor))
        {
            mult *= Mathf.Pow(0.9f, RM.GetCount(Relic.Resistor));
        }
        total_damage += (int)(mult * (line_damage + extra_damage));

        if (RM.IsActive(Relic.DeathLaser)) total_damage += 9999; // rip

        return true;
    }

    public List<int> FindFullLines()
    {
        List<int> res = new List<int>();
        for (int i = 0; i < glass_h; i++)
        {
            bool has_empty = false;
            bool has_lock = false;
            bool has_key = false;

            for (int j = 0; j < glass_w; j++)
            {
                switch (glass[i][j].type)
                {
                    case Block.Type.Empty: has_empty = true; break;
                    case Block.Type.Key: has_key = true; break;
                    case Block.Type.Lock: has_lock = true; break;
                }
                if (has_empty) break;
            }
            if (!has_empty && !(has_lock && !has_key))
            {
                res.Add(i);
            }
        }

        return res;
    }


    private void ClearRemoved()
    {
        for (int i = remove_cnt; i < glass_h; i++)
        {
            for (int j = 0; j < glass_w; j++)
            {
                glass[i - remove_cnt][j] = glass[i][j];
                glass[i][j] = new Block(Block.Type.Empty);
            }
        }
    }

    private int CompareBlocksPrior(BlockCompareStruct first, BlockCompareStruct second)
    {
        Block b1 = first.block;
        Block b2 = second.block;

        if (b1.prior != b2.prior)
        {
            return b2.prior.CompareTo(b1.prior); // max prior first
        }
        if (b1.type != b2.type)
        {
            return b1.type.CompareTo(b2.type); // sort types as well
        }
        switch (b1.type)
        {
            case Block.Type.Sand: return first.y.CompareTo(second.y); // min y first
            default: return 0;
        }

    }

}
