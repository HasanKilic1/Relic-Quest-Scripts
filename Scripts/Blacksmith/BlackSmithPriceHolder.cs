using UnityEngine;

public class BlackSmithPriceHolder : MonoBehaviour
{
    public PriceType priceType;
    public int price;
    public string saveKey;
    public bool CanDemand()
    {
        if(this.priceType == PriceType.Ads)
        {
            return true;
        }
        else if(this.priceType == PriceType.Gem)
        {
            return EconomyManager.Instance.HasEnoughGem(GetPrice());
        }
        else if(this.priceType == PriceType.Coin)
        {
            return EconomyManager.Instance.HasEnoughCoin(GetPrice());
        }
        return false;
    }

    public void Spend()
    {
        if (this.priceType == PriceType.Ads)
        {
            //SHOW ADS
        }
        else if (this.priceType == PriceType.Gem)
        {
            EconomyManager.Instance.SpendGem(GetPrice());
        }
        else if (this.priceType == PriceType.Coin)
        {
            EconomyManager.Instance.SpendCoin(GetPrice());
        }
        PlayerPrefs.SetInt(saveKey, PurchaseCount + 1);
        HKDebugger.LogInfo(this.name + " purchased " + PurchaseCount + " times");
    }
    public int GetPrice() => price * PurchaseCount;
    public int PurchaseCount => PlayerPrefs.GetInt(saveKey);
}
public enum PriceType
{
    Coin,
    Gem,
    Ads
}
