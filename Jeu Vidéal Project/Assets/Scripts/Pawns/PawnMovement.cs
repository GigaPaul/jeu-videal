using UnityEngine;
using UnityEngine.AI;

public class PawnMovement : MonoBehaviour
{
    public Pawn _Pawn;
    public NavMeshAgent _NavMeshAgent;
    public FlockAgent _FlockAgent;
    public ActionManager _ActionManager;

    [Range(0f, 10f)]
    public float MaxSpeed;
    public float WalkingSpeed { get; private set; }
    [Range(0f, 10f)]
    public float RotationSpeed;
    public float SpeedQuotient { get; private set; } = 0f;
    public Vector3 Velocity = Vector3.zero;





    // Start is called before the first frame update
    void Awake()
    {
        WalkingSpeed = MaxSpeed / 2;
        _NavMeshAgent.speed = WalkingSpeed;
    }




    private void Start()
    {
        InvokeRepeating(nameof(Pathfind), 0, 0.25f);
    }





    // Update is called once per frame
    void Update()
    {
        ManageSpeed();
    }





    private void LateUpdate()
    {
        Rotate();
        Move();
    }





    private void Rotate()
    {
        Vector3 target = Vector3.zero;
        float finalRotationSpeed = RotationSpeed;

        // If the pawn must be turned towards a target
        //if (RotationTarget != null)
        //if (false)
        //{
        //    target = RotationTarget.position;
        //}
        // If the pawn is part of a flock
        if (_Pawn.IsFlocking())
        {
            if (_FlockAgent.RotationTarget != null)
            {
                target = _FlockAgent.RotationTarget;
                finalRotationSpeed *= 2;
            }
        }
        else
        {
            // If the pawn must rally the destination of an ongoing action
            if (!_ActionManager.QueueIsEmpty() && !_ActionManager.GetCurrentAction().IsInactive())
            {
                Vector3 lookPosition = _NavMeshAgent.velocity;

                if (_ActionManager.GetCurrentTarget() != null)
                {
                    lookPosition = _ActionManager.GetCurrentTarget().position;
                }
                else if (_ActionManager.GetCurrentDestination() != null && _ActionManager.GetCurrentDestination() != Vector3.zero)
                {
                    lookPosition = _ActionManager.GetCurrentDestination();
                }

                target = lookPosition;
            }
        }

        // Don't continue if the resulting target is zero
        if (target == Vector3.zero)
            return;

        target -= transform.position;
        target.y = 0;

        Quaternion rotation = Quaternion.LookRotation(target);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * finalRotationSpeed);
    }





    private void Move()
    {
        if (!_Pawn.IsAlive)
            return;

        if(_Pawn.IsFlocking())
        {
            GoForward();
        }
    }





    private void GoForward()
    {
        if (_FlockAgent.HasReachedPosition())
            return;

        _NavMeshAgent.velocity = MaxSpeed * SpeedQuotient * transform.forward;
        //transform.position += Time.deltaTime * speed * transform.forward;
    }



    public void Pathfind()
    {
        bool cannotPathfind = !(!_Pawn.IsFlocking() && !_ActionManager.QueueIsEmpty() && !_Pawn.HasReachedDestination());

        if (cannotPathfind)
            return;


        if (!_ActionManager.GetCurrentAction().IsUnloaded())
        {
            _ActionManager.ResetCurrentAction();
        }


        if (_ActionManager.GetCurrentTarget() == null)
        {
            if (_ActionManager.GetCurrentDestination() == null)
            {
                _ActionManager.GetCurrentAction().Destination = transform.position;
            }
        }
        else
        {
            Vector3 subPos = (_ActionManager.GetCurrentTarget().position - transform.position).normalized * _Pawn.Radius;

            Vector3 targetPosition = _ActionManager.GetCurrentTarget().position - subPos;
            targetPosition.y = Terrain.activeTerrain.SampleHeight(targetPosition);
            _ActionManager.GetCurrentAction().Destination = targetPosition;
        }


        NavMeshPath path = new();
        bool canBeReached = _NavMeshAgent.CalculatePath(_ActionManager.GetCurrentDestination(), path);


        if (!canBeReached)
        {
            Vector3 pawnDirection = _ActionManager.GetCurrentDestination() + (transform.position - _ActionManager.GetCurrentDestination()).normalized;

            if (NavMesh.SamplePosition(pawnDirection, out NavMeshHit navHit, 10f, NavMesh.AllAreas))
            {
                _ActionManager.GetCurrentAction().Destination = navHit.position;
            }
        }

        _NavMeshAgent.SetDestination(_ActionManager.GetCurrentDestination());
    }





    private void ManageSpeed()
    {
        if(_Pawn.IsFlocking())
        {
            if(_FlockAgent.HasReachedPosition())
            {
                SpeedQuotient = 0;
                return;
            }


            float distance = _FlockAgent.GetDistanceFromTarget();
            float minDistance = 0.1f;
            float maxDistance = 0.2f;

            float quotient = 1;

            if(0 < distance && distance < minDistance)
            {
                quotient = distance / minDistance / 2;
            }
            else if(minDistance <= distance && distance < maxDistance)
            {
                quotient /= 2;
            }
            else if(maxDistance <= distance)
            {
                quotient = distance / maxDistance / 2;

            }

            if (quotient > 1)
            {
                quotient = 1;
            }


            //if (distance < maxDistance)
            //{
            //    quotient = distance / maxDistance;
            //}


            SpeedQuotient = quotient;
        }
        else
        {
            float quotient = _NavMeshAgent.velocity.magnitude / MaxSpeed;

            if (quotient > 1)
                quotient = 1;

            SpeedQuotient = quotient;
        }

        Velocity = transform.InverseTransformDirection(_NavMeshAgent.velocity) / MaxSpeed;
    }
}
