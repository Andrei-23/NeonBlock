using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using Relic = RelicsManager.RelicType;
public class HintEventManager
{
    private static HintEventManager _instance;
    public static HintEventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new HintEventManager();
            }
            return _instance;
        }
    }
    private HintEventManager()
    {

    }

    //public delegate void BlockChangeHandler(Block block, GameObject obj);
    //public event BlockChangeHandler OnBlockChange;

    public delegate void HintChangeHandler(int state, Block block, Relic relic, GameObject obj);
    public event HintChangeHandler OnHintChange;

    private Block currentBlock;
    private Relic currentRelic;
    private bool showHint = true;
    public void SetBlock(Block block, GameObject obj)
    {
        if (block != currentBlock)
        {
            currentBlock = block;
            if(block.type == Block.Type.Empty)
            {
                ChangeHint(0, obj);
            }
            else
            {
                currentRelic = Relic.Null;
                ChangeHint(1, obj);
            }
        }
    }
    public void SetRelic(Relic relic, GameObject obj)
    {
        Debug.Log(relic.ToString());
        if (relic != currentRelic)
        {
            currentRelic = relic;
            if (relic == Relic.Null)
            {
                ChangeHint(0, obj);
            }
            else
            {
                currentBlock = new Block(Block.Type.Empty);
                ChangeHint(2, obj);
            }
        }
    }
    public void SetDefaultState()
    {
        currentRelic = Relic.Null;
        currentBlock = new Block(Block.Type.Empty);
        ChangeHint(0, null);
        //SetBlock(new Block(Block.Type.Empty), null);
    }

    public void SetVisibility(bool is_visible)
    {
        showHint = is_visible;
        if(showHint == false)
        {
            SetDefaultState();
        }
    }
    private void ChangeHint(int state, GameObject obj)
    {
        // state: 0 - empty, 1 - block, 2 - relic
        if(showHint)
        {
            OnHintChange?.Invoke(state, currentBlock, currentRelic, obj);
        }
    }
}
