using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupUIManager : MonoBehaviour
{
    List<Pawn> ControlledPawns = new();
    // Start is called before the first frame update
    void Start()
    {
        CheckControlledPawns();
    }

    public void CheckControlledPawns()
    {
        Faction playerFaction = FindObjectsOfType<Faction>().FirstOrDefault(i => i.Id == "g_player");

        List<Pawn> playablePawns = FindObjectsOfType<Pawn>().Where(i => i.Faction == playerFaction).ToList();

        foreach(Pawn pawn in playablePawns)
        {
            AddControlledPawn(pawn);
        }
    }

    public void AddControlledPawn(Pawn pawn)
    {
        if(ControlledPawns.Contains(pawn))
        {
            return;
        }

        ControlledPawns.Add(pawn);
    }

    public void RemoveControlledPawn(Pawn pawn)
    {
        ControlledPawns.Remove(pawn);
    }
}
