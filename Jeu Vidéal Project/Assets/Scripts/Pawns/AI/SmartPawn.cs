using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        if (_Pawn.Settlement == null)
        {
            return;
        }

        Vector3 wanderPoint = _Pawn.Settlement.GetRandomPoint();

        float waitingTime = 3;

        Action wander = new()
        {
            Label = "Wandering",
            Destination = wanderPoint
        };

        wander.StartingScript = async () =>
        {
            await Task.Delay((int)(waitingTime * 1000), wander.TokenSource.Token);
        };

        _Pawn.Do(wander);
    }





    protected virtual void Sleep()
    {
        if(_Pawn.Settlement == null)
        {
            return;
        }

        List<Bed> beds = _Pawn.Settlement.GetBeds();
        beds = beds.Where(i => !i.IsBeingUsed()).ToList();

        if (beds.Count == 0)
        {
            return;
        }

        int random = UnityEngine.Random.Range(0, beds.Count);
        Bed bed = beds[random];
        _Pawn.Do(bed.GetAction(_Pawn));
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
