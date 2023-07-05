using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnModel : MonoBehaviour
{
    public List<Renderer> Renderers = new();
    Pawn Master;

    private void Awake()
    {
        Master = GetComponentInParent<Pawn>();
    }
}
