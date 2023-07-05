using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
public class PawnAttachments : MonoBehaviour
{
    public Pawn Master { get; set; }
    public Attachment RightHand;
    public Attachment LeftHand;


    private void Start()
    {
        Master = GetComponent<Pawn>();
    }
}
