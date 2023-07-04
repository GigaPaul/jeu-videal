using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationEventCatcher : MonoBehaviour
{
    PawnCombat _PawnCombat { get; set; }
    AnimatorClipInfo testTruc;

    private void Awake()
    {
        _PawnCombat = GetComponentInParent<PawnCombat>();
    }

    void OnStart()
    {
        //Debug.Log("begin");




        //AnimationClip truc = _PawnCombat._Pawn.Animator.GetNextAnimatorClipInfo(0)[0].clip;
        //foreach(AnimationEvent eventFound in truc.events)
        //{
        //    Debug.Log(truc.name + " " + eventFound.functionName + " : " + eventFound.time);
        //}




        //_PawnCombat.EventCatcher.Clear();

        //if(_PawnCombat.EventCatcher.Any(i => i.Status == "init" && i.Clip == animation))
        //{
        //    List<AbilityClipDTO> status = _PawnCombat.EventCatcher.Where(i => i.Status == "init" && i.Clip == animation).ToList();

        //    foreach(AbilityClipDTO thisStatus in status)
        //    {
        //        if(thisStatus._Ability == _PawnCombat.CastAbility)
        //        {
        //            thisStatus.Status = "begin";
        //            break;
        //        }
        //    }
        //}

        ////AbilityAnimStatus foobarAnim = new("begin", _PawnCombat.CastAbility);
        ////_PawnCombat.EventCatcher.Add(foobarAnim);
    }


    void OnHit()
    {

        //AnimatorClipInfo[] truc = _PawnCombat._Pawn.Animator.GetCurrentAnimatorClipInfo(0);

        //foreach (AnimatorClipInfo test in truc)
        //{
        //    bool bidule = test.clip == testTruc.clip;
        //    Debug.Log(test.clip.name + " : " + bidule);
        //}

        //if (_PawnCombat.EventCatcher.Any(i => i.Status == "begin" && i.Clip == animation))
        //{
        //    List<AbilityClipDTO> status = _PawnCombat.EventCatcher.Where(i => i.Status == "begin" && i.Clip == animation).ToList();

        //    foreach (AbilityClipDTO thisStatus in status)
        //    {
        //        if (thisStatus._Ability == _PawnCombat.CastAbility)
        //        {
        //            thisStatus.Status = "hit";
        //            break;
        //        }
        //    }
        //}

        ////AbilityAnimStatus foobarAnim = new("hit", animName);
        ////_PawnCombat.EventCatcher.Add(foobarAnim);
    }


    void OnEnd()
    {

        //Debug.Log("end");
        //if (_PawnCombat.EventCatcher.Any(i => i.Status == "hit" && i.Clip == animation))
        //{
        //    List<AbilityClipDTO> status = _PawnCombat.EventCatcher.Where(i => i.Status == "hit" && i.Clip == animation).ToList();

        //    foreach (AbilityClipDTO thisStatus in status)
        //    {
        //        if (thisStatus._Ability == _PawnCombat.CastAbility)
        //        {
        //            thisStatus.Status = "end";
        //            break;
        //        }
        //    }
        //}

        ////AbilityAnimStatus foobarAnim = new("end", animName);
        ////_PawnCombat.EventCatcher.Add(foobarAnim);
    }

    //void Begin()
    //{
    //    _PawnCombat.EventCatcher.Add("begin");
    //}


    //void Hit()
    //{
    //    _PawnCombat.EventCatcher.Add("hit");
    //}


    //void End()
    //{
    //    _PawnCombat.EventCatcher.Add("end");
    //}
}
