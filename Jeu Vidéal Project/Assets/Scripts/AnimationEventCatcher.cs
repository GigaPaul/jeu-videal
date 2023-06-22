using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventCatcher : MonoBehaviour
{
    PawnCombat _PawnCombat { get; set; }

    private void Awake()
    {
        _PawnCombat = GetComponentInParent<PawnCombat>();
    }

    void Begin()
    {
        _PawnCombat.EventCatcher.Add("begin");
    }


    void Hit()
    {
        _PawnCombat.EventCatcher.Add("hit");
    }


    void End()
    {
        _PawnCombat.EventCatcher.Add("end");
    }
}
