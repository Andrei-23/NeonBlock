using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWarningAnimation : MonoBehaviour
{
    public float min_alpha;
    public float max_alpha;
    public float loop_duration;
    void Start()
    {
        LeanTween.alpha(GetComponent<RectTransform>(), min_alpha, 0f);
        LeanTween.alpha(GetComponent<RectTransform>(), max_alpha, loop_duration / 2f).setLoopPingPong().setEaseInOutQuad();
    }
}
