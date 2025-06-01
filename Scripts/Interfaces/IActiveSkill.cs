
public interface IActiveSkill 
{
    public void SetPlayerScript(PlayerStateMachine stateMachine);
    public void SetSkillData(int level , int abilityDamage); // every ability has it's base damage and player attributes will influence that
}
