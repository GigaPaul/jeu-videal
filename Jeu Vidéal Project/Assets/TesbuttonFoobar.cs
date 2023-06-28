using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesbuttonFoobar : MonoBehaviour
{
    public Pawn _Pawn;



    public void OnClickUpper()
    {
        AnimThis(true, false);
    }

    public void OnClickLower()
    {
        AnimThis(false, true);
    }

    public void OnClickBoth()
    {
        AnimThis(true, true);
    }


    private void AnimThis(bool upperIsAnimated = false, bool lowerIsAnimated = false)
    {
        int lowerLayer = 1;
        int upperLayer = 3;

        if(!lowerIsAnimated && !upperIsAnimated)
        {
            return;
        }

        AnimatorClipInfo[] infosLower = _Pawn.Animator.GetCurrentAnimatorClipInfo(lowerLayer);
        AnimatorClipInfo[] infosUpper = _Pawn.Animator.GetCurrentAnimatorClipInfo(upperLayer);



        // LOWER
        if (lowerIsAnimated)
        {
            string stateLabel = "Sit Start";
            int thisLayer = lowerLayer;
            int otherLayer = upperLayer;
            float offset = 0f;



            // If an animation is being played on the lower layer
            if (infosLower.Length > 0)
            {
                // If the animation is different than the one we want to play
                if(infosLower[0].clip.name != stateLabel)
                {
                    _Pawn.Animator.CrossFade(stateLabel, 0.5f, thisLayer);
                }
                // If the animation is already being played
                else
                {
                    AnimatorStateInfo stateInfo = _Pawn.Animator.GetCurrentAnimatorStateInfo(thisLayer);
                    offset = stateInfo.normalizedTime;
                }
            }
            else
            {
                _Pawn.Animator.CrossFade(stateLabel, 0.5f, thisLayer);
            }



            if(!upperIsAnimated)
            {
                if (infosUpper.Length == 0 || infosUpper.Length > 0 && infosUpper[0].clip.name != stateLabel)
                {
                    _Pawn.Animator.CrossFade(stateLabel, 0.5f, otherLayer, offset);
                }
            }
        }




        // UPPER
        if (upperIsAnimated)
        {
            string stateLabel = "Mining2H";
            int thisLayer = upperLayer;
            int otherLayer = lowerLayer;
            float offset = 0f;





            // If an animation is being played on the lower layer
            if (infosUpper.Length > 0)
            {
                // If the animation is different than the one we want to play
                if (infosUpper[0].clip.name != stateLabel)
                {
                    _Pawn.Animator.CrossFade(stateLabel, 0.5f, thisLayer);
                }
                // If the animation is already being played
                else
                {
                    AnimatorStateInfo stateInfo = _Pawn.Animator.GetCurrentAnimatorStateInfo(thisLayer);
                    offset = stateInfo.normalizedTime;
                }
            }
            else
            {
                _Pawn.Animator.CrossFade(stateLabel, 0.5f, thisLayer);
            }



            if (!lowerIsAnimated)
            {
                if (infosLower.Length == 0 || infosLower.Length > 0 && infosLower[0].clip.name != stateLabel)
                {
                    _Pawn.Animator.CrossFade(stateLabel, 0.5f, otherLayer, offset);
                }
            }


        }
    }


}
