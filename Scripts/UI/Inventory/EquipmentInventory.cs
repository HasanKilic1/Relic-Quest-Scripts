using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentInventory : MonoBehaviour
{
    OwnedItemInventory ownedItemInventory;
    [Header("Close")]
    [SerializeField] Button closeButton;
    [SerializeField] GameObject toDestroy;
    [Header("Slots")]
    [SerializeField] ItemSlot itemSlotPrefab;
    [SerializeField] ItemSlot helmetSlot;
    [SerializeField] ItemSlot armorSlot;
    [SerializeField] ItemSlot gloveSlot;
    [SerializeField] ItemSlot necklaceSlot;
    [SerializeField] ItemSlot ringSlot;
    [SerializeField] ItemSlot shoesSlot;
    [Header("Attribute Info")]
    [SerializeField] Slider damageSlider;
    [SerializeField] Slider healthSlider;
    [SerializeField] Slider attackSpeedSlider;
    [SerializeField] Slider rangeSlider;
    [SerializeField] Slider lifeStealSlider;
    [SerializeField] Slider defenseSlider;
    [SerializeField] Slider abilityDamageSlider;
    [SerializeField] Slider movementSpeedSlider;
    [SerializeField] Slider criticalDamageChanceSlider;

    float damage;
    float health;
    float attackSpeed;
    float range;
    float lifeSteal;
    float defense;
    float abilityDamage;
    float movementSpeed;
    float criticalDamageChance;

    [Header("Max Slider Values")]
    [SerializeField] float damageMax;
    [SerializeField] float healthMax;
    [SerializeField] float attackSpeedMax;
    [SerializeField] float rangeMax;
    [SerializeField] float lifeStealMax;
    [SerializeField] float defenseMax;
    [SerializeField] float abilityDamageMax;
    [SerializeField] float movementSpeedMax;
    [SerializeField] float criticalDamageChanceMax;

    PlayerData playerData;
    private List<ItemSlot> staticSlots;

    private void OnEnable()
    {
        ItemEquipAndUpgrader.OnAnyItemUpgraded += UpdateEquipmentAttributeShowers;
        ItemEquipAndUpgrader.OnAnyItemEquipped += UpdateEquipmentAttributeShowers;
    }
    private void OnDisable()
    {
        ItemEquipAndUpgrader.OnAnyItemUpgraded -= UpdateEquipmentAttributeShowers;
        ItemEquipAndUpgrader.OnAnyItemEquipped -= UpdateEquipmentAttributeShowers;
    }
    void Start()
    {
        ownedItemInventory = FindAnyObjectByType<OwnedItemInventory>();
        playerData = SaveLoadHandler.Instance.GetPlayerData();

        staticSlots = new List<ItemSlot> { helmetSlot, armorSlot, gloveSlot, necklaceSlot, ringSlot, shoesSlot };
        staticSlots.ForEach(slot => { slot.MakeStatic(); });

        EquipSavedItems();
        SetupSliders();
        UpdateEquipmentAttributeShowers(0);
        ResetInventoryNotification();
        closeButton.onClick.AddListener(() => { Destroy(toDestroy); }); 
    }

    private void ResetInventoryNotification()
    {
        SaveLoadHandler.Instance.GetPlayerData().notification.InventoryNotification = false;
        NotificationSystem.Instance.ResetNotificationTrigger(NotificationSystem.Inventory_Trigger);
    }

    private void EquipSavedItems()
    {
        foreach (var item in ItemManager.Instance.GetEquippedItemSOs())
        {
            EquipItem(item);
        }        
    }

    private void SetupSliders()
    {
        damageSlider.maxValue = damageMax;
        healthSlider.maxValue = healthMax;
        attackSpeedSlider.maxValue = attackSpeedMax;
        rangeSlider.maxValue = rangeMax;
        lifeStealSlider.maxValue = lifeStealMax;
        defenseSlider.maxValue = defenseMax;
        abilityDamageSlider.maxValue = abilityDamageMax;
        movementSpeedSlider.maxValue = movementSpeedMax;
        criticalDamageChanceSlider.maxValue = criticalDamageChanceMax;
    }

    public void EquipItem(ItemSO itemSO)
    {
        // Only include the slot that has the same type of item
        ItemSlot relevantSlot = staticSlots.Find(slot => slot.staticItemType == itemSO.ItemType);
        if (relevantSlot != null)
        {
            if (relevantSlot.GetItemSO())
            {
                ItemData itemToUnequip = ItemManager.Instance.GetOwnedItemDataByID(relevantSlot.GetItemSO().Id);
                if (itemToUnequip != null) 
                {
                    itemToUnequip.isEquipped = false;
                    ownedItemInventory.UnequipItem(relevantSlot.GetItemSO());
                }
            }

            relevantSlot.SetupItemSlot(itemSO);           
            //Make new item as equipped
            ItemData data = ItemManager.Instance.GetOwnedItemDataByID(itemSO.Id);
            data.isEquipped = true;
            SaveLoadHandler.Instance.SaveData();
        }
        else Debug.LogError("Slot could not found!");

        UpdateEquipmentAttributeShowers(0);
        HandleSlotTextActivation();
    }

    private void HandleSlotTextActivation()
    {
        staticSlots.ForEach(slot => slot.GetItemLevelText().SetActive(true));
        var unusedSlots = staticSlots.Where(slot => slot.GetItemSO() == null).ToList();
        unusedSlots.ForEach(slot => slot.GetItemLevelText().SetActive(false));
    }


    private void UpdateEquipmentAttributeShowers(int id)
    {
        damage = 0f; health = 0f; attackSpeed = 0f; range = 0f; lifeSteal = 0f;
        defense = 0f; abilityDamage = 0f; movementSpeed = 0f; criticalDamageChance = 0f;

        foreach (var itemData in ItemManager.Instance.GetEquippedItems())
        {
            ItemSO itemSO = ItemManager.Instance.GetItemSOById(itemData.id);
            foreach(var influencer in itemSO.AttributeInfluencers)
            {
                float influence = influencer.effectOnAttributePerLevel * itemData.level;
                UpdateSliders(influencer.attributeType, influence);
            }
        }
    }
    
    private void UpdateSliders(AttributeType attributeType, float value)
    {
        switch (attributeType)
        {
            case AttributeType.Damage:
                damage += value;
                damageSlider.value = damage;
                break;
            case AttributeType.Health:
                health += value;
                healthSlider.value = health;
                break;
            case AttributeType.AttackSpeed:
                attackSpeed += value;
                attackSpeedSlider.value = attackSpeed;
                break;
            case AttributeType.Range:
                range += value;
                rangeSlider.value = range;
                break;
            case AttributeType.LifeSteal:
                lifeSteal += value;
                lifeStealSlider.value = 100;
                break;
            case AttributeType.Defense:
                defense += value;
                defenseSlider.value = defense;
                break;
            case AttributeType.AbilityDamage:
                abilityDamage += value;
                abilityDamageSlider.value = abilityDamage;
                break;
            case AttributeType.MovementSpeed:
                movementSpeed += value;
                movementSpeedSlider.value = movementSpeed;
                break;
            case AttributeType.CritChance:
                criticalDamageChance += value;
                criticalDamageChanceSlider.value = criticalDamageChance;
                break;
        }
    }
}
