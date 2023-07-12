using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHolder
{
    public Ability AbilityHeld;
    public float CoolDown;
    public bool IsCoolingDown = false;

    public AbilityHolder(Ability ability)
    {
        AbilityHeld = ability;
    }

    public void StartCoolDown()
    {
        IsCoolingDown = true;
    }

    public void StopCoolDown()
    {
        CoolDown = 0;
        IsCoolingDown = false;
    }
}
