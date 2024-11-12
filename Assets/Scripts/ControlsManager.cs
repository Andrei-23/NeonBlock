using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager
{
    private static ControlsManager _instance;
    public static ControlsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ControlsManager();
            }
            return _instance;
        }
    }

    public static int count = 8;
    public List<KeyCode> codes = new List<KeyCode>(new KeyCode[count]);

    ControlsManager()
    {
        Load();
    }

    private KeyCode GetKeyCode(string name)
    {
        return (KeyCode)Enum.Parse(typeof(KeyCode), name);
    }

    private void LoadDefault()
    {
        codes[0] = KeyCode.LeftArrow; // left
        codes[1] = KeyCode.RightArrow; // right
        codes[2] = KeyCode.DownArrow; // down
        codes[3] = KeyCode.UpArrow; // drop
        codes[4] = KeyCode.Z; // rotate left
        codes[5] = KeyCode.X; // rotate right
        codes[6] = KeyCode.LeftShift; // hold
        codes[7] = KeyCode.Escape; // pause
    }
    private void Load()
    {
        LoadDefault();
        for(int i = 0; i < count; i++)
        {
            string prefs_key = "key_" + i.ToString();
            if (PlayerPrefs.HasKey(prefs_key))
            {
                codes[i] = GetKeyCode(PlayerPrefs.GetString(prefs_key));
            }
        }
    }

    private void Save()
    {
        for (int i = 0; i < count; i++)
        {
            PlayerPrefs.SetString("key_" + i.ToString(), codes[i].ToString());
        }
    }

    public void SetKey(int id, KeyCode key)
    {
        codes[id] = key;
    }
}
