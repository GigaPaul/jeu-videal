using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AbilityBar : MonoBehaviour
{
    public List<AbilityButton> Abilities { get; set; }


    private void Awake()
    {
        InitAbilityButtons();
    }


    private void InitAbilityButtons()
    {
        Abilities = GetComponentsInChildren<AbilityButton>().ToList();
        
        for(int i = 0; i < Abilities.Count; i++)
        {
            AbilityButton button = Abilities[i];
            button.ShortCut = (i + 1).ToString();
        }
    }

    public void LoadAbilitiesOf(Pawn pawn)
    {
        //Debug.Log(Abilities.Count);
        for (int i = 0; i < Abilities.Count; i++)
        {
            AbilityButton button = Abilities[i];

            if(i < pawn._PawnCombat.Abilities.Count)
            {
                //Debug.Log("Exists : "+i);
                button.Load(pawn._PawnCombat.Abilities[i]);
            }
            else
            {
                //Debug.Log("Doesn't exist : " + i);
                button.Unload();
            }
        }
    }



    public void UnloadAbilities()
    {
        foreach(AbilityButton button in Abilities)
        {
            button.Unload();
        }
    }
}
