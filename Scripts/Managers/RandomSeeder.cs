using System.Collections.Generic;
using UnityEngine;

public class RandomSeeder : MonoBehaviour
{
    public static int GetSeed()
    {
        if (PlayerPrefs.HasKey("RandomSeed"))
        {
            int seed = PlayerPrefs.GetInt("RandomSeed");
            seed++;
            if (seed > 100000) seed = 42;
            PlayerPrefs.SetInt("RandomSeed" , seed);           
        }
        else
        {
            PlayerPrefs.SetInt("RandomSeed", 42);
        }
        return PlayerPrefs.GetInt("RandomSeed");
    }
}
