using UnityEngine;

[System.Serializable]
public class RelicData
{
    public int Id;
    public int Level;
    public string Name;
    public Rarity rarity;
    public int totalSelected;
    public RelicData(int ýd, int level, string name, Rarity rarity)
    {
        Id = ýd;
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
