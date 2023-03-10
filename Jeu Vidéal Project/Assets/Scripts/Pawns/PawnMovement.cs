using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Pawn))]
public class PawnMovement : MonoBehaviour
{
    public Pawn _Pawn;
    public NavMeshAgent _NavMeshAgent;
    public FlockAgent _FlockAgent;
    public ActionManager _ActionManager;

    [Range(0f, 10f)]
    public float MaxSpeed;
    public float WalkingSpeed { get; private set; }
    public float SpeedQuotient { get; private set; }
    [Range(0f, 10f)]
    public float RotationSpeed;
    public Transform RotationTarget;
    //public Vector3 Velocity = Vector3.zero;

    public Vector3 VelocityDirection = Vector3.zero;




    // Start is called before the first frame update
    void Awake()
    {
        SpeedQuotient = 1;
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
        CalculateSpeedQuotient();
    }





    private void LateUpdate()
    {
        Rotate();
        Move();
    }





    private void Rotate()
    {
        Vector3 target = Vector3.zero;

        // If the pawn must be turned towards a target
        if (RotationTarget != null)
        {
            target = RotationTarget.position;
        }
        // If the pawn is part of a flock
        else if (_Pawn.IsFlocking())
        {
            // If the pawn is still
            if (_FlockAgent.HasReachedPosition())
            {
                Vector3 flockForward = _Pawn.Flock.transform.forward;
                target = transform.position + flockForward;
            }
            else
            {
                target = _FlockAgent.PositionTarget;
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

        //_FlockAgent.TestSphere.position = transform.position + target;

        if (target == Vector3.zero)
            return;

        Quaternion rotation = Quaternion.LookRotation(target);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * RotationSpeed);
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
        // If the flock has no target, return
        if (_FlockAgent.PositionTarget == Vector3.zero)
        {
            return;
        }

        // If pawn has reached his target, reset and return
        if (_FlockAgent.HasReachedPosition())
        {
            _FlockAgent.PositionTarget = Vector3.zero;
            return;
        }



        // Normalized vector pointing to the target's position
        Vector3 direction = _FlockAgent.PositionTarget - transform.position;

        float distToTarget = _FlockAgent.GetDistanceFromTarget();

        // If the target is far away
        if (distToTarget > 1)
        {
            // Normalize the direction
            direction = direction.normalized;

            float distVelocityToTarget = Vector3.Distance(VelocityDirection, direction);

            // Max value of distance is 1 so the character doesn't go too fast if the direction is opposed to the velocity
            // Ex : velocity = (0, 0, 1) and direction = (0, 0, -1), distance would be 2
            if (distVelocityToTarget > 1)
            {
                distVelocityToTarget = 1;
            }

            // Normalized vector poiting to the direction starting from the current velocity
            Vector3 velocityDir = (direction - VelocityDirection).normalized;

            Vector3 velocityMovement = distVelocityToTarget * Time.deltaTime * _NavMeshAgent.speed * velocityDir;
            VelocityDirection += velocityMovement;


            float distToVelocity = Vector3.Distance(Vector3.zero, VelocityDirection);

            // If the velocity is too high, normalize it
            if (distToVelocity > 1)
            {
                VelocityDirection = VelocityDirection.normalized;
            }
        }
        // Else if the target is very close
        else
        {
            VelocityDirection = direction;
        }



        _NavMeshAgent.velocity = _NavMeshAgent.speed * SpeedQuotient * VelocityDirection;
    }





    //private void GoForward()
    //{
    //    if (_FlockAgent.HasReachedPosition())
    //    {
    //        if (_NavMeshAgent.velocity != Vector3.zero)
    //            _NavMeshAgent.velocity = Vector3.zero;

    //        return;
    //    }

    //    _NavMeshAgent.velocity = MaxSpeed * SpeedQuotient * transform.forward;
    //    //transform.position += Time.deltaTime * speed * transform.forward;
    //}



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





    private void CalculateSpeedQuotient()
    {
        if (!_Pawn.IsFlocking())
        {
            return;
        }


        float distance = _FlockAgent.GetDistanceFromTarget();
        float maxDistance = 0.1f;
        float defaultValue = 1;
        float maxValue = 2;

        // If the target is twice as far from the pawn than it is allowed
        if(distance > maxDistance * maxValue)
        {
            SpeedQuotient = maxValue;
            return;
        }

        // If the pawn is too far from the target
        if(distance >= maxDistance)
        {
            SpeedQuotient = distance / maxDistance;
            return;
        }

        // By default, quotient value is 1
        SpeedQuotient = defaultValue;
    }
}
