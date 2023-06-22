using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
public class PawnAnimation : MonoBehaviour
{
    public Animator Animator;
    public Pawn _Pawn;
    public PawnMovement _PawnMovement;
    public int BaseLayer = 0;
    public int WalkLayer = 1;
    public int RightHandLayer = 2;
    public int LeftHandLayer = 3;





    // Update is called once per frame
    void FixedUpdate()
    {
        CheckDeath();
        CheckCombat();
        CheckSpeed();
        CheckEquipedTools();
    }





    private void CheckDeath()
    {
        bool dontTriggerDeathAnim = _Pawn.IsAlive || Animator.GetBool("Death");

        if (dontTriggerDeathAnim)
            return;

        Animator.SetTrigger("Death");
    }




    private void CheckCombat()
    {
        if(_Pawn.IsInCombat() && !Animator.GetBool("IsFighting"))
        {
            Animator.SetBool("IsFighting", true);
        }
        else if(!_Pawn.IsInCombat() && Animator.GetBool("IsFighting"))
        {
            Animator.SetBool("IsFighting", false);
        }
    }





    private void CheckSpeed()
    {
        if (!_Pawn.IsAlive)
            return;

        float intensity = Vector3.Distance(Vector3.zero, _Pawn.NavMeshAgent.velocity) / _PawnMovement.WalkingSpeed;

        Animator.SetLayerWeight(WalkLayer, intensity);



        Vector3 localVelocity = transform.InverseTransformDirection(_Pawn.NavMeshAgent.velocity);
        Animator.SetFloat("Z Velocity", localVelocity.z / _PawnMovement.MaxSpeed);
        Animator.SetFloat("X Velocity", localVelocity.x / _PawnMovement.MaxSpeed);
    }





    public void CheckEquipedTools()
    {
        int rightIntensity = _Pawn.Attachments.RightHand.Item != null ? 1 : 0;
        int leftIntensity = _Pawn.Attachments.LeftHand.Item != null ? 1 : 0;

        Animator.SetLayerWeight(RightHandLayer, rightIntensity);
        Animator.SetLayerWeight(LeftHandLayer, leftIntensity);
    }
}
