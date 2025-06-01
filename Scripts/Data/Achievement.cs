using UnityEngine;

[System.Serializable]
public class Achievement 
{
    public int Id; 
    public QuestType questType;
    public int progress;
    public int targetProgress;
    public bool isCompleted;
    public bool isPrizeTaken;

    public Achievement(int ýd, QuestType questType, int progress, int targetProgress)
    {
        Id = ýd;
        this.questType = questType;
        this.progress = progress;
        this.targetProgress = targetProgress;
    }

    public bool CheckQuestType(QuestType questType)
    {
        return this.questType == questType;
    }

    public void AddProgress(int progress) 
    {
        this.progress += progress;
        this.progress = Mathf.Min(this.progress, targetProgress);
        if (this.progress >= targetProgress) 
        {
            isCompleted = true;
        }
    }
    public void Refresh()
    {
        progress = 0;
        isCompleted = false;
        isPrizeTaken = false;
    }
}

