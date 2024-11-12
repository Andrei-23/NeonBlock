using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BindingUI : MonoBehaviour
{
    [SerializeField]
    private InputActionReference inputActionReference;

    //[SerializeField]
    //private bool excludedMouse = true;
    [Range(0, 10)]
    [SerializeField]
    private int selectedBinding;
    [SerializeField]
    private InputBinding.DisplayStringOptions displayStringOptions;

    [Header("Binding Info (don't edit pls)")]
    [SerializeField]
    private InputBinding inputBinding;
    private int bindingIndex;

    private string actionName;

    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI actionText;
    [SerializeField]
    private Button rebindButton;
    [SerializeField]
    private TextMeshProUGUI rebindText;
    //[SerializeField]
    //private Button resetButton;

    private void OnEnable()
    {
        rebindButton.onClick.AddListener(() => DoRebind());
        //resetButton.onClick.AddListener(() => );

        if(inputActionReference != null)
        {
            GetBindingInfo();
            UpdateUI();
        }
    }
    private void OnValidate()
    {
        GetBindingInfo();
        UpdateUI();
    }

    private void GetBindingInfo()
    {
        if(inputActionReference.action != null)
        {
            actionName = inputActionReference.action.name;
        }
        if(inputActionReference.action.bindings.Count > selectedBinding)
        {
            inputBinding = inputActionReference.action.bindings[selectedBinding];
            bindingIndex = selectedBinding;
        }
    }
    private void UpdateUI()
    {
        if(actionText != null)
        {
            actionText.text = actionName;
        }
        if(rebindText != null)
        {
            if (Application.isPlaying)
            {

            }
            else
            {
                rebindText.text = inputActionReference.action.GetBindingDisplayString(bindingIndex, displayStringOptions);
            }
        }
    }

    private void DoRebind()
    {

    }
}
