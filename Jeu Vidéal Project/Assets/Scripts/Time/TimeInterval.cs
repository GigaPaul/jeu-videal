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
        return Start <= timespan && timespan < End;
    }
}
