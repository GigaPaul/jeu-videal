using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityCaster : MonoBehaviour
{
    public Pawn Master { get; set; }
    public Ability AbilityCast { get; set; }

    #nullable enable
    public AnimationStalker? CurrentStalker { get; set; }
    #nullable disable

    float AnimationTimer;
    float AbilityTimer;
    List<Ability.StageType> TimedStages;





    private void Awake()
    {
        Master = GetComponent<Pawn>();
        TimedStages = new()
        {
            Ability.StageType.casting,
            Ability.StageType.channeling
        };
    }





    private void Update()
    {
        HandleTimers();
    }





    private void FixedUpdate()
    {
        if(AbilityCast == null)
        {
            return;
        }

        HandleAnimations();
        TriggerAnimationEvents();
        CheckForStageSuccess();
    }





    public void Cast(AbilityHolder holder, Pawn target = null)
    {
        _ = holder ?? throw new ArgumentNullException(nameof(holder));

        if (!Master.Knows(holder.AbilityHeld))
        {
            return;
        }


        // Set ability
        Ability abilityClone = Instantiate(holder.AbilityHeld);
        abilityClone.Caster = Master;

        if (target != null)
        {
            abilityClone.Target = target;
        }


        // Set correct stage
        abilityClone.Stage = abilityClone.GetEarliestValidStage();

        // Cast the ability
        ResetCaster();
        AbilityCast = abilityClone;

        // If the ability has a cooldown, start it
        if (abilityClone.HasCooldown())
        {
            holder.StartCoolDown();
        }

    }





    private void HandleAnimations()
    {
        // Only continue if an animation is not already playing
        if(CurrentStalker != null)
        {
            return;
        }

        // Get the animation
        AnimationClip clip = AbilityCast.StageClips[AbilityCast.Stage];

        if (clip == null)
        {
            return;
        }

        // Triggers animations
        AnimationStalker stalker = new(clip);
        CurrentStalker = stalker;
        AnimationTimer = 0;
        Master._Puppeteer.Play(CurrentStalker.Clip);
    }





    private void HandleTimers()
    {
        if(AbilityCast == null)
        {
            return;
        }

        // Update animation timer if an animation is currently playing
        if(CurrentStalker != null)
        {
            AnimationTimer += Time.deltaTime;
        }

        // Update ability timer if at Cast or Channel stage
        if(TimedStages.Contains(AbilityCast.Stage))
        {
            AbilityTimer += Time.deltaTime;
        }
    }





    private void TriggerAnimationEvents()
    {
        if(CurrentStalker == null)
        {
            return;
        }

        // If there are no events to check don't even bother
        if(CurrentStalker.Clip.events.Length == 0)
        {
            return;
        }

        // Check for animation timer and triggers ability events if necessary
        // Start, Hit, End
        foreach (AnimationEvent thisEvent in CurrentStalker.Clip.events)
        {
            if (AnimationTimer < thisEvent.time)
            {
                continue;
            }

            switch (thisEvent.stringParameter)
            {
                // When the animation begins
                case "start":
                    if (CurrentStalker.Stage < AnimationStalker.StageType.start)
                    {
                        CurrentStalker.Stage = AnimationStalker.StageType.start;
                        // Trigger start events here
                        AbilityCast.Start();
                    }
                    break;



                // When the animation looks like it hits its target
                case "hit":
                    if (CurrentStalker.Stage < AnimationStalker.StageType.hit)
                    {
                        CurrentStalker.Stage = AnimationStalker.StageType.hit;
                        // Trigger hit events here
                        AbilityCast.Hit();
                    }
                    break;



                // When the animation ends
                case "end":
                    if (CurrentStalker.Stage < AnimationStalker.StageType.end)
                    {
                        CurrentStalker.Stage = AnimationStalker.StageType.end;
                        // Trigger end events here
                        AbilityCast.End();
                    }
                    break;
            }
        }
    }





    private void CheckForStageSuccess()
    {
        bool abilityEnded = false;

        // If timer is up for Cast or Channel
        if(TimedStages.Contains(AbilityCast.Stage))
        {
            if(AbilityCast.Stage == Ability.StageType.casting)
            {
                abilityEnded = AbilityTimer >= AbilityCast.CastingTime;
            }
            else if(AbilityCast.Stage == Ability.StageType.channeling)
            {
                abilityEnded = AbilityTimer >= AbilityCast.ChannelTime;
            }
        }
        // If animation ended for Fire
        else
        {
            abilityEnded = CurrentStalker == null || AnimationTimer >= CurrentStalker.Clip.length;
        }



        // Go to ability next stage or end
        if(abilityEnded)
        {
            if (!AbilityCast.IsOnLastStage())
            {
                CancelCurrentAnimation();
                AbilityCast.NextStage();
            }
            else
            {
                ResetCaster();
            }
        }
    }





    private void ResetCaster()
    {
        ResetAbility();
        CancelCurrentAnimation();
    }

    private void ResetAbility()
    {
        AbilityCast = null;
        AbilityTimer = 0;
    }

    private void CancelCurrentAnimation()
    {
        CurrentStalker = null;
        AnimationTimer = 0;
    }
}
