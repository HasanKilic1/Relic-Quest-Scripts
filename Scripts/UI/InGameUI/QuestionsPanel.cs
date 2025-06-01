using Assets.Scripts.UI.Visualization;
using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionsPanel : MonoBehaviour
{
    [SerializeField] List<Question> Questions;
    [Header("Question and Answers")]
    [SerializeField] TextMeshProUGUI questionSentence;
    [SerializeField] Button A;
    [SerializeField] Button B;
    [SerializeField] TextMeshProUGUI A_Text;
    [SerializeField] TextMeshProUGUI B_Text;

    [Header("Timer")]
    [SerializeField] TextMeshProUGUI timer;
    [SerializeField] Image timerImage;

    [Header("Prizes")]
    [SerializeField] Slider prizeSlider;
    [SerializeField] Image prizeImage;
    [SerializeField] TextMeshProUGUI prizeText;
    [SerializeField] Sprite goldSprite;
    [SerializeField] Sprite gemSprite;
    [SerializeField] Sprite etherealStoneSprite;

    [SerializeField] List<MultipleResource> givableResourceLists;

    [Header("Feedbacks")]
    [SerializeField] ImageColorizer A_Colorizer;
    [SerializeField] ImageColorizer B_Colorizer;
    [SerializeField] MMF_Player rightAnswerFeedbacks;

    [Header("Result Panel")]
    [SerializeField] GameObject resultPanel;
    [SerializeField] Image rewardImage;
    [SerializeField] TextMeshProUGUI rewardText;
    [SerializeField] Button exitButton;

    private List<ResourceData> givableResourceList;

    private int answeredQuestions;
    private float remainingTime = 15;
    private float remainingTimeMax;
    private bool rightAnswerIsA;
    private bool canCount = true;
    private ResourceData[] currentPrizes;
    private void Awake()
    {
        remainingTimeMax = remainingTime;
        givableResourceList = new List<ResourceData>();
       
    }
    void Start()
    {
        AdjustPrizeUI();
        ShowNewPrize();
        ShowNewQuestion();
        

        A.onClick.AddListener(CheckA_Answer);
        B.onClick.AddListener(CheckB_Answer);
    }

    private void AdjustPrizeUI()
    {
        prizeSlider.value = 0;
        prizeSlider.maxValue = 3;

        int ind = UnityEngine.Random.Range(0, givableResourceLists.Count);
        currentPrizes = givableResourceLists[ind].ResourceArr;
    }

    void Update()
    {
        if (!canCount) return;

        remainingTime -= Time.deltaTime;
        timerImage.fillAmount = remainingTime / remainingTimeMax;      
        timer.text = Convert.ToInt32(remainingTime).ToString();
        if (remainingTime <= 0) 
        {
            StartCoroutine(FinishRoutine());
        }
    }

    private void ShowNewQuestion()
    {
        UnityEngine.Random.InitState(RandomSeeder.GetSeed());
        Question question = Questions[UnityEngine.Random.Range(0 , Questions.Count)];
        Questions.Remove(question);
        rightAnswerIsA = UnityEngine.Random.Range(0, 100) < 50;

        questionSentence.text = question.QuestionSentence;

        A_Text.text = rightAnswerIsA ? question.GetRandomRightAnswer : question.GetRandomWrongAnswer;
        B_Text.text = rightAnswerIsA ? question.GetRandomWrongAnswer : question.GetRandomRightAnswer;
    }

    private void ShowNewPrize()
    {
        ResourceData prize = currentPrizes[answeredQuestions];
        switch (prize.ResourceType)
        {
            case ResourceType.Coin:
                prizeImage.sprite = goldSprite;
                break;
            case ResourceType.Gem:
                prizeImage.sprite = gemSprite;
                break;
            case ResourceType.EtherealStone:
                prizeImage.sprite = etherealStoneSprite;
                break;
        }
        prizeText.text = prize.Quantity.ToString();
    }
   
    private void CheckA_Answer()
    {
        if (rightAnswerIsA) 
        {
            A_Colorizer.Colorize(Color.white, Color.green, 0.3f);
            HandleRightAnswer();
        }
        else
        {
            B_Colorizer.Colorize(Color.white, Color.red, 0.3f);
            HandleWrongAnswer();
        }
    }

    private void CheckB_Answer()
    {
        if (!rightAnswerIsA)
        {
            B_Colorizer.Colorize(Color.white, Color.green, 0.3f);
            HandleRightAnswer();
        }
        else
        {
            B_Colorizer.Colorize(Color.white, Color.red, 0.3f);
            HandleWrongAnswer();
        }
    }

    private void HandleRightAnswer()
    {
        answeredQuestions++;
        prizeSlider.value++;
        rightAnswerFeedbacks?.PlayFeedbacks();
        if (answeredQuestions < 3)
        {
            StartCoroutine(QuestionsRefreshRoutine());
            ShowNewPrize();
        }
        else { StartCoroutine(FinishRoutine()); }
    }

    private void HandleWrongAnswer()
    {
        StartCoroutine(FinishRoutine());
    }

    private void Finish()
    {
        A.interactable = false;
        B.interactable = false;
        canCount = false;
        resultPanel.SetActive(true);

        RewardType rewardType = RewardType.Gold;
        int prizeIndex = answeredQuestions - 1;
        prizeIndex = Mathf.Max(0, prizeIndex);
        switch (currentPrizes[prizeIndex].ResourceType)
        {
            case ResourceType.Coin:
                rewardType = RewardType.Gold;
                break;
            case ResourceType.Gem:
                rewardType = RewardType.Gem;
                break;
            case ResourceType.EtherealStone:
                rewardType = RewardType.EtherealStone;
                break;
        }
        rewardImage.sprite = prizeImage.sprite;
        rewardText.text = "You've gained " + currentPrizes[prizeIndex].Quantity.ToString() + " " + rewardType.ToString();
        exitButton.onClick.AddListener(() => { Destroy(gameObject); });
    }

    private IEnumerator QuestionsRefreshRoutine()
    {
        A.interactable = false;
        B.interactable = false;
        yield return new WaitForSeconds(0.5f);
        A.interactable = true;
        B.interactable = true;
        ShowNewQuestion();
    }

    private IEnumerator FinishRoutine()
    {
        A.interactable = false;
        B.interactable = false;
        canCount = false;

        yield return new WaitForSeconds(0.5f);
        Finish();
    }

}
[Serializable]
public class Question
{
    public string QuestionSentence;
    public string[] RightAnswers;
    public string[] WrongAnswers;

    public string GetRandomRightAnswer => RightAnswers[UnityEngine.Random.Range(0, RightAnswers.Length)];
    public string GetRandomWrongAnswer => WrongAnswers[UnityEngine.Random.Range(0, RightAnswers.Length)];
}
