using MoreMountains.Feedbacks;
using UnityEngine;

public class Character : MonoBehaviour
{
    [field:SerializeField] public int Level {  get; private set; }

    int damage;
    float lifeSteal;
    float criticalDamageChance;
    [SerializeField] float abilityCooldown = 5f;
    public GameObject model;
    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public void SetLifeSteal(float lifeSteal)
    {
        this.lifeSteal = lifeSteal;
    }

    public void SetCriticalChance(float criticalChance)
    {
        this.criticalDamageChance = criticalChance;
    }

    public void InfluenceDamage(int influence)
    {
        damage += influence;
    }
    public void InfluenceRegeneration(float influence)
    {
        lifeSteal += influence;
    }

    public void InfluenceCriticalChance(float influence)
    {
        criticalDamageChance += influence;
    }

    public void InfluenceAbilityCooldown(float influence)
    {
        this.abilityCooldown += influence;
    }

    public float GetDamage => this.damage;
    public float GetCritChance => this.criticalDamageChance;
    public float GetLifeSteal => this.lifeSteal;
    public float GetAbilityCooldown => this.abilityCooldown;
}
