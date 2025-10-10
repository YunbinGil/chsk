using Game.Currencies;

public class GoldBuyBtn : ShopBuyBtnBase
{
    void Reset()  { payType = CurrencyType.Gold; }
    void Awake()  { payType = CurrencyType.Gold; }
}
