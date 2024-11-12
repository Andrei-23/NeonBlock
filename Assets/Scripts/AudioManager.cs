using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static BlockDataManager;

public enum SoundClip
{
    gameMusic = 0,
    meunMusic,
    bossMusic,

    winSound,
    loseSound,
    UIclick,
    UImouseEnter,
    openLevel,
    startLevel,
    winLevel,
    selectPiece,
    selectRelic,

    pieceMove,
    pieceRotate,
    pieceHold,
    piecePlace,
    lineClear,
    lineDamage,
    laserWarning,
    laserDrop,
    energyGain,
    pixelGain,

}

[Serializable]
public class SourceVariant
{
    public AudioSource _defaultSource;
    public AudioSource _webSource;

    public AudioSource GetSource(bool web)
    {
        if(_webSource == null || web == false)
        {
            if(_defaultSource == null)
            {
                Debug.LogError("No AudioSource assigned");
            }
            return _defaultSource;
        }
        return _webSource;
    }
}

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;

    [SerializeField] private bool _isWebAudio; // Editor only
    [SerializeField] private EnumMap<SoundClip, SourceVariant> _mapSources;
    private List<AudioSource> _sources;

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

        LoadSourceList();
    }

    void OnValidate()
    {
        _mapSources.TryRevise();
    }
    void Reset()
    {
        _mapSources.TryRevise();
    }
    private void LoadSourceList()
    {
        bool loadWeb = false;

#if UNITY_WEBGL
    loadWeb = true;
#endif
#if UNITY_EDITOR
    loadWeb = _isWebAudio;
#endif

        Debug.Log("Load web audio: " + loadWeb.ToString());

        //int n = System.Enum.GetNames(typeof(SoundClip)).Length;
        _sources = new List<AudioSource>();
        foreach (SoundClip sound in Enum.GetValues(typeof(SoundClip)))
        {
            _sources.Add(_mapSources[sound].GetSource(loadWeb));
        }
    }


    private void Start()
    {
        PlaySound(SoundClip.meunMusic);
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(SoundClip clip)
    {
        if (_sources[(int)clip].IsUnityNull()) return;
        _sources[(int)clip].Play();
    }
    public void StopSound(SoundClip clip)
    {
        if (_sources[(int)clip].IsUnityNull()) return;
        _sources[(int)clip].Stop();
    }
    public void RestartSound(SoundClip clip)
    {
        StopSound(clip);
        PlaySound(clip);
    }


    public void PlayClearSound(float combo = 0f)
    {
        const float max_pitch_combo = 5f;
        const float min_pitch = 0.8f;
        const float max_pitch = 1.4f;

        if (combo < 0f)
        {
            Debug.LogWarning("Incorrect combo value");
        }

        combo = Mathf.Clamp(combo, 1f, max_pitch_combo);
        float val = (combo - 1f) / (max_pitch_combo - 1f);
        float pitch = (max_pitch - min_pitch) * val + min_pitch;
        _sources[(int)SoundClip.lineClear].pitch = pitch;
        PlaySound(SoundClip.lineClear);
    }

    public void PlayCoinSound()
    {
        StopSound(SoundClip.pixelGain);
        PlaySound(SoundClip.pixelGain);
    }

    public void StopAllMusic()
    {
        StopSound(SoundClip.meunMusic);
        StopSound(SoundClip.gameMusic);
        StopSound(SoundClip.bossMusic);
    }
    public void SetMusic(SoundClip musicClip, bool restart = false)
    {
        if (!restart && _sources[(int)musicClip].isPlaying) return;
        StopAllMusic();
        PlaySound(musicClip);
    }
}
