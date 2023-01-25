using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public string Label { get; set; }
    public float CastingTime { get; set; } = 3;
    public Pawn Actor { get; set; }
    public Vector3? Target { get; set; }
}
