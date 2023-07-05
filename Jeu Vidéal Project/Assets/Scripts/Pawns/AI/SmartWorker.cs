using System;
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
        TimeSpan start = new(9, 0, 0);
        TimeSpan end = new(17,0,0);

        TimeInterval workInterval = new(start, end);
        WorkingHours.Add(workInterval);

        UnityEngine.Object PickaxeObject = Resources.Load("Prefabs/Tools/Pickaxe");
        GameObject Pickaxe = Instantiate(PickaxeObject) as GameObject;

        Master.Attachments.RightHand.Attach(Pickaxe.transform);
    }


    protected override void Work()
    {
        Transform randomWorkingStation = Master.Settlement.WorkingStations[UnityEngine.Random.Range(0, Master.Settlement.WorkingStations.Count)];



        //// Working
        //System.Action working = new()
        //{
        //    Label = "Working",
        //    Target = randomWorkingStation
        //};

        //working.StartingScript = () =>
        //{
        //    _Pawn.Movement.RotationTarget = working.Target;
        //    _Pawn.Animator.SetBool("IsWorking", true);
        //    return Task.FromResult(0);
        //};

        //working.SuccessCondition = () =>
        //{
        //    // Stop after the animation looped 3 times
        //    return _Pawn.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 3;
        //};

        //working.EndingScript = () =>
        //{
        //    if (_Pawn.Movement.RotationTarget == working.Target)
        //    {
        //        _Pawn.Movement.RotationTarget = null;
        //    }

        //    _Pawn.Animator.SetBool("IsWorking", false);
        //};






        //
        Action working = Action.Find("act_move");
        working.Target = randomWorkingStation;

        Master.Do(working);
        //


        // Return resources
        Action returnResources = Action.Find("act_move");
        returnResources.Target = Master.Settlement.Storage;




        Master.Do(working);
        Master.Do(returnResources, false, true);
    }
}
