using System;
using UnityEngine;

[CreateAssetMenu(fileName ="ItemSO" , menuName ="Item Scriptable Object")]
public class ItemSO : ScriptableObject , IComparable<ItemSO>
{
    [System.Serializable]
    public struct AttributeInfluencer
    {
        public AttributeType attributeType;
        public float effectOnAttributePerLevel;
    }

    public int Id;
    public string Name;
    public string Description;
    public Sprite Icon;
    public ItemType ItemType;
    public Rarity Rarity;
    public AttributeInfluencer[] AttributeInfluencers;
    public int UpgradeCostMultiplierPerLevel;

    public int CompareTo(ItemSO other)
    {
        return Rarity.CompareTo(other.Rarity);
    }
}
public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
public enum ItemType
{
    Armor,
    Helmet,
    Ring,
    Necklace,
    Gloves,
    Shoes,
    Undefined
}