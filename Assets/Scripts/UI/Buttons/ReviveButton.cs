using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Revive()
    {
        PlayerStatEventManager.Instance.CurrentLiveState = PlayerLiveState.Alive;
    }
}
