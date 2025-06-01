using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class PlayerData
{
    public string appVersion;
    public User User;

    public int gold;
    public int gem;
    public int silverKey;
    public int goldenKey;
    public int etherealStone;
    public int levelProgress;
    public int totalUpgrades;
    public int selectedChampionID = 0;
    public int continuousLoginDays = 1;
    public List<int> rewardTakenDays;
    public List<Champion> OwnedChampions;
    public List<Achievement> Achievements;
    public List<RelicData> Relics;
    public List<ItemData> OwnedItems;

    public List<int> openedMapIDs;

    public string LastSaveTime;
    public Notification notification;

    //Dates
    public string QuestionPanelRefreshDate;
    public string DailyLoginDate;
    public PlayerData()
    {
        notification = new Notification(); 
    }

    public List<Attribute> playerAttributes;
    public Attribute GetPlayerAttributeByType(AttributeType attributeType)
    {
        foreach (Attribute attribute in playerAttributes)
        {
            if(attribute.attributeType == attributeType)
            {
                return attribute;
            }
        }
        return null;
    }


    public void InitializeFirstTime(List<Attribute> attributes , List<RelicSO> relics , List<ItemSO> items , List<ChampionSO> champions,List<AchievementSO> achievementSOs)
    {
        User = new User(null , 0 , 0);
        gold = 5000;
        gem = 250;
        silverKey = 0;
        goldenKey = 0;
        etherealStone = 100;
        levelProgress = 0;
        totalUpgrades = 0;
        continuousLoginDays = 10;
        
        InitializeAttributes(attributes);
        InitializeItems();
        InitializeQuests(achievementSOs);
        InitializeChampions(champions);
        InitializeRelics(relics);
        
        QuestionPanelRefreshDate = DateTime.UtcNow.ToString();
        DailyLoginDate = DateTime.UtcNow.ToString();

        rewardTakenDays = new List<int>();
        openedMapIDs = new List<int> { 0,1 };
        notification = new Notification();
    }


    private void InitializeAttributes(List<Attribute> attributes)
    {
        playerAttributes = new List<Attribute>();
        foreach (Attribute attribute in attributes)
        {
            this.playerAttributes.Add(attribute);
        }
    }

    private void InitializeRelics(List<RelicSO> relics)
    {
        Relics = new List<RelicData>();
        foreach (var relic in relics)
        {
            RelicData relicData = new(relic.ID, 1, relic.Name, Rarity.Common);           
            Relics.Add(relicData);
        }
    }

    private void InitializeChampions(List<ChampionSO> champions)
    {
        OwnedChampions = new List<Champion>();
        foreach (var championSO in champions)
        {
            Champion champion = new Champion(championSO.ID, 1);
            if(championSO.ID == 0)
            {
                OwnedChampions.Add(champion);
            }                                                            
            champion.isSelected = champion.ID == 0; // update first champion as selected
        }
    }

    private void InitializeQuests(List<AchievementSO> achievementSOs)
    {
        Achievements = new List<Achievement>();

        foreach (var achievementSO in achievementSOs)
        {
            Achievement achievement = new Achievement(achievementSO.id, achievementSO.QuestType, 0, achievementSO.targetedProgress);
            Achievements.Add(achievement);
        }
    }

    private void InitializeItems()
    {
        OwnedItems = new List<ItemData>();
    }

    public void MatchWithOldVersion(PlayerData oldDataVersion ,string newVersion, List<AchievementSO> allAchievements) 
    {
        appVersion = newVersion;
        CopyOldVersion(oldDataVersion);
        ProtectAchievements(oldDataVersion , allAchievements);
        HandleNewVersionDifferences();
    }

    private void CopyOldVersion(PlayerData oldDataVersion)
    {
        gold = oldDataVersion.gold;
        gem = oldDataVersion.gem;
        silverKey = oldDataVersion.silverKey;
        goldenKey = oldDataVersion.goldenKey;
        etherealStone = oldDataVersion.etherealStone;
        levelProgress = oldDataVersion.levelProgress;
        totalUpgrades = oldDataVersion.totalUpgrades;

        playerAttributes = new List<Attribute>(oldDataVersion.playerAttributes);
        OwnedChampions = new List<Champion>(oldDataVersion.OwnedChampions);
        OwnedItems = new List<ItemData>(oldDataVersion.OwnedItems);
        openedMapIDs = new List<int>(oldDataVersion.openedMapIDs);

        QuestionPanelRefreshDate = oldDataVersion.QuestionPanelRefreshDate;
        DailyLoginDate = oldDataVersion.DailyLoginDate;

        rewardTakenDays = new List<int>(oldDataVersion.rewardTakenDays);
        openedMapIDs = new List<int>(oldDataVersion.openedMapIDs);
        notification = oldDataVersion.notification;
    }

    private void ProtectAchievements(PlayerData oldDataVersion , List<AchievementSO> allAchievements)
    {
        Achievements = new List<Achievement>();
        Achievements.AddRange(oldDataVersion.Achievements);

        List<int> oldVersionAchievementIDs = new List<int>();
        oldDataVersion.Achievements.ForEach(achievement => oldVersionAchievementIDs.Add(achievement.Id));
        
        // this list is added in new version
        List<AchievementSO> newlyAddedAchievements = allAchievements.Where(achievementSO => !oldVersionAchievementIDs.Contains(achievementSO.id)).ToList();

        foreach (var achievementSO in newlyAddedAchievements)
        {
            Achievement newAchievement = new Achievement(achievementSO.id,achievementSO.QuestType ,0, achievementSO.targetedProgress);
            Achievements.Add(newAchievement);
        }
        HKDebugger.LogInfo("ADDED ACHIEVEMENT COUNT IN NEW VERSION: " + newlyAddedAchievements.Count);
    }

    private void HandleNewVersionDifferences() // This could be needed if a new class ,List etc should be initialized
    {
        //Ex: BattlePassDate = DateTime.Utc.Now().ToString();
        //Ex: RelicFuser = new RelicFuser();
    }
}
