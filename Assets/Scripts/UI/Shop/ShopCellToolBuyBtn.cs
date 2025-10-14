// ShopCellToolBuyButton_Simple.cs
using UnityEngine;
using UnityEngine.UI;
using chsk.UI.Kitchen;
using chsk.Core.Services;

namespace chsk.UI.Shop
{
    public class ShopCellToolBuyButton : MonoBehaviour
    {
        [SerializeField] private Button buyButton; // 상점 셀의 Buy 버튼
        [SerializeField] private string toolId;    // 파는 주방도구 ID

        void Reset()
        {
            if (!buyButton) buyButton = GetComponentInChildren<Button>(true);
        }

        void Awake()
        {
            if (buyButton)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(OnBuy);
            }
        }

        void OnBuy()
        {
            var km = KitchenManager.Instance;
            if (km == null || string.IsNullOrEmpty(toolId)) return;

            // 인벤 꽉 참/골드 부족이면 내부에서 false 반환
            bool ok = km.TryBuy(toolId);
            // ok에 따라 사운드/토스트 등은 원하면 여기서 추가
        }
    }
}
