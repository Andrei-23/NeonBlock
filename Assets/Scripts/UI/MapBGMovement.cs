using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapBGMovement : MonoBehaviour
{
    public GameObject bg;
    public ScrollRect sr;

    public Vector2 speed; // v += speed * dt
    public Vector2 alpha; // dv *= alpha
    
    Vector2 default_pos; // initial pos
    Vector2 movement = Vector2.zero; // 

    float w = 256; // width of sprite in pixels

    void Awake()
    {
        default_pos = bg.transform.localPosition;
    }

    Vector2 FixVector(Vector2 pos)
    {
        while (pos.x >= w) { pos.x -= w; }
        while (pos.x < 0) { pos.x += w; }
        while (pos.y >= w) { pos.y -= w; }
        while (pos.y < 0) { pos.y += w; }
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
