using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    public string Id;
    public string Label;
    public string TriggerLabel;
    public int Damages;
    public float MaxRange;
    public float MinRange;
    public bool HasBeenTriggered = false;
    public Pawn Target { get; set; }


    public Ability(string id, string triggerLabel, int damages = 10, float maxRange = 2, float minRange = 1)
    {
        Id = id;
        TriggerLabel = triggerLabel;
        Damages = damages;
        MaxRange = maxRange;

        if(maxRange >= minRange)
        {
            MinRange = minRange;
        }
        else
        {
            MinRange = maxRange / 2;
        }
    }
}
