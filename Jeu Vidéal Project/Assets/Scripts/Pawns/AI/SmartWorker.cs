using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartWorker : SmartPawn
{
    public override string Label
    {
        get { return "Worker"; }
    }

    protected override void Routine()
    {
        //Debug.Log("test");
    }
}
