using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PawnCombat : MonoBehaviour
{
    public Pawn Master { get; set; }
    public AbilityCaster _AbilityCaster { get; set; }
    public List<Ability> Spellbook = new();
    public List<AbilityHolder> AbilityHolders = new();
    public List<Pawn> HostilesInRange = new();
    public float AggroRange = 5;

    #nullable enable
    public Pawn? ForcedTarget { get; set; }
    public Pawn? CurrentTarget { get; set; }
    #nullable disable

    public enum StanceType
    {
        aggressive,
        defensive,
        passive
    }

    public StanceType Stance = StanceType.aggressive;





    private void Awake()
    {
        Master = GetComponent<Pawn>();
        _AbilityCaster = GetComponent<AbilityCaster>();
        InitAbilityHolders();
    }





    private void Start()
    {
        InvokeRepeating(nameof(GetHostilesInRange), 0, 0.25f);
    }





    private void Update()
    {
        ProgressAbilityCoolDowns();
    }





    private void FixedUpdate()
    {
        if(!Master.IsAlive)
        {
            return;
        }

        PurgeHostileList();
        StanceLoop();
    }





    public void InitAbilityHolders()
    {
        foreach(Ability ability in Spellbook)
        {
            if(AbilityHolders.Any(i => i.AbilityHeld == ability))
            {
                continue;
            }

            AbilityHolder holder = new(ability);
            AbilityHolders.Add(holder);
        }
    }




    public void ProgressAbilityCoolDowns()
    {
        foreach(AbilityHolder holder in AbilityHolders)
        {
            if(!holder.IsCoolingDown)
            {
                continue;
            }

            holder.CoolDown += Time.deltaTime;

            if(holder.CoolDown >= holder.AbilityHeld.CoolDownTime)
            {
                holder.StopCoolDown();
            }
        }
    }




    void StanceLoop()
    {
        // Don't continue if an ability is already being cast
        if (Master.IsCasting())
        {
            return;
        }

        switch (Stance)
        {
            case StanceType.aggressive:
                AggressiveStance();
                break;
            case StanceType.defensive:
                DefensiveStance();
                break;
            case StanceType.passive:
                PassiveStance();
                break;
        }
    }




    // Stance were the pawn actively go for their target
    void AggressiveStance()
    {
        ChoseTarget();

        // If no target was found, stop there
        if(CurrentTarget == null)
        {
            return;
        }

        // If in range for auto attack, then auto attack
        if (InRangeFor(AbilityHolders.First().AbilityHeld))
        {
            //Cast(AbilityHolders.First());
        }
        // Else, if the pawn isn't already walking, walk to a correct range
        else if(Master._ActionManager.QueueIsEmpty())
        {
            Master.GoTo(CurrentTarget.transform);
        }
    }




    // Stance where the pawn waits for their target to be in range to attack them
    void DefensiveStance()
    {
        ChoseTarget();

        // If no target was found, stop there
        if (CurrentTarget == null)
        {
            return;
        }

        // If in range for auto attack, then auto attack
        if (InRangeFor(AbilityHolders.First().AbilityHeld))
        {
            Cast(AbilityHolders.First());
        }
    }




    void PassiveStance()
    {
        if(Master.IsInCombat())
        {
            ClearTargets();
        }

        return;
    }



    void ChoseTarget()
    {
        if(ForcedTarget != null && Master.CanAttack(ForcedTarget))
        {
            SetTarget(ForcedTarget);
            return;
        }

        ForcedTarget = null;





        if(CurrentTarget != null)
        {
            if (Master.CanAttack(CurrentTarget) && InAggroRange(CurrentTarget))
            {
                return;
            }

            SetTarget(null);
        }

        if(HostilesInRange.Count == 0)
        {
            return;
        }

        SetTarget(GetClosestHostile());
    }

    void PurgeHostileList()
    {
        if(HostilesInRange.Count == 0)
        {
            return;
        }

        List<Pawn> hostilesToCheck = new();
        hostilesToCheck.AddRange(HostilesInRange);

        foreach (Pawn hostile in hostilesToCheck)
        {
            if(!Master.CanAttack(hostile))
            {
                HostilesInRange.Remove(hostile);
            }
        }
    }





    //public void Cast(Ability ability)
    //{
    //    if (ability.HasCooldown() && Spellbook.Contains(ability))
    //    {
    //        AbilityHolder holder = AbilityHolders.FirstOrDefault(i => i.AbilityHeld == ability);
    //        if (holder != null && holder.CoolDown > 0)
    //        {
    //            return;
    //        }
    //    }

    //    if (ability.NeedsTarget)
    //    {
    //        if (!Master.IsInCombat())
    //        {
    //            return;
    //        }

    //        _AbilityCaster.Cast(ability, CurrentTarget);
    //    }
    //    else
    //    {
    //        _AbilityCaster.Cast(ability);
    //    }
    //}



    public void Cast(AbilityHolder holder)
    {
        // Check
        if (holder.IsCoolingDown)
        {
            return;
        }

        if (holder.AbilityHeld.NeedsTarget)
        {
            if (!Master.IsInCombat())
            {
                return;
            }

            _AbilityCaster.Cast(holder, CurrentTarget);
        }
        else
        {
            _AbilityCaster.Cast(holder);
        }
    }




    public void GetHostilesInRange()
    {
        HostilesInRange.Clear();

        if(Stance == StanceType.passive)
        {
            return;
        }


        List<Collider> hitColliders = Physics.OverlapSphere(transform.position, AggroRange, Globals.FocusableMask)
            .Where(i => 
                i.GetComponentInParent<Pawn>() != null &&
                i.GetComponentInParent<Pawn>().Faction != Master.Faction &&
                Master.CanAttack(i.GetComponentInParent<Pawn>()))
            .ToList();

        List<Pawn> hitPawns = new();
        foreach (Collider collider in hitColliders)
        {
            Pawn pawn = collider.GetComponentInParent<Pawn>();

            if (HostilesInRange.Contains(pawn))
            {
                continue;
            }

            HostilesInRange.Add(pawn);
        }
    }




    public Pawn GetClosestHostile()
    {
        Pawn closestHostile = HostilesInRange.First();
        float dist = Mathf.Infinity;

        foreach (Pawn hostile in HostilesInRange)
        {
            float thisDist = Vector3.Distance(Master.transform.position, hostile.transform.position);

            if (thisDist >= dist)
            {
                continue;
            }

            dist = thisDist;
            closestHostile = hostile;
        }

        return closestHostile;
    }



    public bool TooCloseFor(Ability ability)
    {
        float targetDist = Vector3.Distance(transform.position, CurrentTarget.transform.position);

        return targetDist < ability.MinRange;
    }


    public bool TooFarFor(Ability ability)
    {
        float targetDist = Vector3.Distance(transform.position, CurrentTarget.transform.position);

        return targetDist > ability.MaxRange;
    }


    public bool InRangeFor(Ability ability)
    {
        return !TooCloseFor(ability) && !TooFarFor(ability);
    }


    public bool InAggroRange(Pawn target)
    {
        return Vector3.Distance(transform.position, target.transform.position) <= AggroRange;
    }



    public bool TargetIsForced()
    {
        return ForcedTarget != null && ForcedTarget == CurrentTarget;
    }



    public void SetTarget(Pawn target)
    {
        if(target == null)
        {
            if (Master.Movement.RotationTarget == CurrentTarget.transform)
            {
                Master.Movement.RotationTarget = null;
            }

            if(TargetIsForced())
            {
                ForcedTarget = null;
            }
        }
        else
        {
            if (!Master.CanAttack(target))
            {
                return;
            }

            Master.Movement.RotationTarget = target.transform;
        }

        // Combat begins here
        if(CurrentTarget == null)
        {
            //
        }

        CurrentTarget = target;
    }




    public void ForceTarget(Pawn target)
    {

        if (target != null && !Master.CanAttack(target))
        {
            return;
        }


        ForcedTarget = target;
    }



    public void ClearTargets()
    {
        ForcedTarget = null;
        SetTarget(null);
    }
}
