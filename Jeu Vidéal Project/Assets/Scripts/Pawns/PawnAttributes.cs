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

        //float rand = Random.Range(0, 100);

        //if (rand > 50)
        //{
        //    Title title1 = new("Big", 0);
        //    Title title2 = new("«The Big»", 1);
        //    Title title3 = new("The Big", 2);
        //    _Title = title1;
        //}
    }

    public string GetFullName()
    {
        return _Culture.GetFullNameOf(this);
    }
}
