using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilityButton : MonoBehaviour
{
    public string ShortCut { get; set; }
    public Ability _Ability { get; set; }


    public void Load(Ability ability)
    {
        _Ability = ability;
        GetComponentInChildren<TextMeshProUGUI>().text = ability.Id;
    }


    public void Unload()
    {
        _Ability = null;
        GetComponentInChildren<TextMeshProUGUI>().text = "Empty";
    }


    public void Cast()
    {
        if(_Ability == null)
        {
            return;
        }

        if(Globals.FocusedPawn == null)
        {
            return;
        }

        Debug.Log(_Ability.Id);

        Globals.FocusedPawn.Cast(_Ability);
    }
}
