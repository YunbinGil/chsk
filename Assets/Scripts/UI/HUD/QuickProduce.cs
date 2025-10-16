using UnityEngine;
using UnityEngine.UI;
using chsk.UI.IceMaker;
using chsk.Core.Services;

namespace chsk.UI.HUD
{
    /// <summary>
    /// Quick Produce 모드 전환용 HUD - Ice용
    /// </summary>

    public class QuickProduce : MonoBehaviour
    {
        [SerializeField] GameObject root;   // 전체 HUD
        [SerializeField] Image itemImg;     // 선택 메뉴 아이콘
        [SerializeField] Button stopBtn;    // X 버튼

        void Awake()
        {
            root.SetActive(false);
            stopBtn.onClick.AddListener(() => IceMakerUIContext.StopQuickMode());
            IceMakerUIContext.OnQuickModeChanged += OnQuickChanged;
        }
        void OnDestroy()
        {
            IceMakerUIContext.OnQuickModeChanged -= OnQuickChanged;
        }

        void OnQuickChanged(bool active, string itemId)
        {
            root.SetActive(active);
            if (!active) return;

            // 아이콘 갱신
            var im = IceMakerManager.Instance;
            if (im && itemImg) itemImg.sprite = im.GetIcon(itemId);
        }
    }
}
