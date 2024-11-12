using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapBGMovement : MonoBehaviour
{
    [SerializeField] private GameObject bg;
    [SerializeField] private ScrollRect sr;

    [SerializeField] private Vector2 speed; // v += speed * dt
    [SerializeField] private Vector2 alpha; // dv *= alpha
    
    [SerializeField] private float width = 256; // width of sprite in pixels

    private Vector2 default_pos; // initial pos
    private Vector2 movement = Vector2.zero; // 


    void Awake()
    {
        default_pos = bg.transform.localPosition;
    }

    Vector2 FixVector(Vector2 pos)
    {
        while (pos.x >= width) { pos.x -= width; }
        while (pos.x < 0) { pos.x += width; }
        while (pos.y >= width) { pos.y -= width; }
        while (pos.y < 0) { pos.y += width; }
        return pos;
    }

    public void FollowScroll()
    {
        Vector2 dxscroll = sr.content.localPosition;
        //Debug.Log(dxscroll);
        bg.transform.localPosition = FixVector(default_pos + dxscroll * alpha + movement);
    }

    void Update()
    {
        movement += speed * Time.deltaTime;
        movement = FixVector(movement);
        FollowScroll();
    }
}
