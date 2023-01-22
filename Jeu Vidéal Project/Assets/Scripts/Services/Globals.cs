using System.Collections.Generic;
using UnityEngine;

public class Globals
{
    public static Neighbour FocusedNeighbour { get; set; }
    public static Neighbour HoveredNeighbour { get; set; }
    public static Building BuildingBlueprint { get; set; }
    public static int FocusableMask { get; set; } = LayerMask.GetMask("Focusable");
    public static int InteractiveMask { get; set; } = LayerMask.GetMask("Interactive");
    public static int GroundMask { get; set; } = LayerMask.GetMask("Ground");
    public static GameObject PauseMenu { get; set; }
}
