using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHintColliderScript : MonoBehaviour
{
    [HideInInspector] public Block block;
    public bool showOnPause; // if false, dont show hint on pause

    void Start()
    {
        
    }

    private void OnMouseEnter()
    {
        //Debug.Log(block.type);
        if (block == null) return;
        
        // if game running and no pause
        if (GameStateManager.Instance.CurrentGameState == GameState.Gameplay && !GameStateManager.Instance.IsPaused) return;
        if (!showOnPause && GameStateManager.Instance.IsPaused) return;

        HintEventManager.Instance.SetBlock(block, gameObject);
    }
    private void OnMouseExit()
    {
        if(block != null)
        {
            HintEventManager.Instance.SetDefaultState(); 
        }
    }
}
