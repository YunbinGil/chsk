//IMCellGenerateBtn.cs
using UnityEngine;
using UnityEngine.UI;
using chsk.Core.Data;
using chsk.Core.Services;
using chsk.UI.Bubble;
using chsk.UI.IceMaker;
using chsk.GamePlay.Production;

namespace chsk.UI.IceMaker
{
    public class IMCellGenerateBtn : MonoBehaviour
    {
        [SerializeField] private Button genButton;
        [SerializeField] private IMCellCatalogBinder binder;

        private IceMakerManager imManager;


        void Reset()
        {
            if (!genButton) genButton = GetComponentInChildren<Button>(true);
            if (!binder) binder = GetComponent<IMCellCatalogBinder>();
        }

        void Awake()
        {
            if (imManager == null)
            {
#if UNITY_2022_3_OR_NEWER
            imManager = FindAnyObjectByType<chsk.Core.Services.IceMakerManager>(FindObjectsInactive.Include);
#else
                imManager = FindObjectOfType<IceMakerManager>();
#endif  
            }
            if (imManager == null) imManager = IceMakerManager.Instance;
            if (genButton)
            {
                genButton.onClick.RemoveAllListeners();
                genButton.onClick.AddListener(OnClickGenerate);
            }
        }

        void OnClickGenerate()
        {
            if (imManager == null || binder == null) return;

            var ok = imManager.TryGenerate(binder.ItemId);
            if (!ok) return;

            var ctrl = IceMakerUIContext.CurrentController;
            if (!ctrl) ctrl = GetComponentInParent<IceProductionController>(true);
            
            if (!ctrl)
            {
                Debug.LogWarning("[IMCellGenerateBtn] No IceProductionController found (placedTool not ready?)");
                return;
            }
            ctrl.SetItemId(binder.ItemId);

            var data = imManager.GetData(binder.ItemId);
            int sec = data != null ? data.time.ToSeconds() : 0;   // Duration → 초
            if (sec <= 0) sec = 1;

            ctrl.BeginProduction(sec);
        }

    }
}