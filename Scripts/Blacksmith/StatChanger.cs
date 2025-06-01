using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(BlackSmithPriceHolder))]
public class StatChanger : MonoBehaviour , IBlacksmithComponent
{
    BlackSmithController blackSmithController;
    [Header("Camera")]
    public Vector3 CamFollowOffset;
    public Vector3 CamLookOffset;
    public Transform followTransform;
    [Header("Attributes")]
    public AttributeType attributeType;
    [Range(0,100)]public float influenceRatio;

    [Header("UI")]
    public TextMeshProUGUI AttributeText;
    public TextMeshProUGUI InfluenceText;

    [Header("Feedbacks")]
    [SerializeField] MMF_Player selectionFeedbacks;
    private void Awake()
    {
        blackSmithController = GetComponentInParent<BlackSmithController>();
    }

    private void Start()
    {
        AttributeText.text = attributeType.ToString();
        InfluenceText.text = $"+ %{influenceRatio}";
    }

    public void Select()
    {
        Debug.Log("Stat bought : " + attributeType.ToString() + " " + GetInfluence().ToString());
        PlayerController.Instance.InfluenceAttribute(attributeType , GetInfluence());
        selectionFeedbacks?.PlayFeedbacks();
    }

    private float GetInfluence()
    {
        float influence = 0f;
        Character character = PlayerController.Instance.GetComponent<PlayerStateMachine>().selectedCharacter.GetComponent<Character>();
        switch (attributeType)
        {
            case AttributeType.Damage:                
                influence = character.GetDamage * influenceRatio / 100;
                PlayerController.Instance.InfluenceAttribute(attributeType, influence);
                break;

            case AttributeType.Health:
                int maxHealthBefore = (int)PlayerHealth.Instance.GetMaxHealth;

                influence = PlayerHealth.Instance.GetMaxHealth * influenceRatio / 100;
                PlayerHealth.Instance.InfluenceMaxHealth(influence);

                int maxHealthNow = (int)PlayerHealth.Instance.GetMaxHealth;
                break;

            case AttributeType.AttackSpeed:
                influence = PlayerController.Instance.GetComponent<PlayerStateMachine>().GetAttackSpeed * influenceRatio / 100;
                break;

            case AttributeType.Range:
                influence = PlayerController.Instance.GetComponent<PlayerStateMachine>().GetRange * influenceRatio / 100;
                break;

            case AttributeType.LifeSteal:
                break;

            case AttributeType.Defense:
                influence = PlayerHealth.Instance.GetDefense * influenceRatio / 100;
                break;

            case AttributeType.AbilityDamage:
                break;
            case AttributeType.MovementSpeed:
                break;
            case AttributeType.CritChance:
                influence = character.GetCritChance * influenceRatio / 100;
                
                break;
        }
        return influence;
    }
   
}
