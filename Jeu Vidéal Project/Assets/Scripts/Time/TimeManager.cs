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
    public bool IsPaused = false;





    // Update is called once per frame
    void Update()
    {
        if(IsPaused)
        {
            return;
        }

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
        Vector3 euler = Sun.transform.rotation.eulerAngles;
        //Sun.transform.rotation = Quaternion.Euler(360 * dayProgression - 90, euler.y, euler.z);
        //Sun.transform.rotation = Quaternion.Euler(360 * dayProgression - 90, 0, 0);
        Sun.transform.rotation = Quaternion.AngleAxis(360 * dayProgression - 90, Vector3.right);
    }
}
