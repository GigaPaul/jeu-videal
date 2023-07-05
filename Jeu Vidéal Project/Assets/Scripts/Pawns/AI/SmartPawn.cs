using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
public abstract class SmartPawn : MonoBehaviour
{
    [HideInInspector]
    public Pawn Master;
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
        Master = GetComponent<Pawn>();
        _TimeManager = FindObjectOfType<TimeManager>();
        InvokeRepeating(nameof(RoutineController), 0, 1);
    }





    private void RoutineController()
    {
        ///////////////////////////////////////////////////
        // All the reasons a Pawn would not use their AI //
        ///////////////////////////////////////////////////

        // The pawn is dead
        if (!Master.IsAlive)
        {
            return;
        }

        // The pawn is in combat
        if(Master.IsInCombat())
        {
            return;
        }

        // The pawn is moving
        if (Master.IsMoving())
        {
            return;
        }

        // The pawn has tasks to do
        if (!Master._ActionManager.QueueIsEmpty())
        {
            return;
        }

        // The spawn is playable
        if (Master.IsPlayable())
        {
            return;
        }

        ////////////////////////////////////////////////

        // Start AI routine
        Routine();
    }





    //protected abstract void Routine();
    public void Routine()
    {
        if (MustWork())
        {
            Work();
        }
        else if (MustSleep())
        {
            Sleep();
        }
        else
        {
            FreeTime();
        }
    }





    protected abstract void Work();
    //protected abstract void FreeTime();
    //protected abstract void Sleep();





    protected virtual void FreeTime()
    {
        if (Master.Settlement == null)
        {
            return;
        }

        Vector3 wanderPoint = Master.Settlement.GetRandomPoint();

        Action wander = Action.Find("act_wander");
        wander.Destination = wanderPoint;

        Master.Do(wander);
    }





    protected virtual void Sleep()
    {
        if(Master.Settlement == null)
        {
            return;
        }

        List<Bed> beds = Master.Settlement.GetBeds();
        beds = beds.Where(i => !i.IsBeingUsed()).ToList();

        if (beds.Count == 0)
        {
            return;
        }

        int random = UnityEngine.Random.Range(0, beds.Count);
        Bed bed = beds[random];
        //_Pawn.Do(bed.GetAction(_Pawn));



        //
        Action sleep = Action.Find("act_move");
        sleep.Destination = bed.transform.position;

        Master.Do(sleep);
        //
    }





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
