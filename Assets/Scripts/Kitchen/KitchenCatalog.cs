using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MYBAR/Kitchen Catalog", fileName = "KitchenCatalog")]
public class KitchenCatalog : ScriptableObject
{
    public List<KitchenItemData> items = new List<KitchenItemData>();
}

[Serializable]
public class KitchenItemData
{
    public string itemId;        // 고유 ID (예: "shaker_lv1", "icebox_lv1")
    public string displayName;   // 표시 이름
    public int priceGold;        // 가격(골드)
    public Sprite icon;          // 아이콘
}
