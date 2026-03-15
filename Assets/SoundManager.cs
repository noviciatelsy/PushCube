using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;     // ±≥æ∞“Ù¿÷
    public AudioSource sfxSource;     // “Ù–ß

    [Header("“Ù–ß¡–±Ì")]
    public List<SoundData> sounds = new List<SoundData>();

    Dictionary<string, AudioClip> soundDict =
        new Dictionary<string, AudioClip>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitSoundDict();
    }

    private void Start()
    {
        SetBGMVolume(0.6f);
        PlayBGM("BGM1");
    }


    void InitSoundDict()
    {
        soundDict.Clear();

        foreach (var s in sounds)
        {
            if (!soundDict.ContainsKey(s.name))
                soundDict.Add(s.name, s.clip);
        }
    }

    // ==========================
    // ≤•∑≈“Ù–ß
    // ==========================

    public void PlaySFX(string name, float volumn = 1)
    {
        if (!soundDict.ContainsKey(name))
        {
            Debug.LogWarning("Sound not found: " + name);
            return;
        }
        Debug.Log("play sfx:" + name);
        sfxSource.PlayOneShot(soundDict[name]);
    }

    // ==========================
    // ≤•∑≈±≥æ∞“Ù¿÷
    // ==========================

    public void PlayBGM(string name)
    {
        if (!soundDict.ContainsKey(name))
        {
            Debug.LogWarning("Sound not found: " + name);
            return;
        }

        bgmSource.clip = soundDict[name];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    // ==========================
    // Õ£÷πBGM
    // ==========================

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // ==========================
    // “Ù¡øøÿ÷∆
    // ==========================

    public void SetBGMVolume(float v)
    {
        bgmSource.volume = v;
    }

    public void SetSFXVolume(float v)
    {
        sfxSource.volume = v;
    }
}

[System.Serializable]
public class SoundData
{
    public string name;
    public AudioClip clip;
}
