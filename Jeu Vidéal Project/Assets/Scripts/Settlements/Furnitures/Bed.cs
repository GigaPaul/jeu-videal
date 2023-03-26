using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Bed : Furniture
{
    public override string ActionLabel
    {
        get { return "Sleeping"; }
    }

    public override Action GetAction(Pawn pawn)
    {
        Action use = new()
        {
            Label = ActionLabel,
            Target = transform,
            SuccessCondition = () => { return false; }
        };


        use.StartingScript = () =>
        {
            pawn.Animator.SetBool("IsSitting", true);
            pawn.Movement.RotationTarget = use.Target;
            pawn.Movement.RotationTargetOffset = pawn.Movement.RotationTarget.forward;
            User = pawn;
            return Task.FromResult(0);
        };


        use.EndingScript = () =>
        {
            if (pawn.Movement.RotationTarget == use.Target)
            {
                pawn.Movement.RotationTarget = null;
                pawn.Movement.RotationTargetOffset = Vector3.zero;
            }
            pawn.Animator.SetBool("IsSitting", false);
        };

        return use;
    }
}
