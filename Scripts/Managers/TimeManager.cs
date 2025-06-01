using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance {  get; private set; }

    private void Awake()
    {
        instance = this;
    }
    public void FreezeTime()
    {
        Time.timeScale = 0;
    }

    public void ContinueGame()
    {
        Time.timeScale = 1;
    }

    public void ChangeTimeSpeedForAWhile(float speed , float time)
    {
        Time.timeScale = speed;
        Invoke(nameof(ContinueGame) , time);
    }
}
