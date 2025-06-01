using MoreMountains.Feedbacks;
using System;
using UnityEngine;

public class SkillUser : MonoBehaviour
{
    public static event Action OnAbilityFinished;
    PlayerStateMachine playerStateMachine;
    PlayerHealth playerHealth;
    Character character;

    [SerializeField] MonoBehaviour activeSkill;
    [SerializeField] MonoBehaviour passiveSkill;
    [SerializeField] private float skillExitTime = 1.5f;
    [SerializeField] MMF_Player skillFeedbacks;
    [SerializeField] bool closeInputWhileSkillPlays;
    [SerializeField] bool beInvulnerableWhileSkillPlays = true;
    public bool LookTargetWhileSkillPlays = true;
    public AbilityState AbilityState {  get; private set; }
    private float skillTimer = 0f;
    private int skillDamage;

    private void OnEnable()
    {
        InputReader.AbilityAction += UseSkill;
    }
    private void OnDisable()
    {
        InputReader.AbilityAction -= UseSkill;
    }

    private void Awake()
    {
        playerStateMachine = GetComponentInParent<PlayerStateMachine>();
        playerHealth = GetComponentInParent<PlayerHealth>();
        character = GetComponent<Character>();
        skillTimer = character.GetAbilityCooldown;
    }

    private void Start()
    {
        activeSkill = Instantiate(activeSkill, transform);
        MonoBehaviour _passiveSkill = Instantiate(passiveSkill, transform);
        (_passiveSkill as IPassiveSkill).SetPlayer(playerStateMachine);
    }

    private void Update()
    {
        skillTimer += Time.deltaTime;
    }

    public void UseSkill()
    {
        if (playerStateMachine.GetClosestEnemy() == null || !IsSkillReady) return;

        skillTimer = 0f;
        playerStateMachine.inputClosed = closeInputWhileSkillPlays;        
        playerHealth.CanTakeDamage = !beInvulnerableWhileSkillPlays;     
        EnterAbilityState();

        InGameUI.Instance.EnterAbilityCooldown(character.GetAbilityCooldown);
    }

    private void EnterAbilityState()
    {
        AbilityState = new AbilityState(playerStateMachine);
        AbilityState.SetAbility(activeSkill, character.Level, skillDamage);
        playerStateMachine.GetIntoNewState(AbilityState);
        Invoke(nameof(ExitFromAbilityState), skillExitTime);
        skillFeedbacks.PlayFeedbacks();
        skillTimer = 0f;
    }

    public void ExitFromAbilityState()
    {
        playerStateMachine.GetIntoNewState(new IdleState(playerStateMachine));
        OnAbilityFinished?.Invoke();
        playerStateMachine.inputClosed = false;
        playerHealth.CanTakeDamage = true;
    }

    public void SetSkillDamage(int skillDamage)
    {
        this.skillDamage = skillDamage;
    }
    public float GetSkillDamage => skillDamage;
    public void InfluenceSkillDamage(float influence)
    {
        skillDamage += (int)influence;
    }
    private bool IsSkillReady => skillTimer > character.GetAbilityCooldown;

    public MonoBehaviour GetAbility()
    {
        return activeSkill;
    }

   
}
