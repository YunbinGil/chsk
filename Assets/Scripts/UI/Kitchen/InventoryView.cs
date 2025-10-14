// InventoryView.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using chsk.UI.HUD;
using chsk.Core.Services;

namespace chsk.UI.Kitchen
{
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private Transform slotsParent; // 7칸 버튼들 부모
        [SerializeField] private GameObject slotPrefab; // (아이콘+버튼)
        [SerializeField] private ToastManager toast;

        void OnEnable()
        {
            StartCoroutine(Init());
        }

        IEnumerator Init()
        {
            // 필수 레퍼런스 검사
            if (!slotsParent) { Debug.LogError("[InventoryView] slotsParent 미지정"); yield break; }
            if (!slotPrefab)  { Debug.LogError("[InventoryView] slotPrefab 미지정");  yield break; }

            // KitchenManager 준비 대기
            while (KitchenManager.Instance == null) yield return null;

            var km = KitchenManager.Instance;
            // 이벤트 구독은 한 번만
            km.OnInventoryAdded += _ => RedrawSafe();
            km.OnInventoryRemoved += _ => RedrawSafe();

            RedrawSafe();
        }

        void OnDisable()
        {
            var km = KitchenManager.Instance;
            if (km != null)
            {
                km.OnInventoryAdded -= _ => RedrawSafe();
                km.OnInventoryRemoved -= _ => RedrawSafe();
            }
        }

        void RedrawSafe()
        {
            var km = KitchenManager.Instance;
            if (km == null || slotsParent == null || slotPrefab == null) return;

            foreach (Transform c in slotsParent) Destroy(c.gameObject);

            foreach (var s in km.GetInventory())
            {
                var data = km.GetData(s.toolId);
                if (data == null) continue;

                var go = Instantiate(slotPrefab, slotsParent);
                go.name = $"InvSlot_{s.slotId}";

                var btn = go.GetComponentInChildren<Button>();
                var img = go.GetComponentInChildren<Image>();
                var txt = go.GetComponentInChildren<TMP_Text>();

                if (img && data.icon) img.sprite = data.icon;
                if (txt) txt.text = data.displayName;

                if (btn)
                {
                    int capturedId = s.slotId; // 클로저 캡처
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() =>
                    {
                        toast?.Show($"{data.displayName}", 2f);
                        km.BeginPlaceFromInventory(capturedId);
                    });
                }
                else
                {
                    Debug.LogWarning("[InventoryView] slotPrefab에 Button이 없음");
                }
            }
        }
    }
}
