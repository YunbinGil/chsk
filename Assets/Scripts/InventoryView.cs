// InventoryView.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Game.Kitchen
{
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private Transform slotsParent; // 7칸 버튼들 부모
        [SerializeField] private GameObject slotPrefab; // (아이콘+버튼)
        [SerializeField] private ToastManager toast;

        void OnEnable()
        {
            Redraw();
            var km = KitchenManager.Instance;
            km.OnInventoryAdded += _ => Redraw();
            km.OnInventoryRemoved += _ => Redraw();
        }

        void OnDisable()
        {
            var km = KitchenManager.Instance;
            if (!km) return;
            km.OnInventoryAdded -= _ => Redraw();
            km.OnInventoryRemoved -= _ => Redraw();
        }

        void Redraw()
        {
            foreach (Transform c in slotsParent) Destroy(c.gameObject);

            var km = KitchenManager.Instance;
            foreach (var s in km.GetInventory())
            {
                var data = km.GetData(s.toolId);
                var go = Instantiate(slotPrefab, slotsParent);
                go.name = $"InvSlot_{s.slotId}";
                var btn = go.GetComponentInChildren<Button>();
                var img = go.GetComponentInChildren<Image>();
                var txt = go.GetComponentInChildren<TMP_Text>();

                if (img && data.icon) img.sprite = data.icon;
                if (txt) txt.text = data.displayName;

                btn.onClick.AddListener(() =>
                {
                    toast?.Show($"{data.displayName}", 2f);
                    km.BeginPlaceFromInventory(s.slotId);
                });
            }
        }
    }
}
