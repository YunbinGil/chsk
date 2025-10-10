// ShopCellBuyButton.cs  (상점 프리팹에 부착)
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Managers;

public class ShopCellBuyButton : MonoBehaviour
{
    [SerializeField] private Button buyButton;   // ShopCell 안의 Buy 버튼
    [SerializeField] private string spiceId;     // 이 칸이 판매하는 향신료 ID

    void Reset()
    {
        if (!buyButton) buyButton = GetComponentInChildren<Button>();
    }

    void Awake()
    {
        if (buyButton) {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuy);
        }
    }

    void OnBuy()
    {
        var mgr = SpiceManager.Instance;
        if (mgr == null) return;

        var data = mgr.GetData(spiceId);
        if (data == null) return;

        // 슬롯이 꽉 찬 경우 TryBuy가 false를 리턴할 수 있음
        bool ok = mgr.TryBuy(data, 1);
        // ok 결과에 따라 사운드/토스트/UI 피드백 처리 권장
    }
}
