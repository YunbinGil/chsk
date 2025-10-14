using UnityEngine;

namespace chsk.Core.Data
{
    public interface IShopCatalogProvider
    {
        // itemId로 가격/이름/아이콘 조회 (있으면 true)
        bool TryGetPrice(string itemId, out int price);
        string GetDisplayName(string itemId);    // null 가능
        Sprite GetIcon(string itemId);           // null 가능
    }

    public interface IShopPurchaseProvider
    {
        // true면 구매 성공(재화 차감 + 인벤 추가 등)
        bool TryBuy(string id);
    }
}