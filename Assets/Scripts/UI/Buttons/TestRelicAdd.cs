using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRelicAdd : MonoBehaviour
{
    public RelicsList rl;
    void Start()
    {
        
    }

    public void AddRelic(int type)
    {
        RelicsManager.Instance.AddRelic(type);
    }
    public void AddRelic(RelicsManager.RelicType type)
    {
        AddRelic((int)type);
    }
    public void AddRandomRelic()
    {
        AddRelic(Random.Range(0, RelicsManager.Instance.count));
    }

    public void UpdateList()
    {
        rl.UpdateList();
    }
}
