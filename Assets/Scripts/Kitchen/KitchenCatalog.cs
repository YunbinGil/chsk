using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MYBAR/Kitchen Catalog", fileName = "KitchenCatalog")]
public class KitchenCatalog : ScriptableObject
{
    public List<KitchenItemData> items = new List<KitchenItemData>();
    public KitchenItemData Get(string id) => items.Find(x => x.toolId == id);
}

[Serializable]
public class KitchenItemData
{
    public string toolId;          // 고유 ID (e.g., "ice_maker_mini")
    public string displayName;     // 표시명
    public int priceGold = 0;      // 구매가
    public Sprite icon;            // 인벤/미리보기 아이콘
    public Vector2 footprint = new Vector2(2.4f, 2.4f); // 충돌 확인용 크기(월드 단위). 픽셀/PPU에 맞춰 조정
    public GameObject prefabOverride; // 특정 녀석만 고유 프리팹 쓰고 싶을 때만 채움
}
