using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MapPointLine : MonoBehaviour
{
    public GameObject from, to;

    private MapManager mapManager;
    private LineRenderer lr;
    
    private void Awake()
    {
        mapManager = Camera.main.GetComponent<MapManager>();
        lr = GetComponent<LineRenderer>();
    }
    private void Start()
    {
        UpdateLine();
    }
    void Update()
    {
        UpdateLine();
    }
    public void UpdateLine()
    {
        Vector2 a = from.transform.position;
        Vector2 b = to.transform.position;
        Vector2 dv = b - a;
        lr.SetPosition(0, a);
        lr.SetPosition(1, a + 0.1f * dv);
        lr.SetPosition(2, a + 0.2f * dv);
        lr.SetPosition(3, b - 0.2f * dv);
        lr.SetPosition(4, b - 0.1f * dv);
        lr.SetPosition(5, b);
    }

    public void SetGradient(bool is_cur_lvl)
    {
        if (is_cur_lvl)
        {
            lr.colorGradient = mapManager.highlightGradient;
        }
        else
        {
            lr.colorGradient = mapManager.defaultGradient;
        }
    }
}
