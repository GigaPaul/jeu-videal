using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SmartWorker : SmartPawn
{
    public override string Label
    {
        get { return "Worker"; }
    }

    protected override void Routine()
    {
        Transform randomWorkingStation = _Pawn.Settlement.WorkingStations[UnityEngine.Random.Range(0, _Pawn.Settlement.WorkingStations.Count)];
        float waitingTime = 4.833f;
        _Pawn.ActionManager.IsLoop = true;

        Action working = new()
        {
            Label = "Working",
            Target = randomWorkingStation
        };

        working.StartingScript = async () =>
        {
            _Pawn.Animator.SetBool("IsWorking", true);
            await Task.Delay((int)(waitingTime * 1000), working.TokenSource.Token);
            _Pawn.Animator.SetBool("IsWorking", false);
        };

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
