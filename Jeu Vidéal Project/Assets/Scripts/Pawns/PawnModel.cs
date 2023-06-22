using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnModel : MonoBehaviour
{
    public List<Renderer> Renderers = new();
    Pawn _Pawn;

    private void Awake()
    {
        _Pawn = GetComponentInParent<Pawn>();
    }
}
