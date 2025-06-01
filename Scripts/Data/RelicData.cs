using UnityEngine;

[System.Serializable]
public class RelicData
{
    public int Id;
    public int Level;
    public string Name;
    public Rarity rarity;
    public int totalSelected;
    public RelicData(int �d, int level, string name, Rarity rarity)
    {
        Id = �d;
        Level = level;
        Name = name;
        this.rarity = rarity;
        totalSelected = 0;
    }


    public void Upgrade()
    {
        if(rarity < Rarity.Legendary)
        {
            rarity++;
            Level++;
        }

        Debug.Log("Card Upgraded : " + Name);
    }
}
