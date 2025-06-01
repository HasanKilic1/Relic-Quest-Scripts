using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RewardGiver : MonoBehaviour
{
    [Serializable]
    public class ItemRewardGiver
    {
        public Rarity ItemRarity;
        public float chanceMin;
        public float chanceMax;
        public bool isRandomized;
        public ItemSO assignedItem;
        public bool IsInInterval(float random) => random >= chanceMin && random < chanceMax;
       
        public void GiveItemReward()
        {
            ItemSO rewardItem = isRandomized ? GetRandomItem() : assignedItem;
            ItemManager.Instance.OwnItem(rewardItem);

            HKDebugger.LogSuccess($"Item added : {rewardItem.Name}");
            
            MainMenu.Instance.ShowRewardVisual(RewardType.Item, 1, rewardItem);
            SaveLoadHandler.Instance.SaveData();
            if (NotificationSystem.Instance != null)
            {
                NotificationSystem.Instance.TriggerNotification(NotificationSystem.Inventory_Trigger);
            }
        }
        public ItemSO GetRandomItem()
        {
            var items = ItemManager.Instance.GetAllItems().Where(item => item.Rarity == ItemRarity).ToList();
            ItemSO randomItem = items[UnityEngine.Random.Range(0, items.Count - 1)];
            return randomItem;
        }
    }
    [Serializable]
    public class ResourceGiver
    {
        public RewardType rewardType;
        public int amount;
        public void GiveReward()
        {
            if (rewardType == RewardType.Gold) { EconomyManager.Instance.AddCoin(amount); }
            else if (rewardType == RewardType.Gem) { EconomyManager.Instance.AddGem(amount); }
            else if (rewardType == RewardType.SilverKey) { EconomyManager.Instance.AddSilverKey(amount); }
            else if (rewardType == RewardType.GoldenKey) { EconomyManager.Instance.AddGoldenKey(amount); }
            else if (rewardType == RewardType.EtherealStone) { EconomyManager.Instance.AddEtherealStone(amount); }

            MainMenu.Instance.ShowRewardVisual(rewardType,amount,null);
            SaveLoadHandler.Instance.SaveData();

        }
    }

    [SerializeField] Button openButton;
 
    [Header("Cost")]
    [SerializeField] int cost;
    [SerializeField] ResourceType costType;

    [Header("Prizes")]
    [SerializeField] List<ResourceGiver> resourceGivers;
    [SerializeField] List<ItemRewardGiver> itemGivers;
    public bool canGivePrize = true;
    private void Start()
    {
       openButton.onClick.AddListener(GiveReward);        
    }

    private void Update()
    {
        openButton.interactable = canGivePrize && IsInteractableOnCost();
    }
    private void GiveReward()
    {
        SpendResource();
        GiveResources();
        GiveItems();
    }

    private void GiveResources()
    {
        foreach (var resourceGiver in resourceGivers)
        {
            resourceGiver.GiveReward();
        }
    }

    private void GiveItems()
    {
        int rand = UnityEngine.Random.Range(0, 100);
        ItemRewardGiver randomGiver = null;
        foreach (var itemGiver in itemGivers)
        {
            if (itemGiver.IsInInterval(rand))
            {
                randomGiver = itemGiver;
                break;
            }
        }
        randomGiver?.GiveItemReward();

        foreach (var itemGiver in itemGivers)
        {
            if (!itemGiver.isRandomized) itemGiver.GiveItemReward();
        }
    }
    private void SpendResource()
    {
        EconomyManager.Instance.TrySpendResource(costType, cost, out bool isValid);
    }
    private bool IsInteractableOnCost()
    {
        if (cost == 0) return true;
        switch (costType)
        {
            case ResourceType.Coin:
                return EconomyManager.Instance.HasEnoughCoin(cost);
            case ResourceType.Gem:

                return EconomyManager.Instance.HasEnoughGem(cost);
            case ResourceType.SilverKey:

                return EconomyManager.Instance.HasEnoughSilverKey(cost);
            case ResourceType.GoldenKey:

                return EconomyManager.Instance.HasEnoughGoldenKey(cost);
            case ResourceType.EtherealStone:

                return EconomyManager.Instance.HasEnoughEtherealStone(cost);
            default:
                break;
        }
        return false;
    }
}
