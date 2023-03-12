using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockAgent : MonoBehaviour
{
    public Pawn _Pawn;
    public Vector3 RotationTarget;
    public Vector3 PositionTarget;


    public void ApplyRules()
    {
        SteeringSeparation();
    }






    public void SteeringSeparation()
    {
        //Vector3 totalForce = Vector3.zero;
        int neighboursCount = 0;
        Vector3 vCentre = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;
        //float gSpeed = 0.01f;


        foreach (Pawn member in _Pawn.Flock.Members)
        {
            // If the iterated member is the steering pawn, skip him
            if (member == this)
                continue;

            float dist = Vector3.Distance(member.transform.position, transform.position);
            bool isNotTooClose = !(dist <= _Pawn.Flock.MinNeighbourDist);

            if (isNotTooClose)
                continue;

            //Vector3 pushForce = member.transform.position - transform.position;
            //totalForce += pushForce / _Pawn.Flock.AgentRadius;
            vCentre += member.transform.position;
            neighboursCount++;

            if(dist < 1)
            {
                vAvoid += transform.position - member.transform.position;
            }

            //gSpeed += member.NavMeshAgent.velocity.magnitude;
        }


        if (neighboursCount == 0)
        {
            return;
        }


        //totalForce /= neighboursCount;
        //return totalForce * _Pawn.Flock.MaxAgentForce;


        vCentre /= neighboursCount;
        vCentre += PositionTarget - transform.position;
        Vector3 direction = (vCentre + vAvoid);

        if(direction != Vector3.zero)
        {
            RotationTarget = direction;
            //TestSphere.position = direction;
        }
    }



    public bool HasReachedPosition()
    {
        float distance = GetDistanceFromTarget();

        bool hasReachedDestination = distance <= _Pawn.NavMeshAgent.stoppingDistance;
        //bool commanderIsIdle = _Pawn.Flock.Commander.HasReachedDestination();

        //return hasReachedDestination && commanderIsIdle;
        return hasReachedDestination;
    }



    public float GetDistanceFromTarget()
    {
        float distance = Vector3.Distance(transform.position, PositionTarget);

        return Mathf.Floor(distance * 10) / 10;
    }
}
