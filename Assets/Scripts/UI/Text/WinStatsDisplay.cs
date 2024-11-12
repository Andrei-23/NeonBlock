using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinStatsDisplay : MonoBehaviour
{
    public TextMeshProUGUI totalEnergy;
    //public TextMeshProUGUI totalPixels;
    public TextMeshProUGUI totalDamage;
    public TextMeshProUGUI totalPieces;
    public TextMeshProUGUI totalTurns;
    public TextMeshProUGUI totalLevels;
    public TextMeshProUGUI totalEpics;
    public TextMeshProUGUI totalTime;

    private void Start()
    {
        totalEnergy.text = Stats.Instance.total_energy.ToString();
        //totalPixels.text = Stats.Instance.money.ToString();
        totalDamage.text = Stats.Instance.total_damage.ToString();
        totalPieces.text = Stats.Instance.piece_count.ToString();
        totalTurns.text = Stats.Instance.total_turns.ToString();
        totalLevels.text = Stats.Instance.lvl_cnt.ToString();
        totalEpics.text = Stats.Instance.epic_cnt.ToString();

        //TimeSpan delta = DateTime.UtcNow - Stats.Instance.start_time;
        TimeSpan playtime = DateTime.UtcNow.Subtract(Stats.Instance.start_time);
        //string time_str = playtime.Hours + ":" + playtime.Minutes + ":" + playtime.Seconds;
        if (playtime.Hours > 0)
        {
            totalTime.text = playtime.ToString(@"hh\:mm\:ss");
        }
        else
        {
            totalTime.text = playtime.ToString(@"mm\:ss");
        }
    }
}
