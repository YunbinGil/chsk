using Game.Managers;
using Game.Currencies;
using UnityEngine;
using UnityEngine.Events;


public class ShopBuyBtnBase : MonoBehaviour
{
    [Header("가격")]
    public int price = 0;
    [Header("결제 타입 (파생 클래스에서 고정)")]
    [SerializeField] protected CurrencyType payType = CurrencyType.Gold;

    [Header("구매 이벤트")]
    public UnityEvent OnPurchased;     // 성공 시 (아이템 지급/연출 등 연결)
    public UnityEvent OnInsufficient;  // 실패 시 (팝업/사운드 등 연결)


    public void OnClickBuy()
    {
        if (price <= 0)
        {
            // 가격 0이면 그냥 성공 처리 (무료 지급)
            OnPurchased?.Invoke();
            return;
        }
        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("[ShopBuyBtnBase] CurrencyManager.Instance가 없음. 씬에 CurrencyManager 프리팹 배치했는지 확인!");
            OnInsufficient?.Invoke();
            return;
        }
        bool ok = CurrencyManager.Instance.TrySpend(payType, price);

        if (ok)
        {
            // 여기서 실제 아이템 지급/언락 로직을 연결하거나,
            // UnityEvent(OnPurchased)에 인스펙터로 연결해도 됨
            OnPurchased?.Invoke();
        }
        else
        {
            OnInsufficient?.Invoke();
        }
    }
}
