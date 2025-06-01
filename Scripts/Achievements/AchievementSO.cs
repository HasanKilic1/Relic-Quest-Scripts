using System.Collections.Generic;
using UnityEngine;

[System.Serializable]   
[CreateAssetMenu(fileName ="Achievement SO" , menuName = "Achievement Scriptable Object")]
public class AchievementSO : ScriptableObject
{
    public int id;
    public QuestType QuestType;
    public QuestPrizer Prize;

    public int currentProgress;
    public int targetedProgress;
    public int prize;

    #region editor
    public void ResetProgress()
    {
        currentProgress = 0;
    }

    public void CompleteProgress()
    {
        currentProgress = targetedProgress;
    }
    #endregion
}

[System.Serializable]
public class QuestPrizer
{
    public ResourceType prizeType;
    public int prizeAmount;
}

public enum QuestType
{
    CompleteLevel,
    DestroyEnemies,
    RegenerateHealth,
    TakenDamageFromEnemies,
    TotalDeathCount,
    SpendCoin,
    MakeSkillUpgrades
}
