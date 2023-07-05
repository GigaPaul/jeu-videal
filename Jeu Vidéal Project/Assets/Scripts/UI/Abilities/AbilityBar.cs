using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AbilityBar : MonoBehaviour
{
    public List<AbilityButton> Abilities { get; set; } = new();


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
        if(Globals.FocusedPawn == null)
        {
            return;
        }

        List<AbilityHolder> abilityHolders = Globals.FocusedPawn._PawnCombat.AbilityHolders;

        if (abilityHolders.Count == 0)
        {
            return;
        }


        foreach (AbilityButton button in Abilities)
        {
            button.UpdateCooldown();
            //Ability ability = button._Ability;
            //AbilityHolder holder = abilityHolders.FirstOrDefault(i => i.AbilityHeld == ability);

            //if (holder == null)
            //{
            //    continue;
            //}

            //float coolDownProgression = (float)Math.Floor(holder.CoolDown / ability.CoolDownTime * 100) / 100;
            //float maxHeight = button.GetComponent<RectTransform>().rect.height;

            //float width = button.CoolDownRect.rect.width;
            //float height = maxHeight * coolDownProgression;

            //button.CoolDownRect.sizeDelta = new(width, height);
        }
    }




    private void InitAbilityButtons()
    {
        List<AbilityButton> buttonList = GetComponentsInChildren<AbilityButton>().ToList();
        
        for(int i = 0; i < buttonList.Count; i++)
        {
            AbilityButton button = buttonList[i];
            button.ShortCut = (i + 1).ToString();
            Abilities.Add(button);
        }
    }

    public void LoadAbilitiesOf(Pawn pawn)
    {
        //Debug.Log(Abilities.Count);
        for (int i = 0; i < Abilities.Count; i++)
        {
            AbilityButton button = Abilities[i];

            if(i < pawn._PawnCombat.AbilityHolders.Count)
            {
                //Debug.Log("Exists : "+i);
                button.Load(pawn._PawnCombat.AbilityHolders[i]);
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
