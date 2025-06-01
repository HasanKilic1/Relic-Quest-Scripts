using Lofelt.NiceVibrations;
using MoreMountains.Feedbacks;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RouletteController : MonoBehaviour
{
    public static event Action<int> OnSpinFinished;
    public static event Action OnSpinFinishedCompletely;
    public static event Action OnClosed;

    public int TotalRouletteSpinned = 0;

    [SerializeField] Button spinButton;
    [SerializeField] GameObject backGround;
    [SerializeField] GameObject content;
    [SerializeField] float spinSpeedMin = 5f;
    [SerializeField] float spinSpeedMax = 10f;
    [SerializeField] float spinTimeMin = 3f;
    [SerializeField] float spinTimeMax = 5f;
    [SerializeField] Button closeButton;
    [SerializeField] TextMeshProUGUI totalSpinnedText;
    [SerializeField] TextMeshProUGUI cost;
    bool isSpinning = false;
    bool readyToSpin = false;
    [SerializeField] MMF_Player initFeedbacks;
    [SerializeField] MMUIShaker UIShaker;
    [SerializeField] float shakeStopTime = 1.5f;
    void Start()
    {
        spinButton.onClick.AddListener(Spin);
        closeButton.onClick.AddListener(Close);
        RefreshTexts();
        initFeedbacks?.PlayFeedbacks();
        UIShaker.Shaking = false;
        Invoke(nameof(StartShaking), 0.5f);
        Invoke(nameof(StopShaking), shakeStopTime);
        Invoke(nameof(GetReadyToSpin), 2f);
    }

    private void Update()
    {
        spinButton.interactable = CanSpin;
    }
    
    private void GetReadyToSpin()
    {
        readyToSpin = true;
    }

    private void StartShaking()
    {
        UIShaker.Shaking = true;
    }

    public void StopShaking()
    {
        UIShaker.Shaking = false;
    }

    public void Spin()
    {
        StartCoroutine(SpinRoutine());
    }


    private IEnumerator SpinRoutine()
    {
        isSpinning = true;

        int anglePerSec = 45;  
        int spinSpeed = Convert.ToInt32(UnityEngine.Random.Range(spinSpeedMin, spinSpeedMax));
        int spinTime = Convert.ToInt32(UnityEngine.Random.Range(spinTimeMin, spinTimeMax));
        float t = 0f;

        while(t < spinTime)
        {
            t+= Time.deltaTime;
            Rotate(anglePerSec , spinSpeed);
            yield return null;
        }        
        int totalRotated = (anglePerSec * spinSpeed * spinTime) % 360;
        OnSpinFinished?.Invoke(totalRotated);

        yield return new WaitForSeconds(2f);

        isSpinning = false;
        OnSpinFinishedCompletely?.Invoke();
        RefreshTexts();
        Close();
    }

    private void IncreaseSpinnedRoulette()
    {
        TotalRouletteSpinned++;
        int maxSpinForARun = 4;
        if (TotalRouletteSpinned > maxSpinForARun)
        {
            return;
        }
    }


    private void Rotate(float angle , float spinSpeed)
    {
        backGround.transform.Rotate(new Vector3(0, 0, angle * Time.deltaTime * spinSpeed));
        content.transform.Rotate(new Vector3(0, 0, angle * Time.deltaTime * spinSpeed));
    }

    private void Close()
    {
        GameStateManager.Instance.FinishLevel();
        OnClosed?.Invoke();
        Destroy(gameObject);
    }

    private bool CanSpin => EconomyManager.Instance.HasEnoughGem(GetCost()) && !isSpinning && readyToSpin;
    private void RefreshTexts()
    {
        cost.text = GetCost().ToString();
        totalSpinnedText.text = "Total Spinned : " + TotalRouletteSpinned.ToString();
    }
    private int GetCost()
    {
        if(TotalRouletteSpinned == 0) return 0;

        int pow = Mathf.Max(TotalRouletteSpinned, 1);
        int multiplier = (int)Mathf.Pow(3, pow);
        int cost = 50 * multiplier;
        return cost;
    }
}
