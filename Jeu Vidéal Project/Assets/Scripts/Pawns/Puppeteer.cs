using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
public class Puppeteer : MonoBehaviour
{
    public AnimatorController Controller { get; set; }
    public Pawn _Pawn;
    //public List<AvatarMaskBodyPart> DefaultBodyParts = new();
    //public List<AvatarMaskBodyPart> LowerBodyParts = new();
    //public float LowerLayersWeight = 1;

    public enum Layer
    {
        idle,
        movement,
        animations,
        rightHand,
        leftHand
    }




    private void Start()
    {
        //GetController();
        //GetBodyParts();
    }





    // Update is called once per frame
    void FixedUpdate()
    {

        //CheckDeath();

        if(!_Pawn.IsAlive)
        {
            return;
        }

        CheckCombat();
        CheckSpeed();
        //CheckLowerBodyStatus();
        CheckEquipedTools();
    }





    private void GetController()
    {
        RuntimeAnimatorController runtimeController = _Pawn._Animator.runtimeAnimatorController;
        string path = AssetDatabase.GetAssetPath(runtimeController);
        Controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
    }

    //private void GetBodyParts()
    //{
    //    AvatarMaskBodyPart[] allBodyPartsArray = (AvatarMaskBodyPart[])Enum.GetValues(typeof(AvatarMaskBodyPart));
    //    List<AvatarMaskBodyPart> allBodyParts = new();
    //    allBodyParts.AddRange(allBodyPartsArray);
    //    allBodyParts.Remove(AvatarMaskBodyPart.LastBodyPart);

    //    DefaultBodyParts.AddRange(allBodyParts);
    //}




    public void PlayDeathAnim()
    {
        if (!_Pawn._Animator.GetBool("IsDead"))
        {
            _Pawn._Animator.SetBool("IsDead", true);
        }
    }




    private void CheckCombat()
    {
        if(_Pawn.IsInCombat() && !_Pawn._Animator.GetBool("IsFighting"))
        {
            _Pawn._Animator.SetBool("IsFighting", true);
        }
        else if(!_Pawn.IsInCombat() && _Pawn._Animator.GetBool("IsFighting"))
        {
            _Pawn._Animator.SetBool("IsFighting", false);
        }
    }





    private void CheckSpeed()
    {
        if (!_Pawn.IsAlive)
            return;

        float intensity = Vector3.Distance(Vector3.zero, _Pawn.NavMeshAgent.velocity) / _Pawn.Movement.WalkingSpeed;

        _Pawn._Animator.SetLayerWeight((int)Layer.movement, intensity);



        Vector3 localVelocity = transform.InverseTransformDirection(_Pawn.NavMeshAgent.velocity);
        _Pawn._Animator.SetFloat("Z Velocity", localVelocity.z / _Pawn.Movement.MaxSpeed);
        _Pawn._Animator.SetFloat("X Velocity", localVelocity.x / _Pawn.Movement.MaxSpeed);




    }



    //private void CheckLowerBodyStatus()
    //{
    //    float movementLayerWeight = _Pawn._Animator.GetLayerWeight((int)Layer.movement);
    //    // Disable lower body layers if the pawn is moving
    //    float targetLowerWeight = movementLayerWeight > 0 ? 0 : 1;

    //    if (LowerLayersWeight == targetLowerWeight)
    //    {
    //        return;
    //    }

    //    LowerLayersWeight = targetLowerWeight;

    //    foreach (AvatarMaskBodyPart bodypart in LowerBodyParts)
    //    {
    //        SetBodyPartWeight(bodypart, LowerLayersWeight);
    //    }
    //}





    private void CheckEquipedTools()
    {
        int rightIntensity = _Pawn.Attachments.RightHand.Item != null ? 1 : 0;
        int leftIntensity = _Pawn.Attachments.LeftHand.Item != null ? 1 : 0;

        _Pawn._Animator.SetLayerWeight((int)Layer.rightHand, rightIntensity);
        _Pawn._Animator.SetLayerWeight((int)Layer.leftHand, leftIntensity);
    }





    public void Play(AnimationClip clip)
    {
        int stateId = Animator.StringToHash(clip.name);
        int layerId = (int)Layer.animations;

        if (!_Pawn._Animator.HasState(layerId, stateId))
        {
            return;
        }


        float transitionDuration = clip.length * 0.2f;
        _Pawn._Animator.CrossFade(clip.name, transitionDuration, layerId);
    }




    //public void AnimateBodyparts(AnimationClip clip, List<AvatarMaskBodyPart> bodyParts = null)
    //{
    //    if(clip == null)
    //    {
    //        return;
    //    }

    //    if (bodyParts == null)
    //    {
    //        bodyParts = new();
    //    }

    //    if(bodyParts.Count == 0)
    //    {
    //        bodyParts.AddRange(DefaultBodyParts);
    //    }

    //    List<string> bodyPartsNames = bodyParts.Select(i => i.ToString()).ToList();
    //    List<int> layersToAnimate = new();





    //    // Time at which the animation will start
    //    float offset = 0f;
    //    int stateId = Animator.StringToHash(clip.name);



    //    // Look for the animation offset
    //    for (int layerIndex = 0; layerIndex < Controller.layers.Length; layerIndex++)
    //    {
    //        if (!_Pawn._Animator.HasState(layerIndex, stateId))
    //        {
    //            continue;
    //        }

    //        if (bodyPartsNames.Contains(_Pawn._Animator.GetLayerName(layerIndex)))
    //        {
    //            layersToAnimate.Add(layerIndex);
    //        }

    //        AnimatorClipInfo[] infos = _Pawn._Animator.GetCurrentAnimatorClipInfo(layerIndex);

    //        // If this layer is already playing an animation
    //        if (infos.Length > 0)
    //        {
    //            // If the animation is the same than the one we want to play
    //            if (infos[0].clip.name == clip.name)
    //            {
    //                AnimatorStateInfo stateInfo = _Pawn._Animator.GetCurrentAnimatorStateInfo(layerIndex);

    //                if (stateInfo.normalizedTime > offset)
    //                {
    //                    offset = stateInfo.normalizedTime;
    //                }
    //            }
    //        }
    //    }




    //    float transitionDuration = clip.length * 0.5f;

    //    // Start animation on layers
    //    foreach (int layerIndex in layersToAnimate)
    //    {
    //        _Pawn._Animator.CrossFade(clip.name, transitionDuration, layerIndex, offset);
    //    }
    //}


    //public void StopAnimationOnBodyparts(List<AvatarMaskBodyPart> bodyParts = null)
    //{
    //    if (bodyParts == null)
    //    {
    //        bodyParts = new();
    //    }

    //    if (bodyParts.Count == 0)
    //    {
    //        bodyParts.AddRange(DefaultBodyParts);
    //    }

    //    List<string> bodyPartsNames = bodyParts.Select(i => i.ToString()).ToList();

    //    foreach (string name in bodyPartsNames)
    //    {
    //        int layerIndex = _Pawn._Animator.GetLayerIndex(name);

    //        if(layerIndex == -1)
    //        {
    //            continue;
    //        }

    //        _Pawn._Animator.CrossFade("Void", 0.2f, layerIndex);
    //    }
    //}



    public void SetBodyPartWeight(AvatarMaskBodyPart bodypart, float weight)
    {
        int layerIndex = _Pawn._Animator.GetLayerIndex(bodypart.ToString());

        if(layerIndex == -1)
        {
            return;
        }

        _Pawn._Animator.SetLayerWeight(layerIndex, weight);
    }
}
