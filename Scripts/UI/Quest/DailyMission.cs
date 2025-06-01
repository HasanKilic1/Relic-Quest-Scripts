using System;

[Serializable]
public class DailyMission
{
    public DailyMissionType dailyMissionType;
    public ResourceType reward;
    public int rewardAmount;
    public int progress;
    public int target;           
}
public enum DailyMissionType
{
    TotalPlayed,
    FinishedLevel,
    OpenedChest,
    AdsWatched
}

