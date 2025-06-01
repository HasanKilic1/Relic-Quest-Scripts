using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldPanelController : MonoBehaviour
{
    [SerializeField] Button questUIButton;
    [SerializeField] GameObject questUI;
    [SerializeField] Button dailyMissionUIButton;
    [SerializeField] GameObject dailyRewardsUI;

    [Header("Question Panel")]
    [SerializeField] Button questionPanelButton;
    [SerializeField] GameObject questionPanelUI;
    [SerializeField] TextMeshProUGUI remainingTimeText;
    private DateTime questionRefreshTime;
    void Start()
    {
        dailyMissionUIButton.onClick.AddListener(OpenDailyRewardUI);
        questUIButton.onClick.AddListener(OpenQuestPanelUI);
        questionPanelButton.onClick.AddListener(OpenQuestionPanel);
    }

    private void Update()
    {
        RefreshQuestionPanelDate();
    }

    private void RefreshQuestionPanelDate()
    {
        string questionRefreshDate = SaveLoadHandler.Instance.GetPlayerData().QuestionPanelRefreshDate;
        questionRefreshTime = DateTime.Parse(questionRefreshDate);

        TimeSpan remaining = questionRefreshTime - DateTime.UtcNow;
        if (remaining > TimeSpan.Zero)
        {
            questionPanelButton.interactable = false;
            remainingTimeText.text = TimeSpanFormatter.FormatTimeSpanHours(remaining);
        }
        else
        {
            questionPanelButton.interactable = true;
            remainingTimeText.text = "READY";
        }
    }

    private void OpenDailyRewardUI()
    {
        Instantiate(dailyRewardsUI,this.transform);
    }
    private void OpenQuestPanelUI()
    {
        Instantiate(questUI,this.transform);
    }

    private void OpenQuestionPanel()
    {
        Instantiate(questionPanelUI, this.transform);
        SaveLoadHandler.Instance.GetPlayerData().QuestionPanelRefreshDate = DateTime.UtcNow.AddMinutes(120).ToString();
        SaveLoadHandler.Instance.SaveData();
    }
}
