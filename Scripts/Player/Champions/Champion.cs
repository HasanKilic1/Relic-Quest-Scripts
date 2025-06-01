using System;

[Serializable]
public class Champion 
{
    public int ID;
    public int Level;
    public bool isSelected;
    public Champion(int ID , int Level)
    {
        this.ID = ID;
        this.Level = Level; 
    }
    public void UpgradeLevel()
    {
        if (IsUpgradable)
        {
            Level++;
        }
    }
    private bool IsUpgradable => Level < 5;
}