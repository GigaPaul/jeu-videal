using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationStalker
{
    public AnimationClip Clip;

    public string Trigger;

    public bool IsLoop = false;

    public enum StageType
    {
        initializing,
        initialized,
        start,
        hit,
        end
    }

    public StageType Stage = StageType.initializing;

    public void Reset()
    {
        Stage = StageType.initializing;
    }

    public bool IsOnLastStage()
    {
        return Stage == Enum.GetValues(typeof(StageType)).Cast<StageType>().Max();
    }

    public void PlayAnimation(Pawn pawn, List<int> layers)
    {
        foreach(int layer in layers)
        {
            pawn.Animator.CrossFade(Clip.name, 0.5f, layer);
        }
    }
}
