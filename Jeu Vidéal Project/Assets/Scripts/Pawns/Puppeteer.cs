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
    public Pawn Master { get; set; }
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




    private void Awake()
    {
        Master = GetComponent<Pawn>();
    }




    private void Start()
    {
        GetController();
        //GetBodyParts();
    }





    // Update is called once per frame
    void FixedUpdate()
    {

        //CheckDeath();

        if(!Master.IsAlive)
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
        RuntimeAnimatorController runtimeController = Master._Animator.runtimeAnimatorController;
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
        if (!Master._Animator.GetBool("IsDead"))
        {
            Master._Animator.SetBool("IsDead", true);
        }
    }




    private void CheckCombat()
    {
        if(Master.IsInCombat() && !Master._Animator.GetBool("IsFighting"))
        {
            Master._Animator.SetBool("IsFighting", true);
        }
        else if(!Master.IsInCombat() && Master._Animator.GetBool("IsFighting"))
        {
            Master._Animator.SetBool("IsFighting", false);
        }
    }





    private void CheckSpeed()
    {
        if (!Master.IsAlive)
            return;

        float intensity = Vector3.Distance(Vector3.zero, Master.NavMeshAgent.velocity) / Master.Movement.WalkingSpeed;

        Master._Animator.SetLayerWeight((int)Layer.movement, intensity);



        Vector3 localVelocity = transform.InverseTransformDirection(Master.NavMeshAgent.velocity);
        Master._Animator.SetFloat("Z Velocity", localVelocity.z / Master.Movement.MaxSpeed);
        Master._Animator.SetFloat("X Velocity", localVelocity.x / Master.Movement.MaxSpeed);




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
        int rightIntensity = Master.Attachments.RightHand.Item != null ? 1 : 0;
        int leftIntensity = Master.Attachments.LeftHand.Item != null ? 1 : 0;

        Master._Animator.SetLayerWeight((int)Layer.rightHand, rightIntensity);
        Master._Animator.SetLayerWeight((int)Layer.leftHand, leftIntensity);
    }





    public void Play(AnimationClip clip, bool withExitTime = true)
    {
        //int stateId = Animator.StringToHash(clip.name);
        //int layerId = (int)Layer.animations;

        //if (!Master._Animator.HasState(layerId, stateId))
        //{
        //    Debug.Log("Has not state");
        //    return;
        //}


        Master._Animator.SetTrigger("Start");
        Master._Animator.SetTrigger(clip.name);

        if(withExitTime)
        {
            Master._Animator.SetTrigger("StopWET");
        }


        //// We look if the state has a transition condition
        //AnimatorControllerLayer layer = Controller.layers[layerId];
        //List<AnimatorState> states = layer.stateMachine.states.Select(i => i.state).ToList();
        //AnimatorState thisState = states.FirstOrDefault(i => i.nameHash == stateId);
        //Debug.Log(thisState);
        //AnimatorStateTransition entryTransition = layer.stateMachine.anyStateTransitions.FirstOrDefault(i => i.destinationState == thisState);

        //// If it's the case, we use the Animator transition
        //if (entryTransition != null && entryTransition.conditions.Length > 0)
        //{
        //    Debug.Log("Transition found");
        //    string triggerName = entryTransition.conditions.First().parameter;

        //    Master._Animator.SetTrigger("Start");
        //    Master._Animator.SetTrigger(triggerName);
        //    Debug.Log(triggerName);
        //    //if (!Master._Animator.GetBool(triggerName))
        //    //{
        //    //    Master._Animator.SetBool(triggerName, true);
        //    //}
        //}
        //// Otherwise, we use CrossFade
        //else
        //{
        //    Debug.Log("Transition NOT found");
        //    float transitionDuration = clip.length * 0.2f;
        //    Master._Animator.CrossFade(clip.name, transitionDuration, layerId);
        //}
    }




    public void CancelCurrentClip()
    {
        Master._Animator.SetTrigger("StopInstant");
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



    //public void SetBodyPartWeight(AvatarMaskBodyPart bodypart, float weight)
    //{
    //    int layerIndex = Master._Animator.GetLayerIndex(bodypart.ToString());

    //    if(layerIndex == -1)
    //    {
    //        return;
    //    }

    //    Master._Animator.SetLayerWeight(layerIndex, weight);
    //}
}
