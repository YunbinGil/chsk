using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using chsk.Core.Data;
using chsk.Core.Services;

namespace chsk.UI.IceMaker
{
    public class IMCellCatalogBinder : MonoBehaviour
    {
        [Header("아이템 식별자")]
        [SerializeField] private string id;
        [Header("카탈로그 제공자 IceMakerManager)")]
        [SerializeField] private IceMakerManager imManager;

        [Header("바인딩 대상")]
        [SerializeField] private TMP_Text priceText;      // 예: Text.price.gold
        [SerializeField] private TMP_Text iceText;      // 예: Text.ice
        [SerializeField] private TMP_Text timeText;      // 예: Text.time
        [SerializeField] private Image iconImage;      // 예: item_img

        [SerializeField] private string timeFormat = "{0}s";     // "{0} G" 등
        [SerializeField] private bool bindOnAwake = true;

        void Awake()
        {
#if UNITY_2022_3_OR_NEWER
            imManager = FindAnyObjectByType<chsk.Core.Services.IceMakerManager>(FindObjectsInactive.Include);
#else
            imManager = FindObjectOfType<IceMakerManager>();
#endif

        }
        void OnEnable()
        {
            if (bindOnAwake) 
                StartCoroutine(DeferredRefresh());
        }
        IEnumerator DeferredRefresh()
        {
            yield return null;  // 한 프레임 대기(씬 로딩/폰트 준비 기다림)
            Refresh();
        }
        public string ItemId=> id; //getter

        public void Refresh()
        {
            if (imManager == null || string.IsNullOrEmpty(id)) return;
            if (imManager.TryGetPrice(id, out var price))
            {
                if (priceText) priceText.text = price.ToString();
            }
            if (iceText) iceText.text = imManager.GetPrdIce(id).ToString() ?? iceText.text;
            if (timeText) timeText.text = imManager.GetTime(id) ?? timeText.text;
            if (iconImage) iconImage.sprite = imManager.GetIcon(id) ?? iconImage.sprite;
            
        }
    }
}