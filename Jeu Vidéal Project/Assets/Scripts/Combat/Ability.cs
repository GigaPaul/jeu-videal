using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ability : ScriptableObject
{
    public string Name;
    public Sprite _Sprite;


    [Header("Animation")]

    //
    public AnimationClip Clip;
    public string AnimationTrigger;
    public enum AnimationStageType
    {
        initializing,
        initialized,
        start,
        hit,
        end
    }
    public AnimationStageType AnimationStage = AnimationStageType.initializing;
    //

    public List<AnimationStalker> CastingAnimations { get; set; } = new();
    public List<AnimationStalker> ChannelAnimations { get; set; } = new();
    public List<AnimationStalker> FireAnimations { get; set; } = new();

    public enum StageType
    {
        active,
        casting,
        channeling,
        fire
    }
    public StageType Stage = StageType.active;

    [Header("Stats")]
    public bool NeedsTarget = true;
    public float CastingTime;
    public float ChannelTime;
    public float CoolDownTime;
    public float MinRange = 1;
    public float MaxRange = 2;
    public Pawn Caster { get; set; }
    public Pawn Target { get; set; }


    public virtual void Activate() { }

    public virtual void BeginCoolDown()
    { 
        if(!HasCooldown())
        {
            return;
        }
    }

    public static void ResetAnimations(List<AnimationStalker> stalkers)
    {
        foreach(AnimationStalker stalker in stalkers)
        {
            stalker.Reset();
        }
    }




    public bool IsInstant()
    {
        return CastingTime == 0f;
    }

    public bool IsChannel()
    {
        return ChannelTime == 0f;
    }

    public bool HasCooldown()
    {
        return CoolDownTime > 0f;
    }

    public bool AnimationStarted()
    {
        return AnimationStage != AnimationStageType.initializing;
    }

    public bool IsOnLastStage()
    {
        return Stage == Enum.GetValues(typeof(StageType)).Cast<StageType>().Max();
    }
}
