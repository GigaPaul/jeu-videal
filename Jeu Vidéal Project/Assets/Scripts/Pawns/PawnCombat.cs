using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PawnCombat : MonoBehaviour
{
    public Pawn _Pawn { get; set; }
    public AbilityHolder _AbilityHolder { get; set; }
    //public List<AbilityClipDTO> EventCatcher = new();
    //public List<string> EventCatcher = new();

    public enum StanceType
    {
        aggressive,
        defensive,
        passive
    }

    public StanceType Stance = StanceType.aggressive;
    public List<Pawn> HostilesInRange = new();
    public List<Ability> Abilities = new();

    public float AggroRange = 5;

    #nullable enable
    public Pawn? ForcedTarget { get; set; }
    public Pawn? CurrentTarget { get; set; }
    //public Ability? CastAbility { get; set; }

    ////
    //public AbilityTest? newCastAbility { get; set; }
    ////
#nullable disable





    private void Awake()
    {
        _Pawn = GetComponent<Pawn>();
        _AbilityHolder = GetComponent<AbilityHolder>();
    }





    private void Start()
    {
        //LoadDefaultAbilities();
        InvokeRepeating(nameof(GetHostilesInRange), 0, 0.25f);
    }





    private void FixedUpdate()
    {
        if(!_Pawn.IsAlive)
        {
            return;
        }

        PurgeHostileList();
        StanceLoop();
        //TriggerAbility();
        //CheckForEvents();
    }








    //void LoadDefaultAbilities()
    //{
    //    Ability autoAttack = Ability.Find("a_auto_attack");
    //    if(autoAttack != null)
    //    {
    //        Abilities.Add(autoAttack);
    //    }

    //    Ability slash = Ability.Find("a_slash");
    //    if (slash != null)
    //    {
    //        Abilities.Add(slash);
    //    }

    //    Ability block = Ability.Find("a_block");
    //    if (block != null)
    //    {
    //        Abilities.Add(block);
    //    }

    //    Ability stab = Ability.Find("a_stab");
    //    if (stab != null)
    //    {
    //        Abilities.Add(stab);
    //    }
    //}




    void StanceLoop()
    {
        // Don't continue if an ability is already being cast
        if (_Pawn.IsCasting())
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
        if (InRangeFor(Abilities.First()))
        {
            _Pawn.Cast(Abilities.First());
        }
        // Else, if the pawn isn't already walking, walk to a correct range
        else if(_Pawn._ActionManager.QueueIsEmpty())
        {
            _Pawn.GoTo(CurrentTarget.transform);
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
        if (InRangeFor(Abilities.First()))
        {
            _Pawn.Cast(Abilities.First());
        }
    }




    void PassiveStance()
    {
        if(_Pawn.IsInCombat())
        {
            ClearTargets();
        }

        return;
    }



    void ChoseTarget()
    {
        if(ForcedTarget != null && _Pawn.CanAttack(ForcedTarget))
        {
            SetTarget(ForcedTarget);
            return;
        }

        ForcedTarget = null;





        if(CurrentTarget != null)
        {
            if (_Pawn.CanAttack(CurrentTarget) && InAggroRange(CurrentTarget))
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
            if(!_Pawn.CanAttack(hostile))
            {
                HostilesInRange.Remove(hostile);
            }
        }
    }




    public void GetHostilesInRange()
    {
        HostilesInRange.Clear();

        ////
        //if (_Pawn.Faction.Id != "g_player")
        //{
        //    return;
        //}
        ////

        if(Stance == StanceType.passive)
        {
            return;
        }


        List<Collider> hitColliders = Physics.OverlapSphere(transform.position, AggroRange, Globals.FocusableMask)
            .Where(i => 
                i.GetComponentInParent<Pawn>() != null &&
                i.GetComponentInParent<Pawn>().Faction != _Pawn.Faction &&
                _Pawn.CanAttack(i.GetComponentInParent<Pawn>()))
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
            float thisDist = Vector3.Distance(_Pawn.transform.position, hostile.transform.position);

            if (thisDist >= dist)
            {
                continue;
            }

            dist = thisDist;
            closestHostile = hostile;
        }

        return closestHostile;
    }











    //// Triggers the ability's animation
    //void TriggerAbility()
    //{
    //    if (CastAbility == null)
    //    {
    //        return;
    //    }

    //    if (CastAbility.HasBeenTriggered)
    //    {
    //        return;
    //    }

    //    if (CastAbility.NeedsTarget)
    //    {
    //        if (CastAbility.Target == null)
    //        {
    //            return;
    //        }

    //        Vector3 currentPosition = _Pawn.transform.position;
    //        Vector3 targetPosition = CastAbility.Target.transform.position;

    //        if (Vector3.Distance(currentPosition, targetPosition) > CastAbility.MaxRange)
    //        {
    //            return;
    //        }
    //    }

    //    _Pawn.Animator.SetTrigger(CastAbility.TriggerLabel);
    //    CastAbility.HasBeenTriggered = true;
    //}



    //void CheckForEvents()
    //{
    //    if(EventCatcher.Count == 0)
    //    {
    //        return;
    //    }

    //    if(CastAbility == null)
    //    {
    //        return;
    //    }





    //    // If the ability's animation started
    //    if (HasCaught("begin"))
    //    {
    //        AbilityClipDTO status = EventCatcher.FirstOrDefault(i => i._Ability == CastAbility && i.Status == "begin");



    //        //if (_Pawn.IsPlayable())
    //        //{
    //        //    Debug.Log("==================================");
    //        //    Debug.Log("Animation started for " + foobarAnim.AnimName + " | " + EventCatcher.Count() + " | " + CastAbility?.Id);
    //        //}

    //        //EventCatcher.Remove(foobarAnim);
    //    }



    //    // If the ability hit the target
    //    if (HasCaught("hit"))
    //    {
    //        AbilityClipDTO status = EventCatcher.FirstOrDefault(i => i._Ability == CastAbility && i.Status == "hit");
    //        //AbilityAnimStatus foobarAnim = EventCatcher.FirstOrDefault(i => i.Status == "hit");
            
    //        if (CastAbility != null && CastAbility.Target != null)
    //        {
    //            //CastAbility.Target.TakeDamage(CastAbility.Damages);
    //        }
            
    //        //EventCatcher.Remove(foobarAnim);
    //    }



    //    // If the ability's animation ended
    //    if (HasCaught("end"))
    //    {
    //        AbilityClipDTO status = EventCatcher.FirstOrDefault(i => i._Ability == CastAbility && i.Status == "end");
    //        //AbilityAnimStatus foobarAnim = EventCatcher.FirstOrDefault(i => i.Status == "end");

    //        //if (_Pawn.IsPlayable())
    //        //{
    //        //    Debug.Log("Animation ended for "+ status._Ability.Id + " | " + EventCatcher.Count());
    //        //}

    //        EventCatcher.Remove(status);
    //        //CastAbility.HasBeenTriggered = false;
    //        CastAbility = null;
    //    }
    //}



    //public bool HasCaught(string eventName)
    //{
    //    bool eventHappenned = EventCatcher.Any(i => i._Ability == CastAbility && i.Status == eventName);

    //    //if(eventHappenned)
    //    //{
    //    //    AbilityAnimStatus caughtEvent = EventCatcher.FirstOrDefault(i => i._Ability == CastAbility && i.Status == eventName);
    //    //    EventCatcher.Remove(foobarAnim);
    //    //}

    //    return eventHappenned;
    //}


    //public bool HasCaught(string eventName)
    //{
    //    bool eventHappenned = EventCatcher.Contains(eventName);

    //    if (eventHappenned)
    //    {
    //        EventCatcher.Remove(eventName);
    //    }

    //    return eventHappenned;
    //}



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
            if (_Pawn.Movement.RotationTarget == CurrentTarget.transform)
            {
                _Pawn.Movement.RotationTarget = null;
            }

            if(TargetIsForced())
            {
                ForcedTarget = null;
            }
        }
        else
        {
            if (!_Pawn.CanAttack(target))
            {
                return;
            }

            _Pawn.Movement.RotationTarget = target.transform;
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

        if (target != null && !_Pawn.CanAttack(target))
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
