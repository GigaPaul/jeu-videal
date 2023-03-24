using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SmartTrader : SmartPawn
{
    public override string Label
    {
        get { return "Trader"; }
    }

    private List<Settlement> VisitedSettlements = new();
    private int EncounteredTraders = 0;





    protected override void Routine()
    {
        if(_Pawn.Faction.Label == "Wanderers")
        {
            WanderingTraderRoutine();
            return;
        }

        LocalTraderRoutine();
    }





    private void WanderingTraderRoutine()
    {
        if (VisitedSettlements.Count == Globals.Settlements.Count)
        {
            Settlement lastVisitedSettlement = VisitedSettlements.Last();
            VisitedSettlements.Clear();
            VisitedSettlements.Add(lastVisitedSettlement);
        }


        List<Settlement> unvisitedSettlements = Globals.Settlements.Where(i => !VisitedSettlements.Contains(i)).ToList();
        #nullable enable
        float nextWaypointDist = Mathf.Infinity;
        Settlement? nearestSettlement = null;
        #nullable disable

        foreach (Settlement settlement in unvisitedSettlements)
        {
            float thisDist = Vector3.Distance(settlement.transform.position, transform.position);

            if (nearestSettlement == null || thisDist < nextWaypointDist)
            {
                nextWaypointDist = thisDist;
                nearestSettlement = settlement;
            }
        }

        VisitedSettlements.Add(nearestSettlement);


        List<Pawn> LocalTraders = new();

        Vector3 waypoint = nearestSettlement.GetRandomPoint(0.5f);

        Action traderAction = new()
        {
            Label = "Trading",
            Destination = waypoint,
            StartingScript = () =>
            {
                EncounteredTraders = 0;
                _Pawn.Animator.SetTrigger("Wave");
                _Pawn.Say($"Hello {nearestSettlement.Label}!");

                LocalTraders = FindObjectsOfType<Pawn>().Where(i => i.Settlement == nearestSettlement && i.GetComponent<SmartTrader>()).ToList();

                foreach (Pawn local in LocalTraders)
                {
                    Action buyFromTrader = new()
                    {
                        Label = "Buying",
                        Target = transform,
                        StartingScript = () =>
                        {
                            local.Say("Hello trader!");
                            EncounteredTraders++;
                            return Task.FromResult(0);
                        }
                    };

                    local.Do(buyFromTrader);
                }

                return Task.FromResult(0);
            },

            SuccessCondition = () =>
            {
                return EncounteredTraders == LocalTraders.Count;
            },

            SuccessScript = () =>
            {
                _Pawn.Say("I've seen everyone in this village, bye!");

                return Task.FromResult(0);
            }
        };

        _Pawn.Do(traderAction);
    }





    private void LocalTraderRoutine()
    {
        Vector3 wanderPoint = _Pawn.Settlement.GetRandomPoint();

        float waitingTime = 3;

        Action wander = new()
        {
            Label = "Wandering",
            Destination = wanderPoint
        };

        wander.StartingScript = async () =>
        {
            await Task.Delay((int)(waitingTime * 1000), wander.TokenSource.Token);
        };

        _Pawn.Do(wander);
    }
}
