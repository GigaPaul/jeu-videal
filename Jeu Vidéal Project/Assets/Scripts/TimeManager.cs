using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private float ElapsedTime;
    private readonly DateTime StartDate = new(2023, 1, 1, 16, 0, 0);
    /// <summary>
    /// How many real-life seconds for 1 in-game day
    /// </summary>
    [SerializeField]
    private float SecondsPerDay = 86400;
    public DateTime CurrentDate { get; private set; }
    public Light Sun;





    // Update is called once per frame
    void Update()
    {
        ElapsedTime += Time.deltaTime;

        SetCurrentDate();
        DayNightCycle();
    }



    private void SetCurrentDate()
    {
        float elapsedDays = ElapsedTime / SecondsPerDay;
        TimeSpan elapsedTimeSpan = TimeSpan.FromDays(elapsedDays);

        CurrentDate = StartDate.Add(elapsedTimeSpan);
    }



    private void DayNightCycle()
    {
        float secondsOfDay = (float)CurrentDate.TimeOfDay.TotalSeconds;
        float maxSeconds = 86400;

        float dayProgression = Mathf.Floor((secondsOfDay / maxSeconds) * 100000) / 100000;
        Sun.transform.rotation = Quaternion.Euler(360 * dayProgression - 90, 0, 0);
    }
}
