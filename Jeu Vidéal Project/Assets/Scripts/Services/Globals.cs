using System.Collections.Generic;
using UnityEngine;

public class Globals
{
    public static Pawn FocusedPawn { get; set; }
    public static Pawn HoveredPawn { get; set; }
    public static int FocusableMask { get; set; } = LayerMask.GetMask("Focusable");
    public static int InteractiveMask { get; set; } = LayerMask.GetMask("Interactive");
    public static int GroundMask { get; set; } = LayerMask.GetMask("Ground");
    public static GameObject PauseMenu { get; set; }
    public static List<Faction> Factions { get; set; } = new();
    public static List<Settlement> Settlements { get; set; } = new();
}
