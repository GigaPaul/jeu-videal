using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
public class PawnAttachments : MonoBehaviour
{
    public Pawn _Pawn { get; set; }
    public Attachment RightHand;
    public Attachment LeftHand;


    private void Start()
    {
        _Pawn = GetComponent<Pawn>();
    }
}
