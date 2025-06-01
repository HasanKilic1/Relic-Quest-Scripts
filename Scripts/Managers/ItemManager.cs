using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance {  get; private set; }

    [SerializeField] List<ItemSO> allItems;
    [SerializeField] bool isInCombatScene = true;
    PlayerData playerData;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        playerData = SaveLoadHandler.Instance.GetPlayerData();

        if(!isInCombatScene) { return; }
        foreach (var itemSO in GetOwnedItemSOs())
        {
            foreach(var influncer in itemSO.AttributeInfluencers)
            {
                float itemInfluence = influncer.effectOnAttributePerLevel * GetOwnedItemDataByID(itemSO.Id).level;
                if (PlayerController.Instance) PlayerController.Instance.InfluenceAttribute(influncer.attributeType, itemInfluence);
            }
        }
    }

    public List<ItemSO> GetEquippedItemSOs()
    {
        List<ItemSO> equippedItemSOs = new List<ItemSO>();
        foreach (var itemSO in allItems) 
        {
            if(SaveLoadHandler.Instance.GetPlayerData().OwnedItems.Any(itemData => itemData.id == itemSO.Id && itemData.isEquipped))
            {
                equippedItemSOs.Add(itemSO);
            }
        }
        return equippedItemSOs;
    }

    public List<ItemSO> GetOwnedItemSOs()
    {
        List<ItemSO> ownedItemSOs = new List<ItemSO>();
        foreach (var itemSO in allItems)
        {
            if (SaveLoadHandler.Instance.GetPlayerData().OwnedItems.Any(itemData => itemData.id == itemSO.Id))
            {
                ownedItemSOs.Add(itemSO);
            }
        }
        return ownedItemSOs;
    }

    public List<ItemSO> GetAllItems()
    {
        return allItems;
    }
    public ItemSO GetItemSOById(int id)
    {
        foreach (var itemSO in allItems)
        {
            if(itemSO.Id == id) return itemSO;
        }
        return null;
    }

    public void OwnItem(ItemSO itemSO)
    {
        if (!HasItem(itemSO.Id))
        {
            ItemData itemToOwn = new ItemData(itemSO.Id, 1, isEquipped: false);
            SaveLoadHandler.Instance.GetPlayerData().OwnedItems.Add(itemToOwn);
        }
    }

    public void OwnItem(int itemID)
    {
        if (!HasItem(itemID)) 
        {
            ItemData itemToOwn = new ItemData(itemID, 1, isEquipped: false);
            SaveLoadHandler.Instance.GetPlayerData().OwnedItems.Add(itemToOwn);
        }
        SaveLoadHandler.Instance.SaveData();
    }
    public ItemData GetOwnedItemDataByID(int id)
    {
        return SaveLoadHandler.Instance.GetPlayerData().OwnedItems.Find(item => item.id == id);
    }
    public List<ItemData> GetOwnedItems()
    {
        return SaveLoadHandler.Instance.GetPlayerData().OwnedItems;
    }

    public List<ItemData> GetEquippedItems()
    {
        return SaveLoadHandler.Instance.GetPlayerData().OwnedItems.Where(itemData => itemData.isEquipped).ToList();
    }
    bool HasItem(int id) => SaveLoadHandler.Instance.GetPlayerData().OwnedItems.Any(i => i.id == id);
}
