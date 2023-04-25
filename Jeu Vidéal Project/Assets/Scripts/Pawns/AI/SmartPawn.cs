using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
public abstract class SmartPawn : MonoBehaviour
{
    [HideInInspector]
    public Pawn _Pawn;
    public List<TimeInterval> WorkingHours = new();
    public TimeManager _TimeManager;
    public bool IsDiurnal = true;

    public virtual string Label
    {
        get { return "Wanderer"; }
    }





    // Start is called before the first frame update
    protected virtual void Start()
    {
        _Pawn = GetComponent<Pawn>();
        _TimeManager = FindObjectOfType<TimeManager>();
        InvokeRepeating(nameof(RoutineController), 0, 1);
    }





    private void RoutineController()
    {
        // If the pawn is dead
        if (!_Pawn.IsAlive)
        {
            return;
        }

        // If the pawn is moving
        if (_Pawn.IsMoving())
        {
            return;
        }

        // If the pawn has tasks to do
        if (!_Pawn._ActionManager.QueueIsEmpty())
        {
            return;
        }

        // If the spawn is playable
        if (_Pawn.IsPlayable())
        {
            return;
        }

        // Start AI routine
        Routine();
    }





    protected abstract void Routine();

    public bool MustWork()
    {
        if(!WorkingHours.Any())
        {
            return false;
        }


        TimeSpan timeOfDay = _TimeManager.CurrentDate.TimeOfDay;

        foreach (TimeInterval interval in WorkingHours)
        {
            if(interval.Contains(timeOfDay))
            {
                return true;
            }
        }

        return false;
    }


    public bool MustSleep()
    {
        TimeSpan timeOfDay = _TimeManager.CurrentDate.TimeOfDay;

        if (IsDiurnal)
        {
            TimeSpan start = new(22, 0, 0);
            TimeSpan end = new(6, 0, 0);
            TimeInterval interval = new(start, end);

            return interval.Contains(timeOfDay);
        }
        else
        {
            TimeSpan start = new(12, 0, 0);
            TimeSpan end = new(18, 0, 0);
            TimeInterval interval = new(start, end);

            return interval.Contains(timeOfDay);
        }
    }
}
