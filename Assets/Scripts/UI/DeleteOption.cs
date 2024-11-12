using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteOption : MonoBehaviour
{
    [HideInInspector] public int piece_id = 0; // id of piece to delete
    private SceneSwitcher sceneSwitcher;

    private void Start()
    {
        sceneSwitcher = Camera.main.GetComponent<SceneSwitcher>();
    }
    public void Choose()
    {
        Stats.Instance.AddPiece(piece_id, -1);
        SceneSwitcher.OpenMap();
    }
}
