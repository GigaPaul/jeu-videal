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
        DefaultPosition.position = transform.position;
        Pawn = GetComponentInParent<Pawn>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        Target = Pawn.StareTarget != null ? Pawn.StareTarget : DefaultPosition;

        transform.position = Vector3.Lerp(transform.position, Target.position, Time.deltaTime * RotationSpeed);
    }
}
