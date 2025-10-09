using Game.Currencies;
using Game.Managers;
using UnityEngine;
using TMPro;
using System.Collections;

namespace Game.UI
{
    public class CurrencyDisplay : MonoBehaviour
    {
        [Header("설정")]
        public CurrencyType type;             // ★ 인스펙터에서 Gold/Ice 선택
        [SerializeField] TMP_Text text;       // 비우면 자동으로 GetComponent

        [Header("표시 옵션")]
        public bool abbreviate = false;       // 1,234 -> 1.23k
        public string prefix = "";            // 예: "x "
        public string suffix = "";            // 예: " G"

        bool _subscribed;

        void Awake()
        {
            if (!text) text = GetComponent<TMP_Text>();
        }

        void OnEnable()
        {
            StartCoroutine(InitAndBind());
        }

        void OnDisable()
        {
            Unsubscribe();
        }

        IEnumerator InitAndBind()
        {
            // CurrencyManager 준비될 때까지 대기
            float wait = 0f;
            while (CurrencyManager.Instance == null && wait < 5f)
            {
                wait += Time.unscaledDeltaTime;
                yield return null;
            }

            if (CurrencyManager.Instance == null)
            {
                Debug.LogWarning("[CurrencyDisplay] CurrencyManager.Instance가 없음. 씬에 배치됐는지 확인!");
                yield break;
            }

            if (!text) text = GetComponent<TMP_Text>();

            // 초기값 반영
            UpdateLabel(CurrencyManager.Instance.Get(type));

            // 이벤트 구독(중복 방지)
            Unsubscribe();
            CurrencyManager.Instance.OnCurrencyChanged += HandleChanged;
            _subscribed = true;
        }

        void Unsubscribe()
        {
            if (_subscribed && CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.OnCurrencyChanged -= HandleChanged;
            }
            _subscribed = false;
        }

        void HandleChanged(CurrencyType t, int value)
        {
            if (t == type) UpdateLabel(value);
        }

        void UpdateLabel(int v)
        {
            if (!text) return;
            string body = abbreviate ? Abbrev(v) : v.ToString();
            text.text = $"{prefix}{body}{suffix}";
        }

        static string Abbrev(int n)
        {
            // 간단 축약: k, M, B
            if (n >= 1_000_000_000) return (n / 1_000_000_000f).ToString("0.##") + "B";
            if (n >= 1_000_000)     return (n / 1_000_000f).ToString("0.##") + "M";
            if (n >= 1_000)         return (n / 1_000f).ToString("0.##") + "k";
            return n.ToString();
        }
    }
}
