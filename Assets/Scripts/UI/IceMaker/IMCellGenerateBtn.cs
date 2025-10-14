//IMCellGenerateBtn.cs
using UnityEngine;
using UnityEngine.UI;
using chsk.Core.Data;
using chsk.Core.Services;

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
            if (genButton)
            {
                genButton.onClick.RemoveAllListeners();
                genButton.onClick.AddListener(() =>
                {
                    if (imManager == null || binder == null) return;
                    var ok = imManager.TryGenerate(binder.ItemId);
                    // 필요하면 여기서 사운드/토스트 추가
                });
            }
        }

    }
}