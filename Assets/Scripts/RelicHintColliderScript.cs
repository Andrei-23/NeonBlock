using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Relic = RelicsManager.RelicType;

public class RelicHintColliderScript : MonoBehaviour
{
    public Relic relic = Relic.Null;
    void Start()
    {
        
    }

    private void OnMouseEnter()
    {
        if (relic != Relic.Null)
        {
            HintEventManager.Instance.SetRelic(relic, gameObject);
        }
    }
    private void OnMouseExit()
    {
        if (relic != Relic.Null)
        {
            HintEventManager.Instance.SetDefaultState();
        }
    }
}
