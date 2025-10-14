using UnityEngine;
using UnityEngine.UI;
using TMPro;
using chsk.Core.Data;
using chsk.Core.Services;

namespace chsk.UI.Spice
{
    // ShelfCell 프리팹 내부에 넣거나, 없으면 런타임에 자동 AddComponent 됨
    public class SpiceCell : MonoBehaviour
    {
        [Header("UI Refs")]
        [SerializeField] private Image icon;           // Icon Area/spice_img
        [SerializeField] private TMP_Text nameText;    // Text (TMP)
        [SerializeField] private TMP_Text countText;   // Count (TMP) - 없으면 만들어도 됨

        private int _slotId;
        private SpiceData _data;
        private System.Func<int> _getMaxPerSlot;
        private System.Action _onUse;

        // 프리팹 자식 이름 기준 자동 바인딩
        void Reset()
        {
            if (!icon) icon = transform.Find("Icon Area/spice_img")?.GetComponent<Image>();
            if (!nameText) nameText = transform.Find("Text (TMP)")?.GetComponent<TMP_Text>();
            if (!countText) countText = transform.Find("Count (TMP)")?.GetComponent<TMP_Text>();
        }

        public void BindSlot(int slotId, SpiceData data, System.Func<int> getMaxPerSlot, System.Action onUse = null)
        {
            _slotId = slotId;
            _data = data;
            _getMaxPerSlot = getMaxPerSlot;
            _onUse = onUse;

            if (icon != null && data != null) icon.sprite = data.icon;
            if (nameText != null && data != null) nameText.text = data.displayName;

        }

        public void SetCount(int c)
        {
            int max = _getMaxPerSlot != null ? _getMaxPerSlot() : 10;
            if (countText) countText.text = $"{c}";
        }
    }
}
