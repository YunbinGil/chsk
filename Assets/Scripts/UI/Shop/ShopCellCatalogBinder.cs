using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using chsk.Core.Data;
using chsk.Core.Services;

/// <summary>
/// itemId만 넣으면, 선택한 Provider(Spice/Kitchen 등)에서
/// 가격/이름/아이콘을 읽어와 UI에 바인딩해 주는 범용 바인더.
/// </summary>
/// 
namespace chsk.UI.Shop
{ 
    public class ShopCellCatalogBinder : MonoBehaviour
    {
        public enum ProviderType { Auto, Spice, Kitchen }
        [Header("아이템 식별자")]
        [SerializeField] private string itemId;

        [Header("카탈로그 제공자 (SpiceManager, KitchenManager 등)")]
        [SerializeField] private ProviderType providerType = ProviderType.Auto;
        [SerializeField] private Component providerOverride; // 직접 드래그로 지정하고 싶을 때    
        private IShopCatalogProvider _p;

        [Header("바인딩 대상 (UI 없으면 생략 가능)")]
        [SerializeField] private TMP_Text priceText;      // 예: Text.price.gold
        [SerializeField] private TMP_Text nameText;       // 예: Text.item
        [SerializeField] private Image iconImage;      // 예: item_img

        [Header("표시 포맷")]
        [SerializeField] private string priceFormat = "{0}";     // "{0} G" 등
        [SerializeField] private bool bindOnAwake = true;

        [Header("재사용 이벤트")]
        public UnityEvent<int> OnPriceParsed;             // 읽은 가격을 다른 버튼 등에 주입

        void Awake()
        {
            // 1) 인스펙터에 override 있으면 최우선
            if (providerOverride)
            {
                _p = providerOverride as IShopCatalogProvider
                     ?? providerOverride.GetComponent(typeof(IShopCatalogProvider)) as IShopCatalogProvider;
            }

            // 2) 없으면 타입 스위치로 찾기
            if (_p == null)
            {
                switch (providerType)
                {
                    case ProviderType.Spice:
#if UNITY_2022_3_OR_NEWER
                    _p = FindAnyObjectByType<SpiceManager>(FindObjectsInactive.Include);
#else
                        _p = FindObjectOfType<SpiceManager>();
#endif
                        break;

                    case ProviderType.Kitchen:
#if UNITY_2022_3_OR_NEWER
                    _p = FindAnyObjectByType<chsk.Core.Services.KitchenManager>(FindObjectsInactive.Include);
#else
                        _p = FindObjectOfType<chsk.Core.Services.KitchenManager>();
#endif
                        break;

                    case ProviderType.Auto:
                    default:
#if UNITY_2022_3_OR_NEWER
                    _p = (IShopCatalogProvider)FindAnyObjectByType<SpiceManager>(FindObjectsInactive.Include)
                        ?? FindAnyObjectByType<chsk.Core.Services.KitchenManager>(FindObjectsInactive.Include);
#else
                        _p = (IShopCatalogProvider)FindObjectOfType<SpiceManager>()
                            ?? FindObjectOfType<chsk.Core.Services.KitchenManager>();
#endif
                        break;
                }
            }

            if (bindOnAwake) Refresh();
        }

        public string ItemId => itemId;
        public void SetItemId(string id) { itemId = id; Refresh(); }

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
}
