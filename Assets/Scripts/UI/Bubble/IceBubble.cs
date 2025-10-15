using UnityEngine;
using UnityEngine.UI;
using chsk.UI.IceMaker;
using chsk.GamePlay.Production;

namespace chsk.UI.Bubble
{
    public class IceBubble : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private IceProductionController controller; // placedTool 본체 컨트롤러
        [SerializeField] private GameObject iceMakerPanel;           // 카탈로그 패널(선택 UI)
        [SerializeField] private GameObject imStatusPanelPrefab;     // 상태 패널 프리팹(진행/남은시간 등)
        [SerializeField] private Button bubbleButton;                // 버블(클릭용 버튼)
        [SerializeField] private GameObject iceImg;
        private GameObject _imStatusPanelInstance;
        

        void Awake()
        {
            if (iceMakerPanel == null)
                iceMakerPanel = GameObject.Find("Panel.IceMaker");
            if (bubbleButton == null)
                bubbleButton = GetComponent<Button>();
            if (!controller) controller = GetComponentInParent<IceProductionController>(true);

            bubbleButton.onClick.AddListener(OnBubbleClicked);
            // 초기 UI 스냅샷
            ApplyStatusToUI(controller ? controller.Status : IceProductionController.ProdStatus.Idle);
        }

        void OnEnable()
        {
            if (controller != null)
                controller.OnStatusChanged += ApplyStatusToUI;
        }
        void OnDisable()
        {
            if (controller != null)
                controller.OnStatusChanged -= ApplyStatusToUI;
        }

        /// <summary>버블이 클릭되면 이 본체를 현재 선택으로 등록하고, Idle이면 선택 패널을 열어준다.</summary>
        void OnBubbleClicked()
        {
            if (controller)
            {
                IceMakerUIContext.SetCurrent(controller);
                 Debug.Log($"[Bubble] Selected controller = {controller.name} ({controller.GetInstanceID()})", controller);
            }
            if (controller && controller.Status == IceProductionController.ProdStatus.Idle)
            {
                if (iceMakerPanel) iceMakerPanel.SetActive(true);
                SetChildrenActive(true);
                if (iceImg) iceImg.SetActive(false);
            }
        }

        /// <summary>컨트롤러 → 버블 주입(구독 해제/재구독 + 즉시 상태 반영)</summary>
        public void SetController(IceProductionController ctrl)
        {
            if (controller == ctrl) return;

            if (controller != null)
                controller.OnStatusChanged -= ApplyStatusToUI;

            controller = ctrl;

            if (controller != null)
            {
                ApplyStatusToUI(controller.Status);
                controller.OnStatusChanged += ApplyStatusToUI;
            }
            else
            {
                ApplyStatusToUI(IceProductionController.ProdStatus.Idle);
            }
        }
        public void SetItemId(string newId)
        {
            if (controller) controller.SetItemId(newId);
        }


        /// <summary>본체 상태가 바뀔 때마다 UI 반영</summary>
        void ApplyStatusToUI(IceProductionController.ProdStatus status)
        {
            if (!iceMakerPanel) return;

            switch (status)
            {
                case IceProductionController.ProdStatus.Idle:
                    iceMakerPanel.SetActive(true);
                    SetChildrenActive(true);
                    if (iceImg) iceImg.SetActive(false);
                    DestroyStatusPanelInstanceIfAny();
                    break;

                case IceProductionController.ProdStatus.Generating:
                    iceMakerPanel.SetActive(false);
                    SetChildrenActive(false);
                    if (iceImg) iceImg.SetActive(false);
                    EnsureStatusPanelInstance(); // 진행중 패널 켜기
                    break;

                case IceProductionController.ProdStatus.Done:
                    iceMakerPanel.SetActive(false);
                    SetChildrenActive(false);
                    if (iceImg) iceImg.SetActive(true);
                    DestroyStatusPanelInstanceIfAny();
                    break;
            }
        }

        void SetChildrenActive(bool active)
        {
            foreach (Transform child in iceMakerPanel.transform)
            {
                child.gameObject.SetActive(active);
            }
        }

        /// <summary>상태 패널 인스턴스가 없으면 생성하고, 현재 itemId 바인딩</summary>
        void EnsureStatusPanelInstance()
        {
            if (_imStatusPanelInstance || !imStatusPanelPrefab || !iceMakerPanel) return;

            _imStatusPanelInstance = Instantiate(imStatusPanelPrefab, iceMakerPanel.transform.parent);

            // 상태 패널의 바인더에 현재 아이템 id 전달
            var binder = _imStatusPanelInstance.GetComponent<IMStatusPanelBinder>();
            if (binder && controller != null)
                binder.SetItemId(controller.ItemId);
        }
        
        /// <summary>상태 패널 인스턴스 제거</summary>
        void DestroyStatusPanelInstanceIfAny()
        {
            if (_imStatusPanelInstance)
            {
                Destroy(_imStatusPanelInstance);
                _imStatusPanelInstance = null;
            }
        }

    }
}