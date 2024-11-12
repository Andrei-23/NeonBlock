using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource moveSound;
    public AudioSource rotateSound;
    public AudioSource lockSound;
    public AudioSource clearSound;
    public AudioSource deleteSound;
    public AudioSource WinSound;

    public static SoundManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void PlayClearSound(int combo = 0)
    {
        //Debug.Log(combo);
        if (combo < 0)
        {
            Debug.LogError("Incorrect combo value");
            combo = 0;
        }
        clearSound.pitch = Mathf.Min(1.4f, 0.8f + 0.15f * combo);
        clearSound.Play();
        //clearSound.pitch = 0.8f;
    }
    void Start()
    {
        
    }

}
