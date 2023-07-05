using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SmartInnkeeper : SmartPawn
{
    public override string Label
    {
        get { return "Innkeeper"; }
    }



    protected override void Start()
    {
        base.Start();

        TimeSpan start = new(9, 0, 0);
        TimeSpan end = new(17, 0, 0);

        TimeInterval workInterval = new(start, end);
        WorkingHours.Add(workInterval);
    }

    protected override void Work()
    {
        //Action bartending = new()
        //{
        //    Label = "Bartending",
        //    Target = _Pawn.Settlement.Inn,
        //    StartingScript = () =>
        //    {
        //        _Pawn.Animator.SetBool("IsBartending", true);
        //        _Pawn.Movement.RotationTarget = _Pawn.Settlement.Inn;
        //        _Pawn.Movement.RotationTargetOffset = _Pawn.Settlement.Inn.forward;
        //        return Task.FromResult(0);
        //    },

        //    SuccessCondition = () =>
        //    {
        //        return !MustWork();
        //    },

        //    EndingScript = () =>
        //    {
        //        _Pawn.Animator.SetBool("IsBartending", false);
        //        _Pawn.Movement.RotationTarget = null;
        //        _Pawn.Movement.RotationTargetOffset = Vector3.zero;
        //    }
        //};

        //_Pawn.Do(bartending);


        //
        Action bartending = Action.Find("act_move");
        bartending.Destination = Master.Settlement.Inn.position;

        Master.Do(bartending);
        //
    }
}
