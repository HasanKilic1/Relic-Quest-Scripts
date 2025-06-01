using TMPro;
using UnityEngine;

public class EconomyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI gemText;
    [SerializeField] TextMeshProUGUI etherealStoneText;
    public bool useNumberFormat = false;

    private void OnEnable()
    {
        if(EconomyManager.Instance != null)
        {
            EconomyManager_OnGoldChanged(EconomyManager.Instance.CurrentGold);
            EconomyManager_OnGemChanged(EconomyManager.Instance.CurrentGem);
            EconomyManager_OnEtherealChanged(EconomyManager.Instance.CurrentEtherealStone);
        }
    }
    void Start()
    {
        EconomyManager.Instance.OnGoldChanged += EconomyManager_OnGoldChanged;
        EconomyManager.Instance.OnGemChanged += EconomyManager_OnGemChanged;
        EconomyManager.Instance.OnEtherealChanged += EconomyManager_OnEtherealChanged;
    }
   
    private void EconomyManager_OnGoldChanged(int gold)
    {
        if (useNumberFormat)
        {
            moneyText.text = NumberFormatter.GetDisplay(gold);
        }
        else moneyText.text = gold.ToString();
    }
    private void EconomyManager_OnGemChanged(int gem)
    {
        if (useNumberFormat) 
        {
            gemText.text = NumberFormatter.GetDisplay(gem);
        }
        else gemText.text = gem.ToString();
    }

    private void EconomyManager_OnEtherealChanged(int etherealStone)
    {
        if (etherealStoneText == null) return;
        etherealStoneText.text = etherealStone.ToString();
    }

    private void OnDestroy()
    {
        if(EconomyManager.Instance != null)
        {
            EconomyManager.Instance.OnGoldChanged -= EconomyManager_OnGoldChanged;
            EconomyManager.Instance.OnGemChanged -= EconomyManager_OnGemChanged;
            EconomyManager.Instance.OnEtherealChanged -= EconomyManager_OnEtherealChanged;
        }
    }
}
