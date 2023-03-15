using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(Pawn))]
public class PawnAttributes : MonoBehaviour
{
    public Pawn _Pawn;
    public Culture _Culture;
    public string FirstName;
    public string LastName;

    #nullable enable
    public Title? _Title;
    #nullable disable





    // Start is called before the first frame update
    void Start()
    {
        InitPersonality();
    }





    private void InitPersonality()
    {
        if(_Culture == null)
        {
            if(_Pawn.Settlement != null)
            {
                _Culture = _Pawn.Settlement._Culture;
            }
            //else if(_Pawn.Faction != null)
            //{

            //}
        }

        if(_Culture == null)
        {
            return;
        }

        if(FirstName == "" && _Culture.FirstNames.Any())
        {
            FirstName = _Culture.FirstNames[Random.Range(0, _Culture.FirstNames.Count)];
        }

        if (LastName == "" && _Culture.LastNames.Any())
        {
            LastName = _Culture.LastNames[Random.Range(0, _Culture.LastNames.Count)];
        }
    }

    public string GetFullName()
    {
        if(_Culture != null)
        {
            return _Culture.GetFullNameOf(this);
        }

        if(FirstName != "" || LastName != "")
        {
            string result = "";

            if(FirstName != null)
            {
                result += FirstName;

                if(LastName != null)
                {
                    result += " ";
                }
            }

            if(LastName != null)
            {
                result += LastName;
            }

            return result;
        }

        return "Wanderer";
    }
}
