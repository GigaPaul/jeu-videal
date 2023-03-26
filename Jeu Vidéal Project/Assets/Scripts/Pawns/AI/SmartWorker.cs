using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SmartWorker : SmartPawn
{
    public override string Label
    {
        get { return "Worker"; }
    }

    protected override void Start()
    {
        base.Start();

        Object PickaxeObject = Resources.Load("Prefabs/Tools/Pickaxe");
        GameObject Pickaxe = Instantiate(PickaxeObject) as GameObject;

        _Pawn.Attachments.RightHand.Attach(Pickaxe.transform);
    }

    protected override void Routine()
    {
        if(!FindObjectOfType<TimeManager>())
        {
            return;
        }

        TimeManager timeManager = FindObjectOfType<TimeManager>();
        double hour = timeManager.CurrentDate.TimeOfDay.Hours;

        if(9 <= hour && hour < 17)
        {
            WorkRoutine();
        }
        else
        {
            OffDutyRoutine();
        }
    }




    private void OffDutyRoutine()
    {
        List<Bed> beds = _Pawn.Settlement.GetBeds();
        beds = beds.Where(i => !i.IsBeingUsed()).ToList();

        if(beds.Count == 0)
        {
            return;
        }

        int random = Random.Range(0, beds.Count);
        Bed bed = beds[random];
        _Pawn.Do(bed.GetAction(_Pawn));


        //Vector3 wanderPoint = _Pawn.Settlement.GetRandomPoint();

        //float waitingTime = 3;

        //Action wander = new()
        //{
        //    Label = "Wandering",
        //    Destination = wanderPoint
        //};

        //wander.StartingScript = async () =>
        //{
        //    await Task.Delay((int)(waitingTime * 1000), wander.TokenSource.Token);
        //};

        //_Pawn.Do(wander);
    }


    private void WorkRoutine()
    {
        Transform randomWorkingStation = _Pawn.Settlement.WorkingStations[Random.Range(0, _Pawn.Settlement.WorkingStations.Count)];
        //_Pawn._ActionManager.IsLoop = true;



        // Working
        Action working = new()
        {
            Label = "Working",
            Target = randomWorkingStation
        };

        working.StartingScript = () =>
        {
            _Pawn.Movement.RotationTarget = working.Target;
            _Pawn.Animator.SetBool("IsWorking", true);
            return Task.FromResult(0);
        };

        working.SuccessCondition = () =>
        {
            // Stop after the animation looped 3 times
            return _Pawn.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 3;
        };

        working.EndingScript = () =>
        {
            if (_Pawn.Movement.RotationTarget == working.Target)
            {
                _Pawn.Movement.RotationTarget = null;
            }

            _Pawn.Animator.SetBool("IsWorking", false);
        };





        // Return resources
        Action returnResources = new()
        {
            Label = "Returning resources",
            Target = _Pawn.Settlement.Storage,
            StartingScript = () =>
            {
                _Pawn.Settlement.ResourceStock += 10;
                return Task.FromResult(0);
            }
        };



        _Pawn.Do(working);
        _Pawn.Do(returnResources, false, true);
    }
}
