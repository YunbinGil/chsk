// PlaceableTool.cs
using UnityEngine;
using Game.Kitchen;

[RequireComponent(typeof(BoxCollider2D))]
public class PlaceableTool : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    private BoxCollider2D col;

    RectTransform bubblePanel;
    GameObject bubblePrefab;
    Camera cam;

    RectTransform bubbleRt;

    public void Init(RectTransform panel, GameObject prefab, Camera worldCam)
    {
        bubblePanel = panel;
        bubblePrefab = prefab;
        cam = worldCam;

        SpawnBubble();
    }
    void SpawnBubble()
    {
        if (!bubblePanel || !bubblePrefab) return;

        var go = Instantiate(bubblePrefab, bubblePanel, false);
        bubbleRt = go.GetComponent<RectTransform>();

        UpdateBubblePosition(+100f);
    }

    void LateUpdate()
    {
        if (bubbleRt) UpdateBubblePosition(+100f); // 매 프레임 따라오게
    }

    void UpdateBubblePosition(float offsetY)
    {
        Vector2 scr = (cam) ? (Vector2)cam.WorldToScreenPoint(transform.position)
                            : RectTransformUtility.WorldToScreenPoint(null, transform.position);
        scr += new Vector2(0, offsetY);

        var canvas = bubblePanel.GetComponentInParent<Canvas>();
        var uiCam  = (canvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas?.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(bubblePanel, scr, uiCam, out var local);
        bubbleRt.anchoredPosition = local;
    }

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
     void OnDestroy()
    {
        if (bubbleRt) Destroy(bubbleRt.gameObject);
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
