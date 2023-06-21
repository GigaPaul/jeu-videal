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

    //protected override void Routine()
    //{
    //    Vector3 wanderPoint = _Pawn.Settlement.GetRandomPoint();

    //    float waitingTime = 3;

    //    Action wander = new()
    //    {
    //        Label = "Wandering",
    //        Destination = wanderPoint
    //    };

    //    wander.StartingScript = async () =>
    //    {
    //        await Task.Delay((int)(waitingTime * 1000), wander.TokenSource.Token);
    //    };

    //    _Pawn.Do(wander);
    //}
}
