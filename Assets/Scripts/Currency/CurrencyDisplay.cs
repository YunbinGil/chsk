using Game.Currencies;
using Game.Managers;
using UnityEngine;
using TMPro;
using System.Collections;

namespace Game.UI
{
    public class CurrencyDisplay : MonoBehaviour { 

        public CurrencyType type;
        public TMP_Text text;

        void Awake() { if (!text) text = GetComponent<TMP_Text>(); }

        void OnEnable()
        {
            StartCoroutine(Init());
        }

        IEnumerator Init()
{
    while (CurrencyManager.Instance == null) yield return null;

    if (!text) text = GetComponent<TMP_Text>();
    text.text = CurrencyManager.Instance.Get(type).ToString();
    CurrencyManager.Instance.OnCurrencyChanged += HandleChanged;
}
        void OnDisable()
        {
            if (CurrencyManager.Instance)
                CurrencyManager.Instance.OnCurrencyChanged -= HandleChanged;
        }   
        void HandleChanged(CurrencyType t, int value)
        {
            if (t == type) text.text = value.ToString();
        }
    }
}