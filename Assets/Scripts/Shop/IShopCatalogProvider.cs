using UnityEngine;

public interface IShopCatalogProvider
{
    // itemId로 가격/이름/아이콘 조회 (있으면 true)
    bool TryGetPrice(string itemId, out int price);
    string GetDisplayName(string itemId);    // null 가능
    Sprite GetIcon(string itemId);           // null 가능
}
