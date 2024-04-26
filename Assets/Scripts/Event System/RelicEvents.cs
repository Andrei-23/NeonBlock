using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Relic = RelicsManager.RelicType;

public class RelicEvents
{
    private static RelicEvents _instance;
    public static RelicEvents Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RelicEvents();
            }
            return _instance;
        }
    }

    private RelicsManager rm => RelicsManager.Instance;

    public static event Action onRelicListUpdate;
    public static event Action<Relic> onRelicInfoUpdate;
    public static event Action<Relic> onRelicActive;

    public void UpdateRelicInfo(Relic type)
    {
        onRelicInfoUpdate?.Invoke(type);
    }
    public void UpdateRelicList()
    {
        onRelicListUpdate?.Invoke();
    }

    /// <summary>
    /// Set some relic values to 0
    /// </summary>
    public void ResetLevelValues()
    {
        rm.SetValue(Relic.Trashcan, 0);
    }

    public void PutPiece()
    {
        if (rm.IsActive(Relic.Trashcan) && rm.GetValue(Relic.Trashcan) > 0){
            if(rm.IncreaseValue(Relic.Trashcan, -1) == 0)
            {
                onRelicActive?.Invoke(Relic.Trashcan);
            }
        }
    }

    public void ClearLine()
    {
        if (rm.IsActive(Relic.Battery)) {
           PlayerStatEventManager.Instance.HealPlayer(rm.GetCount(Relic.Battery));    
        }
    }

    public void ExplotionCaused()
    {
        PlayerStatEventManager.Instance.GainEnergy(5 * rm.GetCount(Relic.ShockModule));
    }
    public void HoldPiece()
    {
        if (rm.IsActive(Relic.Trashcan))
        {
            rm.SetValue(Relic.Trashcan, 5);
        }
    }

    // called when player get a relic
    public void AddRelic(Relic type)
    {
        UpdateRelicList();
        switch (type)
        {
            // change current prices
            case Relic.Discount:
                onRelicActive?.Invoke(type);
                break;

            case Relic.DeathLaser:
                Stats.Instance.turn_limit += 2;
                break;

            case Relic.Timer:
                Stats.Instance.turn_limit++;
                break;
        }
    }
}
