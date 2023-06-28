using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    public Pawn _Pawn { get; set; }
    public Ability AbilityHeld { get; set; }
    float AnimationTime;
    float AbilityTime;
    bool StartEventHappened = false;
    bool HitEventHappened = false;


    private void Awake()
    {
        _Pawn = GetComponent<Pawn>();
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

        foreach(AnimationEvent thisEvent in AbilityHeld.Clip.events)
        {
            if(AnimationTime < thisEvent.time)
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

        List<AnimationStalker> stalkers = new();

        switch(AbilityHeld.Stage)
        {
            case Ability.StageType.casting:
                stalkers = AbilityHeld.CastingAnimations;
                break;

            case Ability.StageType.channeling:
                stalkers = AbilityHeld.ChannelAnimations;
                break;

            case Ability.StageType.fire:
                stalkers = AbilityHeld.FireAnimations;
                break;
        }

        // If there are no animations to play
        if (stalkers.Count() == 0)
        {
            return;
            //if(AbilityHeld.IsOnLastStage())
            //{
            //    // Ability is finished here
            //}
            //else
            //{
            //    // Go to the next stage (Cast -> Chanel -> Fire)
            //    AbilityHeld.Stage++;
            //    return;
            //}
        }

        if(!stalkers.Any(i => i.Stage == AnimationStalker.StageType.initializing))
        {
            
        }
    }




    public void Hold(Ability ability)
    {
        ResetHolder();
        AbilityHeld = ability;
    }



    public void TriggerAnimation()
    {
        // Trigger animation here
        AnimationTime = 0;

        if(AbilityHeld == null)
        {
            return;
        }

        if(AbilityHeld.AnimationTrigger == "")
        {
            return;
        }

        _Pawn.Animator.SetTrigger(AbilityHeld.AnimationTrigger);
    }

    public void ResetHolder()
    {
        AbilityHeld = null;
        AnimationTime = 0;
        StartEventHappened = true;
        HitEventHappened = true;
    }
}
