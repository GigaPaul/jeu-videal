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
        DefaultPosition.position = Pawn.FocusElement.position + Pawn.NavMeshAgent.transform.forward;
        transform.position = DefaultPosition.position;
    }

    private void LateUpdate()
    {
        Target = Pawn.StareTarget != null ? Pawn.StareTarget : DefaultPosition;

        transform.position = Vector3.Lerp(transform.position, Target.position, Time.deltaTime * RotationSpeed);
    }
}
