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

    protected override void Routine()
    {
        Action bartending = new()
        {
            Label = "Bartending",
            Target = _Pawn.Settlement.Inn,
            StartingScript = () =>
            {
                _Pawn.Animator.SetBool("IsBartending", true);
                _Pawn._PawnMovement.RotationTarget = _Pawn.Settlement.Inn;
                _Pawn._PawnMovement.RotationTargetOffset = _Pawn.Settlement.Inn.forward;
                return Task.FromResult(0);
            },
            SuccessCondition = () =>
            {
                return false;
            }
        };

        _Pawn.Do(bartending);
    }
}
