using System;
using UnityEngine;

[Serializable]
public class Attribute
{
    public AttributeType attributeType;
    public int attributeLevel;
    public float influencePerLevel;
    public int maxLevel;
    public Attribute(AttributeType attributeType, int attributeLevel, float influencePerLevel)
    {
        this.attributeType = attributeType;
        this.attributeLevel = attributeLevel;
        this.influencePerLevel = influencePerLevel;
    }

    public float InfluenceOnAttribute => influencePerLevel * attributeLevel;
}
public enum AttributeType
{
    Damage,
    Health,
    AttackSpeed,
    Range,
    LifeSteal,
    Defense,
    AbilityDamage,
    MovementSpeed,
    CritChance
}
