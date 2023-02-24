using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        Animator.SetLayerWeight(1, _PawnMovement.SpeedQuotient * 2);
        Animator.SetFloat("Z Velocity", _PawnMovement.Velocity.z);
        Animator.SetFloat("X Velocity", _PawnMovement.Velocity.x);
    }
}
