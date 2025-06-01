using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance {  get; private set; }
    [field: SerializeField] public List<Attribute> BasePlayerAttributes { get; private set; }
    [field:SerializeField] public List<Shooter> AllCharacters {  get; private set; }
    [SerializeField] public int selectedCharacterID;

    PlayerStateMachine playerStateMachine;
    PlayerHealth playerHealth;
    Character character;
    SkillUser skillUser;

    public int baseDamage;
    public float baseHealth;
    public float baseAttackSpeed;
    public float baseRange;

    private void Awake()
    {
        if(Instance != null && Instance != this) 
        {
            Destroy(gameObject);
        }
        else Instance = this;

        playerStateMachine = GetComponent<PlayerStateMachine>();
        playerHealth = GetComponent<PlayerHealth>();
    }
    private void Start()
    {
        OpenSelectedCharacter();
    }

    public void OpenSelectedCharacter()
    {
        selectedCharacterID = SaveLoadHandler.Instance.GetPlayerData().OwnedChampions.Find(c => c.isSelected).ID;       
        playerStateMachine.selectedCharacter = AllCharacters[selectedCharacterID].gameObject;
        playerStateMachine.SetSelectedCharacter(AllCharacters[selectedCharacterID].gameObject);
        for (int i = 0; i < AllCharacters.Count; i++)
        {
            if (i == selectedCharacterID) { AllCharacters[i].gameObject.SetActive(true); }
            else { AllCharacters[i].gameObject.SetActive(false); }
        }
        character = playerStateMachine.selectedCharacter.GetComponent<Character>();
        skillUser = playerStateMachine.selectedCharacter.GetComponent<SkillUser>();
        SetupAttributes();
    }
    
    private void SetupAttributes()
    {
        foreach (var attribute in SaveLoadHandler.Instance.GetPlayerData().playerAttributes)
        {
            switch (attribute.attributeType)
            {
                case AttributeType.Damage:
                    {
                        float damage = baseDamage + attribute.InfluenceOnAttribute;
                        character.SetDamage((int)(damage));
                        break;
                    }

                case AttributeType.Health:
                    playerHealth.SetStartHealth(baseHealth + attribute.InfluenceOnAttribute);
                    break;

                case AttributeType.AttackSpeed:
                    playerStateMachine.SetAttackSpeed(baseAttackSpeed + attribute.InfluenceOnAttribute);
                    break;

                case AttributeType.Range:
                    playerStateMachine.SetRange(baseRange + attribute.InfluenceOnAttribute);
                    break;

                case AttributeType.LifeSteal:
                    character.SetLifeSteal(attribute.InfluenceOnAttribute);
                    break;

                case AttributeType.Defense:
                    playerHealth.SetDefense(attribute.InfluenceOnAttribute);
                    break;

                case AttributeType.AbilityDamage:
                    skillUser?.SetSkillDamage((int)attribute.InfluenceOnAttribute);
                    break;

                case AttributeType.MovementSpeed:
                    playerStateMachine.InfluenceMovementSpeed(attribute.InfluenceOnAttribute);
                    break;

                case AttributeType.CritChance:
                    character.SetCriticalChance(attribute.InfluenceOnAttribute);
                    break;
            }
        }
    }
    public void InfluenceAttribute(AttributeType attributeType, float influence)
    {
        switch (attributeType)
        {
            case AttributeType.Damage:
                character.InfluenceDamage((int)influence);
                break;

            case AttributeType.Health:
                playerHealth.InfluenceMaxHealth((int)influence);     
                if(playerHealth.GetMaxHealth < baseHealth) // clamp player health , it can't be smaller than the base health
                {
                    float balanced = baseHealth - playerHealth.GetMaxHealth;
                    playerHealth.InfluenceMaxHealth((int)balanced);
                }
                break;

            case AttributeType.AttackSpeed:                
                playerStateMachine.InfluenceAttackSpeed(influence);
                break;

            case AttributeType.Range:
                playerStateMachine.InfluenceRange(influence);
                break;

            case AttributeType.LifeSteal:
                character.InfluenceRegeneration(influence);
                break;

            case AttributeType.Defense:
                playerHealth.InfluenceDefense(influence);
                break;

            case AttributeType.AbilityDamage:
                skillUser.InfluenceSkillDamage(influence);
                break;

            case AttributeType.MovementSpeed:
                playerStateMachine.InfluenceMovementSpeed(influence);
                break;

            case AttributeType.CritChance:
                character.InfluenceCriticalChance(influence);
                break;
        }

        HKDebugger.LogInfo("Attribute influenced : " + attributeType.ToString() + " by : " + influence);
    }

}
