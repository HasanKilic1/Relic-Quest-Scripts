using System;
[Serializable]
public class ItemData 
{
    public int id; 
    public int level;
    public bool isEquipped;
    public ItemData()
    {}

    public ItemData(int id, int level, bool isEquipped)
    {
        this.id = id;
        this.level = level;
        this.isEquipped = isEquipped;
    }

    
}
