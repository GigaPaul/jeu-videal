using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationStalker
{
    public AnimationClip Clip;
    public bool IsLoop = false;

    public enum StageType
    {
        initialized,
        start,
        hit,
        end
    }

    public StageType Stage = StageType.initialized;





    public AnimationStalker(AnimationClip clip)
    {
        Clip = clip;
    }





    public void Reset()
    {
        Stage = StageType.initialized;
    }





    public bool IsOnLastStage()
    {
        return Stage == Enum.GetValues(typeof(StageType)).Cast<StageType>().Max();
    }
}
