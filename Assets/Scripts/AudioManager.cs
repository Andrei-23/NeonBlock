using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource soundSource;

    [SerializeField] AudioSource lineClearSource; // sfx
    [SerializeField] AudioSource coinSource; // sfx

    [Header("Music Clips")]
    public AudioClip defaultMusic;
    public AudioClip menu_music;
    public AudioClip main_music;
    public AudioClip boss_music;
    public AudioClip win_music;
    public AudioClip lose_music;

    [Header("SFX Clips")]
    public AudioClip click;
    public AudioClip startLevel;
    public AudioClip completeLevel;
    public AudioClip selectItem;

    public AudioClip loseSound;
    public AudioClip winSound;

    public AudioClip pieceMove;
    public AudioClip pieceRotate;
    public AudioClip pieceHold;
    public AudioClip piecePlace;
    public AudioClip pieceDrop;

    public AudioClip lineClear;
    public AudioClip lineDelete;
    
    public AudioClip laserWarning;
    public AudioClip laserMove;

    public AudioClip energyGain;
    public AudioClip pixelGain;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        PlayMusic(defaultMusic);

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(AudioClip clip)
    {
        soundSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip music)
    {
        if (music == musicSource.clip) return;
        musicSource.clip = music;
        musicSource.Play();
    }
    public void StopMusic()
    {
        musicSource.Stop();
    }


    public void PlayClearSound(int combo = 0)
    {
        if (combo < 0)
        {
            Debug.LogError("Incorrect combo value");
            combo = 0;
        }
        lineClearSource.pitch = Mathf.Min(1.4f, 0.8f + 0.15f * combo);
        lineClearSource.PlayOneShot(lineClear);
        //clearSound.pitch = 0.8f;
    }

    public void PlayCoinSound()
    {
        coinSource.Stop();
        coinSource.PlayOneShot(pixelGain);
    }
}
