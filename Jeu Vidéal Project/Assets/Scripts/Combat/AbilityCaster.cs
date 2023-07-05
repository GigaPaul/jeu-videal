using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityCaster : MonoBehaviour
{
    public Pawn Master { get; set; }
    public Ability AbilityHeld { get; set; }

    #nullable enable
    public AnimationStalker? CurrentStalker { get; set; }
    #nullable disable

    float AnimationTime;
    float AbilityTime;
    bool StartEventHappened = false;
    bool HitEventHappened = false;


    private void Awake()
    {
        Master = GetComponent<Pawn>();
    }


    private void Update()
    {
        if(AbilityHeld == null)
        {
            return;
        }

        if(AbilityHeld.AnimationStarted())
        {
            AnimationTime += Time.deltaTime;
            AbilityTime += Time.deltaTime;
        }
    }


    private void FixedUpdate()
    {
        if(AbilityHeld == null)
        {
            return;
        }

        HandleAnimation();
    }



    void HandleAnimation()
    {
        if(AbilityHeld.AnimationStage == Ability.AnimationStageType.initializing)
        {
            TriggerAnimation();
            AbilityHeld.AnimationStage = Ability.AnimationStageType.initialized;
        }

        foreach (AnimationEvent thisEvent in AbilityHeld.FireClip.events)
        {
            if (AnimationTime < thisEvent.time)
            {
                continue;
            }

            switch(thisEvent.stringParameter)
            {
                case "start":
                    if(!StartEventHappened)
                    {
                        StartEventHappened = true;
                    }
                    break;



                case "hit":
                    if (!HitEventHappened)
                    {
                        Debug.Log("hit");
                        HitEventHappened = true;
                    }
                    break;



                case "end":
                    ResetHolder();
                    break;
            }
        }
    }



    public void NewHandleAnimation()
    {
        if(AbilityHeld == null)
        {
            return;
        }

        AnimationClip clip = null;

        switch(AbilityHeld.Stage)
        {
            case Ability.StageType.casting:
                clip = AbilityHeld.CastClip;
                break;

            case Ability.StageType.channeling:
                clip = AbilityHeld.ChannelClip;
                break;

            case Ability.StageType.fire:
                clip = AbilityHeld.FireClip;
                break;
        }

        if(clip == null)
        {
            return;
        }

        if(CurrentStalker == null)
        {
            AnimationStalker stalker = new(clip);
            CurrentStalker = stalker;
            Master._Puppeteer.Play(CurrentStalker.Clip);
        }


    }




    public void Hold(Ability ability, Pawn target = null)
    {
        Ability abilityClone = Instantiate(ability);
        abilityClone.Caster = Master;

        if(target != null)
        {
            abilityClone.Target = target;
        }

        ResetHolder();
        AbilityHeld = abilityClone;

        // If the ability has a cooldown, start it
        if (ability.HasCooldown() && Master.Knows(ability))
        {
            AbilityHolder holder = Master._PawnCombat.AbilityHolders.FirstOrDefault(i => i.AbilityHeld == ability);

            if (holder != null)
            {
                holder.StartCoolDown();
            }
        }
    }



    public void TriggerAnimation()
    {
        // Trigger animation here
        AnimationTime = 0;

        if(AbilityHeld == null)
        {
            return;
        }

        if(AbilityHeld.FireClip == null)
        {
            return;
        }

        Master._Puppeteer.Play(AbilityHeld.FireClip);
        //_Pawn._Animator.SetTrigger(AbilityHeld.AnimationTrigger);
    }

    public void ResetHolder()
    {
        AbilityHeld = null;
        AnimationTime = 0;
        StartEventHappened = false;
        HitEventHappened = false;
    }
}
