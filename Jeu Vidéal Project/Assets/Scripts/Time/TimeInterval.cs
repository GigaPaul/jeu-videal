using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeInterval
{
    TimeSpan Start;
    TimeSpan End;

    public TimeInterval(TimeSpan start, TimeSpan end)
    {
        Start = start;
        End = end;
    }

    public bool Contains(TimeSpan timespan)
    {
        if(Start <= End)
        {
            return Start <= timespan && timespan < End;
        }

        // If the interval is crossing midnight (Ex: Start = 22:00 and End = 05:00)

        TimeSpan earlyMidnight = new(0, 0, 0);
        TimeSpan lateMidnight = new(24, 0, 0);

        bool isLate = Start <= timespan && timespan <= lateMidnight;
        bool isEarly = earlyMidnight <= timespan && timespan < End;

        return isLate || isEarly;
    }
}
