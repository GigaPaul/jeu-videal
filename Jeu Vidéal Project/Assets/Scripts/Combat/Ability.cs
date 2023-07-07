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
    public AnimationClip CastClip;
    public AnimationClip ChannelClip;
    public AnimationClip FireClip;
    public enum AnimationStageType
    {
        initializing,
        initialized,
        start,
        hit,
        end
    }
    public AnimationStageType AnimationStage = AnimationStageType.initializing;

    public Dictionary<StageType, AnimationClip> StageClips = new();
    //

    public List<AnimationStalker> CastingAnimations { get; set; } = new();
    public List<AnimationStalker> ChannelAnimations { get; set; } = new();
    public List<AnimationStalker> FireAnimations { get; set; } = new();

    public enum StageType
    {
        //active,
        casting,
        channeling,
        fire
    }
    public StageType Stage = StageType.casting;

    [Header("Stats")]
    public bool NeedsTarget = true;
    public float CastingTime;
    public float ChannelTime;
    public float CoolDownTime;
    public float MinRange = 1;
    public float MaxRange = 2;
    public Pawn Caster { get; set; }
    public Pawn Target { get; set; }





    private void Awake()
    {
        InitStageClips();
    }





    public virtual void Activate() { }
    public virtual void Start() { }
    public virtual void Hit() { }
    public virtual void End() { }
    public virtual void StartCoolDown()
    { 
        if(!HasCooldown())
        {
            return;
        }
    }





    private void InitStageClips()
    {
        StageClips.Add(StageType.casting, CastClip);
        StageClips.Add(StageType.channeling, ChannelClip);
        StageClips.Add(StageType.fire, FireClip);
    }

    public static void ResetAnimations(List<AnimationStalker> stalkers)
    {
        foreach(AnimationStalker stalker in stalkers)
        {
            stalker.Reset();
        }
    }




    //public void NextStage()
    //{
    //    if(!IsOnLastStage())
    //    {
    //        Stage += 1;
    //    }
    //}




    public void NextStage()
    {
        while(!IsOnLastStage())
        {
            Stage += 1;

            if(MustPassThrough(Stage))
            {
                break;
            }
        }
    }





    public StageType GetEarliestValidStage()
    {
        if (MustPassThrough(Stage))
        {
            return Stage;
        }

        foreach (StageType stageType in (StageType[]) Enum.GetValues(typeof(StageType)))
        {
            if(stageType == Stage)
            {
                continue;
            }

            if (MustPassThrough(stageType) && stageType > Stage)
            {
                return stageType;
            }
        }

        return StageType.fire;
    }





    public bool StageHasAnimations(StageType stageType)
    {
        return StageClips[stageType] != null;
    }

    public bool IsInstant()
    {
        return CastingTime == 0f;
    }

    public bool IsChannel()
    {
        return ChannelTime > 0f;
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

    public bool MustPassThrough(StageType stageType)
    {
        return stageType switch
        {
            StageType.casting => !IsInstant(),
            StageType.channeling => IsChannel(),
            StageType.fire => StageHasAnimations(StageType.fire),
            _ => true,
        };
    }
}
