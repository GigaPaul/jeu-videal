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

        _Pawn.Attachments.RightHand.Attach(Pickaxe.transform);
    }


    protected override void Work()
    {
        Transform randomWorkingStation = _Pawn.Settlement.WorkingStations[UnityEngine.Random.Range(0, _Pawn.Settlement.WorkingStations.Count)];



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
