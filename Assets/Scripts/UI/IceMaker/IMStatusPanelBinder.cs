using UnityEngine;
using UnityEngine.UI;
using TMPro;
using chsk.Core.Services;
using System.Collections;

/// <summary>
/// itemId만 넣으면, 선택한 Provider(Spice/Kitchen 등)에서
/// 가격/이름/아이콘을 읽어와 UI에 바인딩해 주는 범용 바인더.
/// </summary>

namespace chsk.UI.IceMaker
{
    public class IMStatusPanelBinder : MonoBehaviour
    {
        [Header("카탈로그 제공자 IceMakerManager)")]
        [SerializeField] private IceMakerManager imManager;

        [Header("아이템 식별자")]
        [SerializeField] private string id;

        [Header("바인딩 대상")]
        [SerializeField] private Image iconImage;      // 예: item_img
        [SerializeField] private TMP_Text iceText;      // 예: Text.ice
        [SerializeField] private TMP_Text timeText;      // 예: Text.time

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

        public string ItemId => id; //getter
        public void SetItemId(string newId)
        {
            id = newId;
            Refresh();
        }
        public void Refresh()
        {
            if (imManager == null || string.IsNullOrEmpty(id)) return;
            if (iconImage) iconImage.sprite = imManager.GetIcon(id);
            if (iceText) iceText.text = imManager.GetPrdIce(id);
            if (timeText) timeText.text = imManager.GetTime(id);
        }
    }
}
