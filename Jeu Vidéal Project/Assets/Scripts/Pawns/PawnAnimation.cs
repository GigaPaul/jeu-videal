using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
public class PawnAnimation : MonoBehaviour
{
    public Animator Animator;
    public Pawn _Pawn;
    public PawnMovement _PawnMovement;





    // Update is called once per frame
    void FixedUpdate()
    {
        CheckDeath();
        CheckSpeed();
    }





    private void CheckDeath()
    {
        bool dontTriggerDeathAnim = _Pawn.IsAlive || Animator.GetBool("Death");

        if (dontTriggerDeathAnim)
            return;

        Animator.SetTrigger("Death");
    }





    private void CheckSpeed()
    {
        if (!_Pawn.IsAlive)
            return;

        //float weight = _PawnMovement.SpeedQuotient * 2;

        //if (weight > 1)
        //    weight = 1;

        float intensity = Vector3.Distance(Vector3.zero, _Pawn.NavMeshAgent.velocity) / _PawnMovement.WalkingSpeed;

        Animator.SetLayerWeight(1, intensity);



        Vector3 localVelocity = transform.InverseTransformDirection(_Pawn.NavMeshAgent.velocity);
        Animator.SetFloat("Z Velocity", localVelocity.z / _PawnMovement.MaxSpeed);
        Animator.SetFloat("X Velocity", localVelocity.x / _PawnMovement.MaxSpeed);
        //Animator.SetFloat("Z Velocity", _PawnMovement.Velocity.z * 2);
        //Animator.SetFloat("X Velocity", _PawnMovement.Velocity.x * 2);
    }
}
