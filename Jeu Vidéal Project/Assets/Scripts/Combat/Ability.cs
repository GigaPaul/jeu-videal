using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    public string Id;
    public string Label;
    public string TriggerLabel;
    public int Damages;
    public int Range;
    public bool HasBeenTriggered = false;
    public Pawn Target { get; set; }


    public Ability(string triggerLabel, int damages = 10, int range = 1)
    {
        TriggerLabel = triggerLabel;
        Damages = damages;
        Range = range;
    }
}
