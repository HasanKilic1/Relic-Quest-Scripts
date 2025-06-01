using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GameStateManager: MonoBehaviour
{
    [field:SerializeField] public GamePlatform GamePlatform;
    [field: SerializeField] public int SceneID {  get; private set; }
    public static GameStateManager Instance { get; private set; }

    public static event Action<int> OnLevelStart;
    public static event Action<int> OnLevelFinished;
    public static event Action OnMapFinished;

    [field: SerializeField] public int LastLevelOfMap { get; private set; }
    [field: SerializeField] public int CurrentLevel { get; private set; }
    [field: SerializeField] public int MapExpMultiplier {  get; private set; } = 1;
    [SerializeField] public int[] GiftLevels;
    [SerializeField] public int[] CardLevels;
    [SerializeField] public int[] BlackSmithLevels;
    [SerializeField] public int[] BossLevels;

    private void Awake()
    {
        Time.timeScale = 1.0f;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        CurrentLevel = 0;
    }

    private void Start()
    {
        StartNewLevel();
        CallTinySauceStartEvent();
    }

    private void CallTinySauceStartEvent()
    {
        int startLevel = SceneID * 30 + 1;
        TinySauce.OnGameStarted(startLevel);
        Debug.Log("Tiny sauce start event called..");
    }

    public void StartNewLevel()
    {
        CurrentLevel++;
        OnLevelStart?.Invoke(CurrentLevel);
        EnemySpawnManager.Instance.StartNewLevel();
    }

    public void FinishLevel()
    {
        OnLevelFinished?.Invoke(CurrentLevel);
        if (CurrentLevel >= LastLevelOfMap)
        {
            StartCoroutine(SuccessfullFinishRoutine(2f));
        }
        else
        {
            if (MustShowGift)
            {
                GiftType giftType = GiftType.None;
                if (ShouldShowAbilityCard) {
                    giftType = GiftType.AbilityCard;
                }
                if (ShouldShowBlackSmith)
                {
                    giftType = GiftType.BlackSmith;
                }
                if (ShouldShowGift)
                {
                    giftType = GiftType.Standard;
                }
                GiftManager.Instance.Show(giftType);
            }
            else
            {
                StartNewLevel(); 
            }
        }
    }

    private IEnumerator SuccessfullFinishRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        FinishGame(true);
    }

    public void FinishGame(bool isSuccess)
    {
        InGameUI.Instance.OpenEndGameUI();
        if (isSuccess)
        {
            SaveLoadHandler.Instance.GetPlayerData().openedMapIDs.Add(SceneID + 1);
        }

        CallTinySauceFinishEvent(isSuccess);
        MMTimeManager.Instance.SetTimeScaleTo(0f);
        OnLevelFinished?.Invoke(CurrentLevel);
        OnMapFinished?.Invoke();
    }

    private void CallTinySauceFinishEvent(bool isSuccess)
    {
        int score = CurrentLevel;
        TinySauce.OnGameFinished(isSuccess, score);
        Debug.Log("Tiny sauce finish event called. Score: " + score);
    }
    public bool MustShowGift => ShouldShowGift || ShouldShowAbilityCard || ShouldShowBlackSmith;
    public bool ShouldShowGift => GiftLevels.Contains(CurrentLevel);
    public bool ShouldShowAbilityCard => CardLevels.Contains(CurrentLevel);
    public bool ShouldShowBlackSmith => BlackSmithLevels.Contains(CurrentLevel);
    public int GetExp => CurrentLevel * 250 * MapExpMultiplier;
}
public enum GamePlatform
{
    Mobile,
    PC
}