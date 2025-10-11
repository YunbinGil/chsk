// ShopCellBuyBtn.cs
using UnityEngine;
using UnityEngine.UI;

public class ShopCellBuyBtn: MonoBehaviour
{
    public enum ProviderType { Auto, Spice, Kitchen }

    [SerializeField] private Button buyButton;
    [SerializeField] private ShopCellCatalogBinder binder; // 같은 셀에 붙어있는 바인더 재사용 권장
    [SerializeField] private ProviderType providerType = ProviderType.Auto;
    [SerializeField] private Component purchaseOverride; // 직접 지정 가능

    private IShopPurchaseProvider _purchase;

    void Reset()
    {
        if (!buyButton) buyButton = GetComponentInChildren<Button>(true);
        if (!binder)    binder    = GetComponent<ShopCellCatalogBinder>();
    }

    void Awake()
    {
        if (purchaseOverride)
            _purchase = purchaseOverride as IShopPurchaseProvider
                      ?? purchaseOverride.GetComponent(typeof(IShopPurchaseProvider)) as IShopPurchaseProvider;

        if (_purchase == null)
        {
            switch (providerType)
            {
                case ProviderType.Spice:
#if UNITY_2022_3_OR_NEWER
                    _purchase = FindAnyObjectByType<SpiceManager>(FindObjectsInactive.Include);
#else
                    _purchase = FindObjectOfType<SpiceManager>();
#endif
                    break;
                case ProviderType.Kitchen:
#if UNITY_2022_3_OR_NEWER
                    _purchase = FindAnyObjectByType<Game.Kitchen.KitchenManager>(FindObjectsInactive.Include) as IShopPurchaseProvider;
#else
                    _purchase = FindObjectOfType<Game.Kitchen.KitchenManager>() as IShopPurchaseProvider;
#endif
                    break;
                default: // Auto
#if UNITY_2022_3_OR_NEWER
                    _purchase = (IShopPurchaseProvider)FindAnyObjectByType<SpiceManager>(FindObjectsInactive.Include)
                             ?? FindAnyObjectByType<Game.Kitchen.KitchenManager>(FindObjectsInactive.Include) as IShopPurchaseProvider;
#else
                    _purchase = (IShopPurchaseProvider)FindObjectOfType<SpiceManager>()
                             ?? FindObjectOfType<Game.Kitchen.KitchenManager>() as IShopPurchaseProvider;
#endif
                    break;
            }
        }

        if (buyButton)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() =>
            {
                if (_purchase == null || binder == null) return;
                var ok = _purchase.TryBuy(binder.ItemId);
                // 필요하면 여기서 사운드/토스트 추가
            });
        }
    }
}
