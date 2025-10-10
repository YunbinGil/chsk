// PlaceableTool.cs
using UnityEngine;
using Game.Kitchen;

public class PlaceableTool : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] BoxCollider2D col;

    public void Setup(KitchenItemData data)
    {
        var sprite = data.icon;
        if (sr) sr.sprite = sprite;
        if (col) col.size = data.footprint;      // 2.4 x 2.4 등 충돌 크기 결정
        var lp = GetComponent<Game.Kitchen.LongPressRelocate>();
        if (lp) lp.SetToolId(data.toolId);
    }
}
