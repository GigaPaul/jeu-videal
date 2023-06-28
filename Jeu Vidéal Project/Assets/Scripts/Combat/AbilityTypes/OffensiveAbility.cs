using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Offensive", menuName = "Abilities/Offensive")]
public class OffensiveAbility : Ability
{
    public int Damages = 10;

    public override void Activate()
    {   
        Target.TakeDamage(Damages);
    }
}
