using System.Collections.Generic;
using UnityEngine;

public class GiftManager : MonoBehaviour
{
    public static GiftManager Instance { get; private set; }
    [SerializeField] List<MonoBehaviour> gifts;
    [SerializeField] Transform blackSmithPos;
    [SerializeField] BlackSmithController blackSmith;
    [SerializeField] List<string> blacksmithPriceKeys;
    [SerializeField] List<GameObject> onlyOpenOnBlacksmith;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
            Destroy(gameObject);

        foreach (var key in blacksmithPriceKeys)
        {
            PlayerPrefs.SetInt(key, 1);
        }
    }

    public void Show(GiftType giftType)
    {
        if (giftType == GiftType.None) return;

        if (giftType == GiftType.AbilityCard)
        {
            Invoke(nameof(ShowAbilityCardImmediately), 2.5f);
        }
        else if (giftType == GiftType.Standard)
        {
            Invoke(nameof(ShowGiftImmediately), 2.5f);
        }
        else if (giftType == GiftType.BlackSmith)
        {
            Invoke(nameof(ShowBlackSmithImmediately), 2.5f);
        }
    }
   

    private void ShowAbilityCardImmediately()
    {
        InGameUI.Instance.OpenAbilityCardPanel();
    }
    private void ShowGiftImmediately()
    {
        GiftResource gift = Instantiate(gifts[UnityEngine.Random.Range(0, gifts.Count)]).GetComponent<GiftResource>();
        if (gift != null)
        {
            GridManager.Instance.ClearLegacyObjects(1);
            gift.Initialize();
        }
    }

    private void ShowBlackSmithImmediately()
    {
        BlackSmithController bs = Instantiate(blackSmith, blackSmithPos.position , blackSmithPos.transform.rotation);
        bs.Initialize();

        if(onlyOpenOnBlacksmith != null)
        {
            if(onlyOpenOnBlacksmith.Count > 0)
            {
                onlyOpenOnBlacksmith.ForEach(g => g.SetActive(true));
            }
        }
    }

    public void CloseBlacksmith()
    {
        if (onlyOpenOnBlacksmith != null)
        {
            if (onlyOpenOnBlacksmith.Count > 0)
            {
                onlyOpenOnBlacksmith.ForEach(g => g.SetActive(false));
            }
        }
    }
}
public enum GiftType
{
    None,
    Standard,
    AbilityCard,
    BlackSmith
}