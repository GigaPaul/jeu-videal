using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostileCatcher : MonoBehaviour
{
    Pawn Master { get; set; }

    private void Awake()
    {
        Master = GetComponentInParent<Pawn>();
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    // We only check for player's aggro for now
    //    if(_Pawn.Faction.Id != "g_player")
    //    {
    //        return;
    //    }

    //    // Can only aggro pawns
    //    if (collision.gameObject.GetComponent<Pawn>() == null)
    //    {
    //        return;
    //    }

    //    Pawn pawnInRange = collision.gameObject.GetComponent<Pawn>();

    //    // Bandits are the only hostile factions for now
    //    if(pawnInRange.Faction.Id != "g_bandits")
    //    {
    //        return;
    //    }

    //    // Check if the hostile in range wasn't already caught
    //    if(_Pawn._PawnCombat.HostilesInRange.Contains(pawnInRange))
    //    {
    //        return;
    //    }

    //    // Catch the hostile
    //    _Pawn._PawnCombat.HostilesInRange.Add(pawnInRange);

    //    Debug.Log("Hostile caught : " + pawnInRange.Attributes.GetFullName());
    //}





    //private void OnCollisionExit(Collision collision)
    //{
    //    // We only check for player's aggro for now
    //    if (_Pawn.Faction.Id != "g_player")
    //    {
    //        return;
    //    }

    //    // Can only aggro pawns
    //    if (collision.gameObject.GetComponent<Pawn>() == null)
    //    {
    //        return;
    //    }

    //    Pawn pawnOutsideRange = collision.gameObject.GetComponent<Pawn>();

    //    // Bandits are the only hostile factions for now
    //    if (pawnOutsideRange.Faction.Id != "g_bandits")
    //    {
    //        return;
    //    }

    //    // Check if the hostile wasn't already released
    //    if (!_Pawn._PawnCombat.HostilesInRange.Contains(pawnOutsideRange))
    //    {
    //        return;
    //    }

    //    // Release the hostile
    //    _Pawn._PawnCombat.HostilesInRange.Remove(pawnOutsideRange);

    //    Debug.Log("Hostile released : " + pawnOutsideRange.Attributes.GetFullName());
    //}
}
