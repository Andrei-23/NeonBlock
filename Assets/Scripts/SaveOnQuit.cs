using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveOnQuit : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        Stats.Instance.Save(); 
    }
}
