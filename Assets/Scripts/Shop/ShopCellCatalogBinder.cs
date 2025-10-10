using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// itemId만 넣으면, 선택한 Provider(Spice/Kitchen 등)에서
/// 가격/이름/아이콘을 읽어와 UI에 바인딩해 주는 범용 바인더.
/// </summary>
public class ShopCellCatalogBinder : MonoBehaviour
{
    [Header("아이템 식별자")]
    [SerializeField] private string itemId;

    [Header("카탈로그 제공자 (SpiceManager, KitchenManager 등)")]
    [SerializeField] private Component provider; // IShopCatalogProvider를 구현한 컴포넌트
    private IShopCatalogProvider _p;

    [Header("바인딩 대상 (없으면 생략 가능)")]
    [SerializeField] private TMP_Text priceText;      // 예: Text.price.gold
    [SerializeField] private TMP_Text nameText;       // 예: Text.item
    [SerializeField] private Image iconImage;      // 예: item_img

    [Header("표시 포맷")]
    [SerializeField] private string priceFormat = "{0}";     // "{0} G" 등
    [SerializeField] private bool bindOnAwake = true;
    [SerializeField] private bool autoFindProviderIfNull = true; // ★추가

    [Header("재사용 이벤트")]
    public UnityEvent<int> OnPriceParsed;             // 읽은 가격을 다른 버튼 등에 주입

    void Awake()
    {
        /// ***
        /// 이부분은 kitchenManager.cs 생성 후 나중에 사용 
        /// ***
        
        //         if (provider == null && autoFindProviderIfNull)
        //         {
        // #if UNITY_2022_3_OR_NEWER
        //             provider = (Component)(FindAnyObjectByType<SpiceManager>(FindObjectsInactive.Include));
        //                         // ?? (Component)FindAnyObjectByType<KitchenManager>(FindObjectsInactive.Include));
        // #else
        //             provider = (Component)(FindObjectOfType<SpiceManager>());
        //                         // ?? (Component)FindObjectOfType<KitchenManager>());
        // #endif
        //         }

        //         _p = provider as IShopCatalogProvider;
        //         if (_p == null && provider != null)
        //             _p = provider.GetComponent(typeof(IShopCatalogProvider)) as IShopCatalogProvider;

        if (provider == null && autoFindProviderIfNull)
        {
#if UNITY_2022_3_OR_NEWER
    var spice = FindAnyObjectByType<SpiceManager>(FindObjectsInactive.Include);
    if (spice != null) provider = spice;
    else
    {
        foreach (var mb in FindObjectsByType<MonoBehaviour>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (mb is IShopCatalogProvider) { provider = mb; break; }
    }
#else
            var spice = FindObjectOfType<SpiceManager>();
            if (spice != null) provider = spice;
            else
            {
                foreach (var mb in FindObjectsOfType<MonoBehaviour>())
                    if (mb is IShopCatalogProvider) { provider = mb; break; }
            }
#endif
        }
        _p = provider as IShopCatalogProvider;
        if (_p == null && provider != null)
            _p = provider.GetComponent(typeof(IShopCatalogProvider)) as IShopCatalogProvider;

        if (bindOnAwake) Refresh();
    }

    public void SetItemId(string id)
    {
        itemId = id;
        Refresh();
    }

    public void Refresh()
    {
        if (_p == null || string.IsNullOrEmpty(itemId)) return;

        if (_p.TryGetPrice(itemId, out var price))
        {
            if (priceText) priceText.text = string.Format(priceFormat, price);
            OnPriceParsed?.Invoke(price);
        }

        if (nameText) nameText.text = _p.GetDisplayName(itemId) ?? nameText.text;
        if (iconImage) iconImage.sprite = _p.GetIcon(itemId) ?? iconImage.sprite;
    }
}
