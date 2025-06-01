using System;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public event Action<int> OnGoldChanged;
    public event Action<int> OnGemChanged;
    public event Action<int> OnEtherealChanged;
    public static EconomyManager Instance {  get; private set; }

    public int CurrentGold;
    public int CurrentGem;
    public int CurrentSilverKey;
    public int CurrentGoldenKey;
    public int CurrentEtherealStone;

    public int EarnedMoneyFromLevel = 0;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else Instance = this;
    }

    private void Start()
    {
        CurrentGold = SaveLoadHandler.Instance.GetPlayerData().gold;
        CurrentGem = SaveLoadHandler.Instance.GetPlayerData().gem;
        CurrentSilverKey = SaveLoadHandler.Instance.GetPlayerData().silverKey;
        CurrentGoldenKey = SaveLoadHandler.Instance.GetPlayerData().goldenKey;
        CurrentEtherealStone = SaveLoadHandler.Instance.GetPlayerData().etherealStone;

        Invoke(nameof(InvokeEvents), .1f);    
    }

    private void InvokeEvents()
    {
        OnGoldChanged?.Invoke(CurrentGold);
        OnGemChanged?.Invoke(CurrentGem);
        OnEtherealChanged?.Invoke(CurrentEtherealStone);
    }    

    public void AddResource(ResourceType resourceType , int amount , bool earnedFromLevel = false)
    {
        switch (resourceType)
        {
            case ResourceType.Coin:
                AddCoin(amount);
                if (earnedFromLevel)
                {
                    EarnedMoneyFromLevel += amount;
                }
                break;
            case ResourceType.Gem:
                AddGem(amount);
                break;
            case ResourceType.SilverKey:
                AddSilverKey(amount);
                break;
            case ResourceType.GoldenKey:
                AddGoldenKey(amount);
                break;
            case ResourceType.EtherealStone:
                AddEtherealStone(amount);
                break;
        }
        SaveLoadHandler.Instance.SaveData();
    }

    public void TrySpendResource(ResourceType resourceType , int amount , out bool isValid)
    {
        isValid = false;
        switch (resourceType)
        {
            case ResourceType.Coin:
                isValid = HasEnoughCoin(amount);
                if (isValid)
                {
                    SpendCoin(amount);
                }
                break;

            case ResourceType.Gem:
                isValid = HasEnoughGem(amount);
                if (isValid)
                {
                    SpendGem(amount);
                }
                break;

            case ResourceType.SilverKey:
                isValid = HasEnoughSilverKey(amount);
                if (isValid)
                {
                    SpendSilverKey(amount);
                }
                break;

            case ResourceType.GoldenKey:
                isValid = HasEnoughGoldenKey(amount);
                if (isValid)
                {
                    SpendGoldenKey(amount);
                }
                break;

            case ResourceType.EtherealStone:
                isValid = HasEnoughEtherealStone(amount);
                if (isValid)
                {
                    SpendEtherealStone(amount);
                }
                break;

            case ResourceType.Relic:
                break;
        }
    }
    #region BoilerPlateChecks
    public void AddCoin(int amount)
    {
        CurrentGold += amount;
        SaveLoadHandler.Instance.GetPlayerData().gold = CurrentGold;
        OnGoldChanged?.Invoke(CurrentGold);
    }
    public void SpendCoin(int amount)
    {
        CurrentGold -= amount;
        SaveLoadHandler.Instance.GetPlayerData().gold = CurrentGold;
        AchievementManager.Instance.EffectQuestByType(QuestType.SpendCoin, amount);
        OnGoldChanged?.Invoke(CurrentGold);
    }
    public bool HasEnoughCoin(int amountToSpend) => CurrentGold >= amountToSpend;

    public void AddGem(int amount)
    {
        CurrentGem += amount;
        SaveLoadHandler.Instance.GetPlayerData().gem = CurrentGem;
        OnGemChanged?.Invoke(CurrentGem);
    }
    public void SpendGem(int amount)
    {
        CurrentGem -= amount;
        SaveLoadHandler.Instance.GetPlayerData().gem = CurrentGem;
        OnGemChanged?.Invoke(CurrentGem);
    }

    public bool HasEnoughGem(int amountToSpend) => CurrentGem >= amountToSpend;

    public void AddSilverKey(int amount)
    {
        CurrentSilverKey += amount;
        SaveLoadHandler.Instance.GetPlayerData().silverKey = CurrentSilverKey;
    }
    public void SpendSilverKey(int amount)
    {
        CurrentSilverKey -= amount;
        SaveLoadHandler.Instance.GetPlayerData().silverKey = CurrentSilverKey;
    }

    public bool HasEnoughSilverKey(int amountToSpend) => CurrentSilverKey >= amountToSpend;

    public void AddGoldenKey(int amount)
    {
        CurrentGoldenKey += amount;
        SaveLoadHandler.Instance.GetPlayerData().goldenKey = CurrentGoldenKey;
    }
    public void SpendGoldenKey(int amount)
    {
        CurrentGoldenKey -= amount;
        SaveLoadHandler.Instance.GetPlayerData().goldenKey = CurrentGoldenKey;
    }

    public bool HasEnoughGoldenKey(int amountToSpend) => CurrentGoldenKey >= amountToSpend;

    public void AddEtherealStone(int amount)
    {
        CurrentEtherealStone += amount;
        SaveLoadHandler.Instance.GetPlayerData().etherealStone = CurrentEtherealStone;
        OnEtherealChanged?.Invoke(CurrentEtherealStone);
    }
    public void SpendEtherealStone(int amount)
    {
        CurrentEtherealStone -= amount;
        SaveLoadHandler.Instance.GetPlayerData().etherealStone = CurrentEtherealStone;
        OnEtherealChanged?.Invoke(CurrentEtherealStone);
    }
    
    public bool HasEnoughEtherealStone(int amountToSpend) => CurrentEtherealStone >= amountToSpend;

    #endregion;

}
public enum ResourceType
{
    Coin,
    Gem,
    SilverKey,
    GoldenKey,
    EtherealStone,
    Purchase,
    Relic
}
