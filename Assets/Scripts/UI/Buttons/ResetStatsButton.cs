using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStatsButton : MonoBehaviour
{
    public void OnClick()
    {
        Stats.Instance.Reset();
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

}
