using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SmartUnemployed : SmartPawn
{
    public override string Label
    {
        get { return "Unemployed"; }
    }

    protected override void Work()
    {
        return;
    }
}
