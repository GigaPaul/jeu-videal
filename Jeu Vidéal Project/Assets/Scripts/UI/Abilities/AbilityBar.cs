using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AbilityBar : MonoBehaviour
{
    public List<AbilityButton> Slots { get; set; } = new();


    private void Awake()
    {
        InitAbilityButtons();
    }


    private void Update()
    {
        UpdateCoolDowns();
    }



    private void UpdateCoolDowns()
    {
        if(Globals.FocusedInteractive == null)
        {
            return;
        }

        //List<AbilityHolder> abilityHolders = Globals.FocusedPawn._PawnCombat.AbilityHolders;

        //if (abilityHolders.Count == 0)
        //{
        //    return;
        //}

        foreach (AbilityButton button in Slots)
        {
            button.UpdateCooldown();
        }
    }




    private void InitAbilityButtons()
    {
        List<AbilityButton> buttonList = GetComponentsInChildren<AbilityButton>().ToList();
        
        for(int i = 0; i < buttonList.Count; i++)
        {
            AbilityButton button = buttonList[i];
            button.ShortCut = (i + 1).ToString();
            Slots.Add(button);
        }
    }

    public void LoadAbilitiesOf(Pawn pawn)
    {
        //Debug.Log(Abilities.Count);
        for (int i = 0; i < Slots.Count; i++)
        {
            AbilityButton button = Slots[i];

            if(i < pawn._PawnCombat.AbilityHolders.Count)
            {
                button.Load(pawn._PawnCombat.AbilityHolders[i]);
            }
            else
            {
                button.Unload();
            }
        }
    }



    public void UnloadAbilities()
    {
        foreach(AbilityButton button in Slots)
        {
            button.Unload();
        }
    }
}
