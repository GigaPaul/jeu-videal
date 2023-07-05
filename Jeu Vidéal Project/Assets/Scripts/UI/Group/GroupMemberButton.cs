using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GroupMemberButton : MonoBehaviour
{
    public Pawn Master { get; set; }





    private void FixedUpdate()
    {
        if (Master == null)
        {
            return;
        }

        GetComponentInChildren<TextMeshProUGUI>().text = Master.Attributes.GetFullName();
    }





    public void FocusPawn()
    {
        if(Master == null)
        {
            return;
        }

        Master.Focus();
    }
}
