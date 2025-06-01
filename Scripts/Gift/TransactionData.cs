using MoreMountains.Feedbacks;
using System;
using UnityEngine;

[Serializable]
public class TransactionData
{
    public DealType DealType;

    [Header("Give")]
    public ResourceType ResourceType;
    public int amountToGive = 1;
    public bool spawnPrefab = false;
    public GameObject prefab;
    public int lifeTime = int.MaxValue;

    [Header("When Deal Type is 'Take' ")]
    public TakeableResourceType takeableResourceType;
    public bool takeAsRatio = true;
    [Range(0 , 100)][SerializeField]public float ratio_amountToTake = 20;
    public int amountToTake;

    [Header("UI")]
    public bool changeDeclarationAtRuntime = false;
    public string Name;
    public Sprite Icon;
    public string UI_Declaration;

    [Header("Feedbacks")]
    public MMF_Player performFeedbacks;
    public bool playFeedbacks = false;
    public void PerformTransaction(EconomyManager economyManager , PlayerHealth playerHealth)
    {
        switch (DealType)
        {
            case DealType.GiveToPlayer:
                economyManager.AddResource(ResourceType, amountToGive);
                break;
            case DealType.TakeFromPlayer:
                TakeResourceFromPlayer(economyManager, playerHealth);
                break;
        }
        if (playFeedbacks) performFeedbacks?.PlayFeedbacks();
    }
    private void TakeResourceFromPlayer(EconomyManager economyManager, PlayerHealth playerHealth)
    {
        switch (takeableResourceType)
        {
            case TakeableResourceType.Health:
                float effectToHealth = takeAsRatio ? playerHealth.GetCurrentHealth() * (ratio_amountToTake / 100) : amountToTake;
                playerHealth.IncreaseHealth((int)-effectToHealth);
                break;
            case TakeableResourceType.Coin:
                int effectToCoin = takeAsRatio ? (int)(economyManager.CurrentGold * (ratio_amountToTake / 100)) : (int)amountToTake;
                economyManager.SpendCoin(effectToCoin);
                break;
            case TakeableResourceType.Gem:
                int effectToGem = takeAsRatio ? (int)(economyManager.CurrentGold * (ratio_amountToTake / 100)) : (int)amountToTake;
                economyManager.SpendCoin(effectToGem);
                break;
            case TakeableResourceType.Relic:
                break;
        }
    }
}
public enum DealType
{
    GiveToPlayer,
    TakeFromPlayer
}

public enum TakeableResourceType
{
    Health,
    Coin,
    Gem,
    Relic
}

public struct Transactor
{
    public DealType Type;
    public int amount;
}