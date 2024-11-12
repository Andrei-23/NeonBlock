using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;

    public void PlayDropParticles(Color color)
    {
        var psm = ps.main;
        Color color1 = color;
        Color color2 = color;
        color1.a = 0.3f;
        color2.a = 0.7f;
        psm.startColor = new ParticleSystem.MinMaxGradient(color1, color2);

        PlayDropParticles();
    }    

    public void PlayDropParticles()
    {
        ps.Play();
    }
}
