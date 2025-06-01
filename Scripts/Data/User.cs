using System;

[Serializable]
public class User
{
    public static event Action OnExperienceAdded;
    public string Name;
    public int Level;
    public int Experience {  get; set; }

    public User(string name, int level, int experience)
    {
        Name = name;
        Level = level;
        Experience = experience;
    }

    public void AddExperience(int experience)
    {
        Experience += experience;
        OnExperienceAdded?.Invoke();
    }
}
