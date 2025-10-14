using System;                       
using System.Collections.Generic;
using chsk.Core.Data;
using UnityEngine;

namespace chsk.Core.Services{
public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance {get; private set;}

    [Serializable]
    public struct StartAmount {
        public CurrencyType type;
        public int amount;
    }

    [Header("초기 값(세이브가 없을 때만 적용)")]
    public List<StartAmount> defaults = new List<StartAmount> {
        new StartAmount{ type = CurrencyType.Gold, amount = 0 },
        new StartAmount{ type = CurrencyType.Ice,  amount = 0 },
    };

    // 변경 알림: (타입, 현재값)
    public event Action<CurrencyType,int> OnCurrencyChanged;

    Dictionary<CurrencyType,int> _balance = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 로드 또는 기본값 세팅
        foreach (CurrencyType t in Enum.GetValues(typeof(CurrencyType))) {
            int v = PlayerPrefs.GetInt(Key(t), -999999); // 존재 여부 체크
            if (v == -999999) {
                v = GetDefault(t);
                PlayerPrefs.SetInt(Key(t), v);
            }
            _balance[t] = Mathf.Max(0, v);
        }
        PlayerPrefs.Save();

        // 처음 한 번 UI에 알림
        foreach (var kv in _balance)
            OnCurrencyChanged?.Invoke(kv.Key, kv.Value);
    }

    int GetDefault(CurrencyType t)
    {
        foreach (var d in defaults) if (d.type == t) return d.amount;
        return 0;
    }
    string Key(CurrencyType t) => $"CUR_{t.ToString().ToUpper()}";

    // 조회
    public int Get(CurrencyType t) => _balance.TryGetValue(t, out var v) ? v : 0;

    // 지급(+)
    public void Add(CurrencyType t, int amount)
    {
        if (amount <= 0) return;
        _balance[t] = Get(t) + amount;
        PlayerPrefs.SetInt(Key(t), _balance[t]);
        PlayerPrefs.Save();
        OnCurrencyChanged?.Invoke(t, _balance[t]);
    }

    // 소비(-) : 가능하면 true
    public bool TrySpend(CurrencyType t, int cost)
    {
        if (cost <= 0) return true;
        int cur = Get(t);
        if (cur < cost) return false;

        _balance[t] = cur - cost;
        PlayerPrefs.SetInt(Key(t), _balance[t]);
        PlayerPrefs.Save();
        OnCurrencyChanged?.Invoke(t, _balance[t]);
        return true;
    }

    // 디버그용 리셋
    [ContextMenu("Reset All Currency")]
    public void ResetAll()
    {
        foreach (CurrencyType t in Enum.GetValues(typeof(CurrencyType))) {
            _balance[t] = GetDefault(t);
            PlayerPrefs.SetInt(Key(t), _balance[t]);
            OnCurrencyChanged?.Invoke(t, _balance[t]);
        }
        PlayerPrefs.Save();
    }
}
}
