using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartGuard : SmartPawn
{
    public override string Label
    {
        get { return "Guard"; }
    }

    protected override void Start()
    {
        base.Start();

        Object SwordObject = Resources.Load("Prefabs/Tools/Sword");
        GameObject Sword = Instantiate(SwordObject) as GameObject;

        _Pawn.Attachments.RightHand.Attach(Sword.transform);
    }

    protected override void Routine()
    {
        if(_Pawn.Flock.Commander != _Pawn)
        {
            return;
        }


        _Pawn._ActionManager.IsLoop = true;

        foreach (Transform waypoint in _Pawn.Settlement.Patrol)
        {
            _Pawn.GoTo(waypoint.position, true);
        }
    }
}
