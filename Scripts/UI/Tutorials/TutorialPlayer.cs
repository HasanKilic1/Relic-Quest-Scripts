using MoreMountains.Feedbacks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPlayer : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Button nextButton;
    [SerializeField] MMF_Player showFeedbacks;
    private int tutorialIndex;

    private List<Tutorial> tutorialList;
    private void Start()
    {
        tutorialList = new List<Tutorial>();
        nextButton.onClick.AddListener(Continue);
    }
    
    public void PlayTutorialsFromScratch(List<Tutorial> tutorials)
    {
        tutorialIndex = 0;
        tutorialList = tutorials;
        Continue();
    }
    private void Continue()
    {
        if (tutorialIndex >= tutorialList.Count)
        {
            Hide();
            return;
        }

        showFeedbacks?.PlayFeedbacks();
       
        Show(tutorialList[tutorialIndex]);
        tutorialIndex++;
    }
    private void Show(Tutorial sequence)
    {
        panel.SetActive(true);
        text.text = sequence.Sentence;
        if (sequence.Highlighter != null) 
        {
            Hide();
            sequence.Highlighter.HighLight();
            sequence.Highlighter.OnHighlightComplete.AddListener(Continue);
        }
    }

    private void Hide()
    {
        panel.SetActive(false);
    }
}
[Serializable]
public class Tutorial
{
    public string Sentence;
    public Highlighter Highlighter;
}

