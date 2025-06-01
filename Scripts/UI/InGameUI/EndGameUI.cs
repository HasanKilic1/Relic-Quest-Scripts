using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] LootController lootController;
    [SerializeField] GameObject resultPanel;

    [Header("Next level")]
    [SerializeField] string mainMenuName;
    [Header("Buttons")]
    [SerializeField] Button claimButton;
    [SerializeField] Button claim2xButton;

    [Header("Texts")]
    [SerializeField] TextMeshProUGUI headerText;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] TextMeshProUGUI gemText;
    [SerializeField] TextMeshProUGUI etherealStoneText;

    [Header("Visuals")]
    [SerializeField] List<GameObject> stars;
    [SerializeField] List<GameObject> loots;
    
    [Header("Feedbacks")]
    [SerializeField] MMF_Player initializationFeedbacks;
    [SerializeField] TextCounter coinCounter;
    [SerializeField] TextCounter gemCounter;
    [SerializeField] TextCounter etherealStoneCounter;

    [SerializeField] GameObject itemPanel;

    private void Start()
    {
        claimButton.onClick.AddListener(Claim);
        claim2xButton.onClick.AddListener(Claim2x);
    }
    public void Show()
    {
        if (initializationFeedbacks) initializationFeedbacks.PlayFeedbacks();
        if (GameStateManager.Instance != null) 
        {
            headerText.text ="PROGRESSION: "+ GameStateManager.Instance.CurrentLevel.ToString() + " / " + GameStateManager.Instance.LastLevelOfMap;
        }
        MMTimeManager.Instance.SetTimeScaleTo(0f);
        resultPanel.SetActive(true);
        itemPanel.SetActive(lootController.GetGiveableItem() != null);
        
        ShowStars();
        ShowLoots();
        CountTexts();
    }

    private void ShowStars()
    {
        List<GameObject> starList = new List<GameObject>();
        int showableStarCount = 0;
        if (GameStateManager.Instance.CurrentLevel <= GameStateManager.Instance.LastLevelOfMap / 3) showableStarCount = 1;
        if (GameStateManager.Instance.CurrentLevel > GameStateManager.Instance.LastLevelOfMap / 3) showableStarCount = 2;
        if (GameStateManager.Instance.CurrentLevel >= GameStateManager.Instance.LastLevelOfMap) showableStarCount = 3;
        for (int i = 0; i < showableStarCount; i++)
        {
            starList.Add(stars[i]);
        }
        StartCoroutine(EnableRoutine(starList , 0.2f));
    }

    private void ShowLoots()
    {
        List<GameObject> lootList = new List<GameObject>(loots);
        if (!lootController.GetGiveableItem())
        {
            lootList.Remove(itemPanel);
        }
        StartCoroutine(EnableRoutine(lootList, 0.5f));
    }
    private IEnumerator EnableRoutine(List<GameObject> objectList , float timeBetween)
    {
        float t = 0f;
        for (int i = 0; i < objectList.Count; i++) 
        {
            while (t <= timeBetween) 
            {
                t += Time.unscaledDeltaTime;                
                yield return null;
            }

            t = 0f;
            objectList[i].SetActive(true);
            if (objectList[i].TryGetComponent(out ScaleAnimationer scaleAnimationer))
            {
                scaleAnimationer.TimeScaleType = TimeScaleMode.Unscaled;
                scaleAnimationer.Perform();
            }
        }
        yield return null;
    }

    private void CountTexts()
    {
        int coinCountTo = lootController.GetCoin();
        int gemCountTo = lootController.GetGem();
        int etherealStoneCountTo = lootController.GetEtherealStone();

        coinCounter.StartCounting(0, coinCountTo, 2.5f);
        gemCounter.StartCounting(0, gemCountTo, 2.5f);
        etherealStoneCounter.StartCounting(0, etherealStoneCountTo, 2.5f);
    }

    private void Claim()
    {
        lootController.Give();
        claimButton.interactable = false;
        claim2xButton.interactable = false;

        FinishGame();
        SwitchLoadingScreenScene();
    }
    private void Claim2x()
    {
        lootController.Give();
        lootController.Give();
        claimButton.interactable = false;
        claim2xButton.interactable = false;
        FinishGame();
        SwitchLoadingScreenScene();
    }

    private void SwitchLoadingScreenScene()
    {
        SceneLoader.NextScene = SceneName.MainMenu;
        SceneManager.LoadScene(SceneName.LoadingScreenScene.ToString());
    }

    private void FinishGame()
    {
        GameStateManager.Instance.FinishGame(true);
    }
}
