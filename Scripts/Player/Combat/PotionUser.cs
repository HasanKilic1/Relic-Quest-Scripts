using System;
using System.Collections;
using UnityEngine;

public class PotionUser : MonoBehaviour
{
    public static event Action<PotionSO> OnPotionSettled;
    public PotionSO currentPotionSO;
    private Potion currentPotion;
    private int potionCount;
    private float currentPotionCooldown;
    private float potionTimer;

    private void OnEnable()
    {
        InputReader.PotionAction += TryUseCurrentPotion;

    }
    private void OnDestroy()
    {
        InputReader.PotionAction -= TryUseCurrentPotion;
    }

    public void AddPotion(PotionSO potionSO)
    {
        OnPotionSettled?.Invoke(potionSO);

        if(currentPotionSO != potionSO)
        {
            if (currentPotion != null) 
            {
                Destroy(currentPotion.gameObject);
            }
            currentPotionSO = potionSO;
            currentPotion = Instantiate(potionSO.Potion);
            potionCount = 1;
        }
        else { potionCount++; }
       
        currentPotionCooldown = potionSO.Cooldown;

        if (currentPotion != null) 
        {
            HKDebugger.LogSuccess("New potion added : " + potionSO.Name);            
        }
        else
        {
            Debug.LogError("Selected potionSO doesn't have any potion prefab : " +  potionSO.Name);
        }
        InGameUI.Instance.SetPotionText(potionCount);
    }

    private void TryUseCurrentPotion()
    {
        if (CanUsePotion)
        { 
            currentPotion.UsePotion(this);
            
            potionCount--;
            InGameUI.Instance.SetPotionText(potionCount);

            potionTimer = Time.time + currentPotionCooldown;
            InGameUI.Instance.EnterPotionCooldown(currentPotionSO.Cooldown);
            if (currentPotion.TryGetComponent(out IResettablePotion resettablePotion)) 
            { 
                StartCoroutine(PotionResetRoutine(resettablePotion));
            }

            if(potionCount == 0) { 
                InGameUI.Instance.DeactivatePotionUI();
            }
        }
    }

    private bool CanUsePotion => Time.time > potionTimer && currentPotion != null && potionCount > 0;
   
    private IEnumerator PotionResetRoutine(IResettablePotion resettablePotion)
    {
        yield return new WaitForSeconds(currentPotionSO.EffectDuration);
        resettablePotion.ResetPotionEffect();
    }
}
