using UnityEngine;
using UnityEngine.UI;
using chsk.UI.IceMaker;
using chsk.GamePlay.Production;
using chsk.UI.Common;
using chsk.Core.Data;
using chsk.Core.Services;
using TMPro;

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
        [SerializeField] private TMP_Text bubbleText;
        
        private GameObject _imStatusPanelInstance;
        private CurrencyManager currencyManager = CurrencyManager.Instance;
        private IceMakerManager imManager = IceMakerManager.Instance;
        

        void Awake()
        {
            if (iceMakerPanel == null)
                iceMakerPanel = GameObject.Find("Panel.IceMaker");
            if (bubbleButton == null)
                bubbleButton = GetComponent<Button>();
            if (!controller) controller = GetComponentInParent<IceProductionController>(true); 
            bubbleButton.onClick.AddListener(OnBubbleClicked);
            ApplyStatusToUI(controller ? controller.Status : IceProductionController.ProdStatus.Idle);
        }

        void OnEnable()
        {
            if (controller != null)
            {
                controller.OnStatusChanged += ApplyStatusToUI;
                ApplyStatusToUI(controller.Status);
            }
        }
        void OnDisable()
        {
            if (controller != null)
                controller.OnStatusChanged -= ApplyStatusToUI;
        }

        /// <summary>버블이 클릭되면 이 본체를 현재 선택으로 등록하고, Idle이면 선택 패널을 열어준다.</summary>
        public void OnBubbleClicked()
        {
            if (controller)
            {
                IceMakerUIContext.SetCurrent(controller);
                Debug.Log($"[Bubble] Selected controller = {controller.name} ({controller.GetInstanceID()})", controller);

                // === Quick Produce Mode: 같은 메뉴 바로 생산 ===
                if (IceMakerUIContext.QuickModeActive
                    && controller.Status == IceProductionController.ProdStatus.Idle)
                {
                    var itemId = IceMakerUIContext.QuickItemId;
                    if (!string.IsNullOrEmpty(itemId) && imManager != null)
                    {
                        // 결제부터 시도 (돈 부족하면 false)
                        if (imManager.TryGenerate(itemId))
                        {
                            // 결제 OK → 바로 생산
                            controller.SetItemId(itemId);
                            var data = imManager.GetData(itemId);
                            int sec = data != null ? data.time.ToSeconds() : 1;
                            controller.BeginProduction(sec);
                            return; // 패널 열지 않고 끝
                        }
                        else
                        {
                            // 돈이 부족하면 패널 열어서 안내/다른 메뉴 선택하도록
                            Debug.Log("[Bubble] QuickMode generate failed (not enough currency) → open panel");
                            // 퀵모드 유지할지 끌지는 취향인데, 보통 유지
                        }
                    }
                }
                
                // === 일반 흐름 (퀵모드 아니거나 Idle이 아닌 경우) ===
                switch (controller.Status)
                {
                    case IceProductionController.ProdStatus.Idle:
                        iceMakerPanel.GetComponent<UIPanel>().Open();
                        SetChildrenActive(true);
                        break;
                    case IceProductionController.ProdStatus.Generating:
                        ShowStatusPanel(); // 진행중 패널 켜기
                        break;
                    case IceProductionController.ProdStatus.Done:
                        currencyManager.Add(CurrencyType.Ice, imManager.GetPrdIce(controller.ItemId)); // TODO: 재화 추가
                        controller.Status = IceProductionController.ProdStatus.Idle; // 완료 → 대기
                        Debug.Log($"[Bubble] Status set to Idle, iceImg.activeSelf={iceImg.activeSelf}");
                        break;
                }
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
            Debug.Log($"ApplyStatusToUI: {status}, iceImg.activeSelf={iceImg.activeSelf}, frame={Time.frameCount}");
            if (!iceMakerPanel) return;

            switch (status)
            {
                case IceProductionController.ProdStatus.Idle:
                    iceImg.SetActive(false);
                    bubbleText.text = "...";
                    DestroyStatusPanelInstanceIfAny();
                    break;

                case IceProductionController.ProdStatus.Generating:
                    SetChildrenActive(false);
                    bubbleText.text = "~.~";
                    iceImg.SetActive(false);
                    break;

                case IceProductionController.ProdStatus.Done:
                    iceImg.GetComponent<Image>().sprite = imManager.GetIcon(controller.ItemId);
                    iceImg.SetActive(true);
                    DestroyStatusPanelInstanceIfAny();
                    break;
            }
            Debug.Log($"ApplyStatusToUI: {status}, iceImg.activeSelf={iceImg.activeSelf}, frame={Time.frameCount}");
        }

        void SetChildrenActive(bool active)
        {
            foreach (Transform child in iceMakerPanel.transform)
            {
                child.gameObject.SetActive(active);
            }
        }

        /// <summary>상태 패널 인스턴스가 없으면 생성하고, 현재 itemId 바인딩</summary>
        void ShowStatusPanel()
        {
            if (!imStatusPanelPrefab || !iceMakerPanel) return;

            // 1) 없으면 생성
            if (_imStatusPanelInstance == null)
                _imStatusPanelInstance = Instantiate(imStatusPanelPrefab, iceMakerPanel.transform.parent);

            if (_imStatusPanelInstance == null) return;

            // 2) 바인딩(아이템 아이디 갱신은 매번 보장해주는 편이 안전)
            var binder = _imStatusPanelInstance.GetComponent<IMStatusPanelBinder>();
            if (binder && controller != null)
                binder.SetItemId(controller.ItemId);

            // 3) 다시 열기 (UIPanel 있으면 Open, 없으면 SetActive)
            var panel = _imStatusPanelInstance.GetComponent<UIPanel>();
            if (panel != null) panel.Open();
            else _imStatusPanelInstance.SetActive(true);
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