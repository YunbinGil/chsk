using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    // 공개 키(토글에서 문자열로 씀)
    public const string KEY_MUSIC = "music";
    public const string KEY_SFX   = "sfx";

    // 기본값
    [Header("Defaults")]
    public bool defaultMusic = true;
    public bool defaultSfx = true;

    [Header("Audio (선택) — 태그로 자동 탐색")]
    public bool autoFindAudioByTag = true;
    public string musicTag = "Music";
    public string sfxTag   = "SFX";
    public List<AudioSource> musicSources = new List<AudioSource>();
    public List<AudioSource> sfxSources   = new List<AudioSource>();

    // 내부 상태
    bool musicOn, sfxOn;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (autoFindAudioByTag)
        {
            if (musicSources.Count == 0) CollectByTag(musicTag, musicSources);
            if (sfxSources.Count   == 0) CollectByTag(sfxTag,   sfxSources);
        }

        // 로드 & 적용
        musicOn       = Load(KEY_MUSIC, defaultMusic);
        sfxOn         = Load(KEY_SFX,   defaultSfx);

        ApplyMusic(musicOn);
        ApplySfx(sfxOn);
        // screenshake는 저장만 — 게임 로직에서 SettingsManager.Get("screenshake")로 읽어 쓰면 됨
    }

    void CollectByTag(string tag, List<AudioSource> list)
    {
        foreach (var go in GameObject.FindGameObjectsWithTag(tag))
        {
            var a = go.GetComponent<AudioSource>();
            if (a && !list.Contains(a)) list.Add(a);
        }
    }

    // ====== 외부 API (문자열 키 기반) ======
    public bool Get(string key)
    {
        switch (key)
        {
            case KEY_MUSIC: return musicOn;
            case KEY_SFX:   return sfxOn;
            default:        return Load(key, true); // 알 수 없는 키도 저장값 있으면 반환
        }
    }

    public void Set(string key, bool value)
    {
        Save(key, value);

        switch (key)
        {
            case KEY_MUSIC:
                musicOn = value; ApplyMusic(value); break;
            case KEY_SFX:
                sfxOn = value;   ApplySfx(value);   break;
            default:
                // 커스텀 키는 저장만. 필요하면 여기서 즉시 적용 훅 추가
                break;
        }
    }

    // ====== 내부 저장/적용 ======
    bool Load(string key, bool def)
    {
        return PlayerPrefs.GetInt(PrefKey(key), def ? 1 : 0) == 1;
    }
    void Save(string key, bool val)
    {
        PlayerPrefs.SetInt(PrefKey(key), val ? 1 : 0);
        PlayerPrefs.Save();
    }
    string PrefKey(string key) => $"SET_{key.ToUpper()}";

    void ApplyMusic(bool on) { foreach (var a in musicSources) if (a) a.mute = !on; }
    void ApplySfx(bool on)   { foreach (var a in sfxSources)   if (a) a.mute = !on; }

    // 런타임 등록용(선택)
    public void RegisterAudioSource(AudioSource src, bool isMusic)
    {
        if (!src) return;
        if (isMusic) { if (!musicSources.Contains(src)) musicSources.Add(src); src.mute = !musicOn; }
        else         { if (!sfxSources.Contains(src))   sfxSources.Add(src);   src.mute = !sfxOn;  }
    }
}
