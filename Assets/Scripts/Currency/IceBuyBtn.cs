using Game.Currencies;

public class IceBuyBtn : ShopBuyBtnBase
{
    void Reset()  { payType = CurrencyType.Ice; }
    void Awake()  { payType = CurrencyType.Ice; }
}
