using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GroupMemberButton : MonoBehaviour
{
    public Pawn _Pawn { get; set; }


    private void FixedUpdate()
    {
        if(_Pawn == null)
        {
            return;
        }

        GetComponentInChildren<TextMeshProUGUI>().text = _Pawn.Attributes.GetFullName();
    }

    public void FocusPawn()
    {
        if(_Pawn == null)
        {
            return;
        }

        Globals.FocusedPawn = _Pawn;
    }
}
