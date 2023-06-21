using System;
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

        TimeSpan start = new(9, 0, 0);
        TimeSpan end = new(17, 0, 0);

        TimeInterval workInterval = new(start, end);
        WorkingHours.Add(workInterval);

        UnityEngine.Object SwordObject = Resources.Load("Prefabs/Tools/Sword");
        GameObject Sword = Instantiate(SwordObject) as GameObject;

        _Pawn.Attachments.RightHand.Attach(Sword.transform);
    }





    protected override void Work()
    {
        if (_Pawn.Flock.Commander != _Pawn)
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
