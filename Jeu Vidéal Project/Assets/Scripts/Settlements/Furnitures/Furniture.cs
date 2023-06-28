using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Furniture : MonoBehaviour
{
    #nullable enable
    [HideInInspector]
    public Pawn? User;
#nullable disable
    public Action UsageAction;





    public bool IsBeingUsed()
    {
        return User != null;
    }
}
