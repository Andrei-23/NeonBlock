using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControlsMenu : MonoBehaviour
{
    public GameObject[] buttons;
    public int selected_id = -1; // -1 if not selected
    ControlsManager cm;

    void Start()
    {
        cm = ControlsManager.Instance;
    }

    private void Update()
    {

    }

    void UpdateMenu()
    {
        for (int i = 0; i < ControlsManager.count; i++)
        {
            buttons[i].GetComponent<TextMeshProUGUI>().text = cm.codes[i].ToString();
        }
    }

    public bool isKeySelectActive()
    {
        return selected_id != -1;
    }
    void ChangeControl(int id)
    {
        selected_id = id;
    }
    /// <summary>
    /// Sets key to some action
    /// </summary>
    /// <param name="id">id of action in ControlsManager</param>
    /// <param name="key">attached key</param>
    public void SetKey(int id, KeyCode key)
    {
        ControlsManager.Instance.SetKey(id, key);
    }

}
