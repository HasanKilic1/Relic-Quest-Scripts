using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;

public class ComboCounter : MonoBehaviour
{
    [SerializeField] MMF_Player countFeedBacks;
    [SerializeField] MMF_Player enableFeedBacks;
    [SerializeField] MMF_Player disableFeedBacks;

    [SerializeField] Slider comboSlider;
    private int counter;
    
    private bool enableFeedBacksPlayed = false;
    [SerializeField] private float cooldown = 3f;
    private float timeToDisable;

   
    void Update()
    {
        if(enabled && Time.time > timeToDisable)
        {
            Disable();
        }
        HandleSlider();
    }

    private void HandleSlider()
    {
        comboSlider.value -= (comboSlider.maxValue / cooldown) * Time.deltaTime;
    }

    private void StartCounting(object sender, int countPlus)
    {        
        Enable();
        HandleTextCounting(countPlus);
    }

    private void HandleTextCounting(int countPlus)
    {
        MMF_TMPCountTo counterMMF = countFeedBacks.GetFeedbackOfType<MMF_TMPCountTo>();
        counterMMF.CountFrom = counter;
        counter += countPlus;
        counterMMF.CountTo = counter;

        countFeedBacks.PlayFeedbacks();
    }

    private void Enable()
    {        
        if(!enableFeedBacksPlayed)
        {
            enableFeedBacks.PlayFeedbacks();
            enableFeedBacksPlayed = true;
        }
        comboSlider.value = comboSlider.maxValue;
        timeToDisable = Time.time + cooldown;
    }

    private void Disable()
    {
        disableFeedBacks.PlayFeedbacks();
        counter = 0;
        enableFeedBacksPlayed = false;
    }
}
