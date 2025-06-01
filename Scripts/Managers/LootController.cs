using UnityEngine;

public class LootController : MonoBehaviour
{    
    [SerializeField] int coinPerLevel;
    [SerializeField] int gemPerLevel;
    [SerializeField] float etherealStonePerLevel;
    [SerializeField] ItemSO GiveableItemOnMapFinish;

    public void Give()
    {
        EconomyManager.Instance.AddResource(ResourceType.Coin, GetCoin());
        EconomyManager.Instance.AddResource(ResourceType.Gem, GetGem());
        EconomyManager.Instance.AddResource(ResourceType.EtherealStone, GetEtherealStone());
        if (GetGiveableItem())
        {
            ItemManager.Instance.OwnItem(GiveableItemOnMapFinish.Id);
        }
    }

    public int GetCoin() => GameStateManager.Instance.CurrentLevel * coinPerLevel;
    public int GetGem() => GameStateManager.Instance.CurrentLevel * gemPerLevel;
    public int GetEtherealStone() => Mathf.FloorToInt(GameStateManager.Instance.CurrentLevel * etherealStonePerLevel);
    public ItemSO GetGiveableItem()
    {
        if(GameStateManager.Instance.CurrentLevel >= GameStateManager.Instance.LastLevelOfMap)
        {
            return GiveableItemOnMapFinish;
        }
        return null;
    }

}
