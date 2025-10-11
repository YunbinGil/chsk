// PlaceableTool.cs
using UnityEngine;
using Game.Kitchen;

[RequireComponent(typeof(BoxCollider2D))]
public class PlaceableTool : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    private BoxCollider2D col;

    public void Awake()
    {
        if (!sr) sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
    }

    public void Setup(KitchenItemData data)
    {
        var sprite = data.icon;
        if (sr) sr.sprite = sprite;
        FitColliderToSprite(sprite);
        var lp = GetComponent<Game.Kitchen.LongPressRelocate>();
        if (lp) lp.SetToolId(data.toolId);
    }
    private void FitColliderToSprite(Sprite sprite)
    {
        if (!col || !sprite) return;

        // Sprite.bounds는 "스프라이트 로컬 단위" (PPU 반영) 이라
        // BoxCollider2D(size/offset 로컬 단위)와 바로 일치합니다.
        var b = sprite.bounds;
        col.size = new Vector2(b.size.x, b.size.y);
        col.offset = new Vector2(b.center.x, b.center.y);
    }
#if UNITY_EDITOR
    void OnValidate()
    {
        if (!sr) sr = GetComponent<SpriteRenderer>();
        if (!col) col = GetComponent<BoxCollider2D>();
        if (sr && col && sr.sprite) FitColliderToSprite(sr.sprite);
    }
#endif
}
