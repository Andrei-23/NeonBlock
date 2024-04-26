using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHintColliderScript : MonoBehaviour
{
    [HideInInspector] public Block block;
    
    void Start()
    {
        
    }

    private void OnMouseEnter()
    {
        if(block != null)
        {
            HintEventManager.Instance.SetBlock(block, gameObject);
        }
    }
    private void OnMouseExit()
    {
        if(block != null)
        {
            HintEventManager.Instance.SetDefaultState(); 
        }
    }
}
