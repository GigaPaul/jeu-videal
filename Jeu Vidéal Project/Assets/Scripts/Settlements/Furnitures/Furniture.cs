using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Furniture : MonoBehaviour
{
#nullable enable
    [HideInInspector]
    public Pawn? User;
    #nullable disable

    public virtual string ActionLabel
    {
        get { return "Using furniture"; }
    }

    public abstract Action GetAction(Pawn pawn);





    public bool IsBeingUsed()
    {
        return User != null;
    }
}
