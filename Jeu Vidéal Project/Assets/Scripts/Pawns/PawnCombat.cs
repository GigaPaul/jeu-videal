using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PawnCombat : MonoBehaviour
{
    Pawn _Pawn { get; set; }
    public List<string> EventCatcher = new();
    public string StanceType;
    public List<Pawn> HostilesInRange = new();
    public List<Ability> Abilities = new();

    public float AggroRange = 5;

    #nullable enable
    public Pawn? ForcedTarget { get; set; }
    public Pawn? CurrentTarget { get; set; }
    public Ability? CastAbility { get; set; }
    #nullable disable

    private void Awake()
    {
        _Pawn = GetComponent<Pawn>();
    }

    private void Start()
    {
        if(StanceType == "")
        {
            StanceType = "aggressive";
        }

        LoadBasicAbilities();


        InvokeRepeating(nameof(GetHostilesInRange), 0, 0.25f);
    }

    private void FixedUpdate()
    {
        StanceLoop();
        TriggerAbility();
        CheckForEvents();
    }




    void LoadBasicAbilities()
    {
        Ability autoAttack = new("Punch")
        {
            Range = 2
        };

        Abilities.Add(autoAttack);
    }




    void StanceLoop()
    {
        if(CastAbility != null)
        {
            return;
        }

        if(CurrentTarget != null && !CurrentTarget.CanBeAttacked())
        {
            SetTarget(null);
        }


        switch(StanceType)
        {
            case "aggressive":
                AggressiveStance();
                break;
            case "defensive":
                DefensiveStance();
                break;
            case "passive":
                PassiveStance();
                break;
        }
    }




    void AggressiveStance()
    {
        if(HostilesInRange.Count == 0 && ForcedTarget == null)
        {
            return;
        }

        if(ForcedTarget != null && CurrentTarget != ForcedTarget)
        {
            Debug.Log("ping");
            SetTarget(ForcedTarget);
        }

        bool needNewTarget = CurrentTarget == null || (!TargetIsForced() && Vector3.Distance(transform.position, CurrentTarget.transform.position) > AggroRange);

        // Find a target if the Pawn doesn't have one or if his current target is too far away
        if (needNewTarget)
        {
            SetTarget(GetClosestHostile());
        }

        float targetDist = Vector3.Distance(transform.position, CurrentTarget.transform.position);

        if(targetDist > Abilities.First().Range)
        {
            _Pawn.GoTo(CurrentTarget.transform);
        }
        else
        {
            _Pawn.Cast(Abilities.First(), CurrentTarget);
        }
    }




    void DefensiveStance()
    {

    }




    void PassiveStance()
    {

    }




    public void GetHostilesInRange()
    {
        HostilesInRange.Clear();

        if (_Pawn.Faction.Id != "g_player")
        {
            return;
        }

        if(StanceType == "passive")
        {
            return;
        }


        List<Collider> hitColliders = Physics.OverlapSphere(transform.position, AggroRange, Globals.FocusableMask)
            .Where(i => i.GetComponentInParent<Pawn>() != null && i.GetComponentInParent<Pawn>().CanBeAttacked())
            .ToList();

        List<Pawn> hitPawns = new();
        foreach (Collider collider in hitColliders)
        {
            Pawn pawn = collider.GetComponentInParent<Pawn>();

            if (pawn.Faction.Id != "g_bandits" || HostilesInRange.Contains(pawn))
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





    // Triggers the ability's animation
    void TriggerAbility()
    {
        if(CastAbility == null)
        {
            return;
        }

        if (CastAbility.HasBeenTriggered)
        {
            return;
        }

        if(CastAbility.Target == null)
        {
            return;
        }

        Vector3 currentPosition = _Pawn.transform.position;
        Vector3 targetPosition = CastAbility.Target.transform.position;

        if (Vector3.Distance(currentPosition, targetPosition) > CastAbility.Range)
        {
            return;
        }

        _Pawn.Animator.SetTrigger(CastAbility.TriggerLabel);
        CastAbility.HasBeenTriggered = true;
    }



    void CheckForEvents()
    {
        if(EventCatcher.Count == 0)
        {
            return;
        }





        // If the ability's animation started
        if(HasCaught("begin"))
        {

        }



        // If the ability hit the target
        if (HasCaught("hit"))
        {
            if (CastAbility != null && CastAbility.Target != null)
            {
                CastAbility.Target.TakeDamage(CastAbility.Damages);
            }
        }



        // If the ability's animation ended
        if (HasCaught("end"))
        {
            CastAbility.HasBeenTriggered = false;
            CastAbility = null;
        }
    }



    public bool HasCaught(string eventName)
    {
        bool eventHappenned = EventCatcher.Contains(eventName);

        if(eventHappenned)
        {
            EventCatcher.Remove(eventName);
        }

        return eventHappenned;
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
            if (!target.CanBeAttacked())
            {
                return;
            }

            _Pawn.Movement.RotationTarget = target.transform;
        }

        CurrentTarget = target;
    }




    public void ForceTarget(Pawn target)
    {
        if (!target.CanBeAttacked())
        {
            return;
        }

        ForcedTarget = target;
    }
}
