using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StareTarget : MonoBehaviour
{
    public Transform DefaultPosition;
    private Pawn Pawn { get; set; }
    private Transform Target { get; set; }
    public int RotationSpeed;

    private void Awake()
    {
        Pawn = GetComponentInParent<Pawn>();
        //Vector3 forward = Pawn.NavMeshAgent.transform.forward;
        //DefaultPosition.position = Pawn.FocusElement.position + forward;
        //transform.position = DefaultPosition.position;
    }

    private void LateUpdate()
    {
        if(Pawn.StareTarget != null)
        {
            Target = Pawn.StareTarget;
        }
        else
        {
            DefaultPosition.position = Pawn.FocusElement.position + Pawn.NavMeshAgent.transform.forward;
            Target = DefaultPosition;
        }

        transform.position = Vector3.Lerp(transform.position, Target.position, Time.deltaTime * RotationSpeed);
    }
}
