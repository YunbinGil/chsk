// onOffBtn.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class onOffBtn : MonoBehaviour
{
    [Header("Key & Default")]
    public string settingKey = "music";   // "music", "sfx", "screenshake" 등
    public bool defaultOn = true;

    [Header("Visuals")]
    public Image spriteTarget;            // 바꿀 대상 (배경/노브 등)
    public Sprite onSprite;
    public Sprite offSprite;

    bool isOn;
    Button btn;

    void OnValidate()
    {
        if (!spriteTarget) spriteTarget = GetComponent<Image>();
    }

    void Awake()
    {
        btn = GetComponent<Button>();

        // 초기 로드
        if (SettingsManager.Instance)
            isOn = SettingsManager.Instance.Get(settingKey);
        else
            isOn = PlayerPrefs.GetInt(PrefKey(), defaultOn ? 1 : 0) == 1;

        ApplyVisual();

        // Button 클릭 연결
        if (btn) btn.onClick.AddListener(Toggle);

        // 디버그
       Debug.Log($"[OnOffBtn] init key={settingKey}, isOn={isOn}, target={spriteTarget?.name}");
    }

    void OnDestroy()
    {
        if (btn) btn.onClick.RemoveListener(Toggle);
    }

    void Toggle()
    {
        isOn = !isOn;

        // 저장 & 즉시 적용
        if (SettingsManager.Instance) 
            SettingsManager.Instance.Set(settingKey, isOn);
        else { PlayerPrefs.SetInt(PrefKey(), isOn ? 1 : 0); PlayerPrefs.Save(); }

        ApplyVisual();

        Debug.Log($"[OnOffBtn] Toggle key={settingKey} -> {isOn}");
    }

    void ApplyVisual()
    {
        if (!spriteTarget) return;
        spriteTarget.sprite = isOn ? onSprite : offSprite;
    }

    // 시각 외 실제 효과가 필요하면 여기에서 (음소거 등)
    void ApplyEffect()
    {
        // SettingsManager.Set에서 이미 처리하면 여기서 할 일 없음.
        // 별도 처리 원하면 여기에 추가.
    }

    string PrefKey() => $"SET_{settingKey.ToUpper()}";
}
