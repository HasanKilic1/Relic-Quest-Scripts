using UnityEngine;

public class PlayerInfluencerObject : MonoBehaviour
{
    Influencer influencer;

    public InfluenceType influenceType;
    [SerializeField] float totalDuration;
    [SerializeField] int totalDamage;
    [SerializeField] int loops = 1;

    private void OnValidate()
    {
        if(loops < 1) loops = 1;
    }

    private void Start()
    {
        influencer = PlayerHealth.Instance.GetComponent<Influencer>();
    }

    public void ApplyContact()
    {
        influencer.TakeDamageOverTime(totalDamage , totalDuration, loops , influenceType);
    }
    public void ApplyContactNonParameter()
    {
        influencer.TakeDamageOverTime(totalDamage, totalDuration:5, loops:5, influenceType);
    }
}
public enum InfluenceType
{
    None,
    Flame,
    Poison,
    Electricity
}
