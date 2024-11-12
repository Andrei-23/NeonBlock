using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class EnergyTaskText : MonoBehaviour
{
    public LocalizedString localizedTask; // Required energy:{task}
    [HideInInspector] public LocalizeStringEvent lse;
    [HideInInspector] public int task;

    private IntVariable task_var;

    public void Refresh()
    {
        localizedTask.Arguments[0] = task;
        localizedTask.RefreshString();
    }

    void Start()
    {
        lse = gameObject.GetComponent<LocalizeStringEvent>();
        lse.StringReference.Clear();

        task = Stats.Instance.energy_task;
        task_var = new IntVariable { Value = task };
        lse.StringReference.Add("task", task_var);
        lse.StringReference.RefreshString();
    }
}
