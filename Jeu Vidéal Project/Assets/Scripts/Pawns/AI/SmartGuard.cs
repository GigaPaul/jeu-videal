using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartGuard : SmartPawn
{
    public override string Label
    {
        get { return "Guard"; }
    }

    protected override void Routine()
    {
        if(_Pawn.Flock.Commander != _Pawn)
        {
            return;
        }


        _Pawn.ActionManager.IsLoop = true;

        foreach (Transform waypoint in _Pawn.Settlement.Patrol)
        {
            _Pawn.GoTo(waypoint.position, true);
        }
    }
}
