using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public List<Ability> Abilities = new();

    private void Awake()
    {
        LoadAbilities();
    }



    private void LoadAbilities()
    {
        LoadAutoAttack();
        LoadSlash();
        LoadBlock();
        LoadStab();
    }










    /////////////////
    // AUTO ATTACK //
    /////////////////
    private void LoadAutoAttack()
    {
        Ability ability = new("a_auto_attack", "Punch")
        {
            MinRange = 1,
            MaxRange = 2
        };

        Abilities.Add(ability);
    }





    ///////////
    // SLASH //
    ///////////
    private void LoadSlash()
    {
        Ability ability = new("a_slash", "Punch")
        {
            MinRange = 1,
            MaxRange = 2
        };

        Abilities.Add(ability);
    }





    ///////////
    // BLOCK //
    ///////////
    private void LoadBlock()
    {
        Ability ability = new("a_block", "Punch")
        {
            MinRange = 1,
            MaxRange = 2
        };

        Abilities.Add(ability);
    }





    //////////
    // STAB //
    //////////
    private void LoadStab()
    {
        Ability ability = new("a_stab", "Punch")
        {
            MinRange = 1,
            MaxRange = 2
        };

        Abilities.Add(ability);
    }
}
