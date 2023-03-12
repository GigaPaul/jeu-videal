using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class FlockManager : MonoBehaviour
{
    [Header("Data")]
    public Pawn Commander;
    public bool CommanderIsLeading = true;
    [Range(1, 15)]
    public int UnitsPerRow = 5;
    [Range(0f, 10f)]
    public float MinNeighbourDist = 0;
    [Range(0f, 10f)]
    public float MaxAgentForce = 5;
    [Range(0f, 10f)]
    public float AgentRadius = 1;
    [Range(0f, 10f)]
    public float MaxAgentCohesion = 2;

    public List<Pawn> Members = new();
    public FlockGrid Grid = new();
    [SerializeField]
    LineRenderer RadiusRenderer;
    //[Range(0.1f, 10f)]
    //public float Radius = 2.5f;

    private void Start()
    {
        Grid = new(Members.Count());
    }


    private void LateUpdate()
    {
        Rotate();
        Move();
        //Debug.DrawRay(transform.position, transform.forward);
    }





    private void FixedUpdate()
    {
        //SetAgentTargets();
        FindDestinations();

        //DrawRadius();

        Commander.NavMeshAgent.avoidancePriority = 49;

        if (Commander.NavMeshAgent.hasPath)
        {
            Commander.DrawPath();
        }
    }




    private void Move()
    {
        //Vector3 newPosition = FindCenterOfMass();

        bool commanderIsOutsideRadius = Vector3.Distance(Commander.transform.position, transform.position) > Grid.GetRadius();

        if (commanderIsOutsideRadius)
        {
            Vector3 pos = Commander.transform.position - Commander.transform.forward * Grid.GetRadius();
            transform.position = pos;
            return;
        }
    }




    private void Rotate()
    {

        bool commanderIsFarFromFormation = Vector3.Distance(Commander.transform.position, transform.position) > Grid.GetRadius();

        if (commanderIsFarFromFormation)
        {
            transform.rotation = Commander.transform.rotation;
            return;
        }

        Vector3 leavingPoint = FindLeavingPoint();

        if (leavingPoint == Vector3.zero)
        {
            return;
        }

        Vector3 direction = leavingPoint - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = rotation;
    }





    public void MarchOn(Vector3 destination)
    {
        Commander.GoTo(destination);
        Vector3 leavingPoint = FindLeavingPoint();

        if(leavingPoint == Vector3.zero)
        {
            return;
        }
    }





    void DrawRadius()
    {
        int steps = 100;
        RadiusRenderer.positionCount = steps;
        for (int currentStep = 0; currentStep < steps; currentStep++)
        {
            float CircumferenceProgress = (float)currentStep / steps;
            float CurrentRadian = CircumferenceProgress * 2 * Mathf.PI;
            float xScaled = Mathf.Cos(CurrentRadian);
            float zScaled = Mathf.Sin(CurrentRadian);
            float x = xScaled * Grid.GetRadius();
            float z = zScaled * Grid.GetRadius();

            Vector3 CurrentPosition = new(x, 0.25f, z);
            CurrentPosition += transform.position;

            RadiusRenderer.SetPosition(currentStep, CurrentPosition);
        }
    }





    //public void SetAgentTargets()
    //{
    //    for(int i = 0; i < Members.Count; i++)
    //    {
    //        Pawn member = Members[i];
    //        FlockAgent agent = member.FlockAgent;

    //        if (member == Commander)
    //        {
    //            continue;
    //        }

    //        int row = Mathf.CeilToInt(i / UnitsPerRow);
    //        int col = (i - row * UnitsPerRow) - Mathf.CeilToInt(UnitsPerRow / 2);

    //        Vector3 destination = Commander.transform.position - Commander.transform.forward;

    //        Vector3 offset = new(col, 0, row);
    //        Vector3 newPos = destination - offset;

    //        newPos = Commander.transform.rotation * (newPos - destination) + destination;

    //        agent.PositionTarget = newPos;
    //    }
    //}





    public void FindDestinations()
    {
        List<Pawn> remainingMembers = new();

        Vector3[] availablePositions = Grid.GetOffsetedVertices(transform);

        // First we check if the member is almost on a point
        foreach (Pawn member in Members)
        {
            if (member == Commander)
                continue;

            int index = -1;

            for (int i = 0; i < availablePositions.Length; i++)
            {

                float dist = Vector3.Distance(member.transform.position, availablePositions[i]);

                if(dist <= 0.1f)
                {
                    index = i;
                    break;
                }
            }

            // If the member isn't really close to a position
            if(index == -1)
            {
                remainingMembers.Add(member);
                continue;
            }

            member.FlockAgent.PositionTarget = availablePositions[index];

            Array.Clear(availablePositions, index, 1);
        }

        if (remainingMembers.Count == 0)
            return;

        // Then, we find the closest point for each remaining member
        while (remainingMembers.Count > 0)
        {
            Pawn member = remainingMembers[0];
            int closestIndex = -1;
            float closestDist = Mathf.Infinity;

            for(int i = 0; i < availablePositions.Length; i++)
            {
                float dist = Vector3.Distance(member.transform.position, availablePositions[i]);
                
                if(dist < closestDist)
                {
                    closestDist = dist;
                    closestIndex = i;
                }
            }


            member.FlockAgent.PositionTarget = availablePositions[closestIndex];
            Array.Clear(availablePositions, closestIndex, 1);
            remainingMembers.Remove(member);
        }

    }



    public Vector3 FindCenterOfMass()
    {
        Vector3 centerOfMass = Vector3.zero;
        int memberNumber = 0;


        foreach(Pawn member in Members)
        {
            if(member == Commander)
            {
                continue;
            }

            centerOfMass += member.transform.position;
            memberNumber++;
        }

        if(memberNumber == 0)
        {
            return Vector3.zero;
        }

        centerOfMass /= memberNumber;

        return centerOfMass;
    }




    public Vector3 FindLeavingPoint()
    {
        // If the commander has no current path
        if(!Commander.NavMeshAgent.hasPath)
        {
            return Vector3.zero;
        }

        NavMeshPath path = Commander.NavMeshAgent.path;
        Vector3 lastPoint = path.corners.Last();
        float distLastPoint = Vector3.Distance(transform.position, lastPoint);

        // If the end of the path is still inside the radius of the flock
        if(distLastPoint < Grid.GetRadius())
        {
            return Vector3.zero;
        }

        int index = -1;

        for(int i = 0; i < path.corners.Length; i++)
        {
            Vector3 point = path.corners[i];
            float dist = Vector3.Distance(transform.position, point);

            if(dist >= Grid.GetRadius())
            {
                index = i;
                break;
            }
        }

        // If there are no corner of the path outside of the flock's radius
        if(index <= 0)
        {
            return Vector3.zero;
        }


        Vector3 fromPoint = path.corners[index - 1];
        Vector3 toPoint = path.corners[index];
        Vector3 center = transform.position;

        Vector3 segmentDir = toPoint - fromPoint;

        // A line : y = mx + b
        float m = segmentDir.z / segmentDir.x;
        float b = fromPoint.z - m * fromPoint.x;

        // A circle : (x - c)² + (y - d)² = r²
        float c = center.x;
        float d = center.z;
        float r = Grid.GetRadius();

        // Calculation of the point of intersection between toPoint -> fromPoint and the radius of the flock
        float aPrime = 1 + m * m;
        float bPrime = 2 * (m * b - m * d - c);
        float cPrime = c * c + (b - d) * (b - d) - r * r;

        float rightDividend = (float)Math.Sqrt(bPrime * bPrime - 4 * aPrime * cPrime);
        float divisor = 2 * aPrime;

        // There are two possible solutions in this case
        float xOne = (-bPrime + rightDividend) / divisor;
        float xTwo = (-bPrime - rightDividend) / divisor;

        Vector3 leavingPoint = new(xOne, 0, m * xOne + b);

        bool xIsBetween = Math.Min(fromPoint.x, toPoint.x) <= leavingPoint.x && leavingPoint.x <= Math.Max(fromPoint.x, toPoint.x);
        bool zIsBetween = Math.Min(fromPoint.z, toPoint.z) <= leavingPoint.z && leavingPoint.z <= Math.Max(fromPoint.z, toPoint.z);
        bool isOnSegment = xIsBetween && zIsBetween;

        // If the first solution isn't on the segment toPoint -> fromPoint, switch to the second solution
        if (!isOnSegment)
        {
            leavingPoint = new(xTwo, 0, m * xTwo + b);
        }

        return leavingPoint;
    }





    public Vector3 SteeringSeek(Pawn pawn, Vector3 centerOfMass)
    {
        //Vector3 desired = centerOfMass - pawn.transform.position;
        //desired *= pawn.NavMeshAgent.speed
        return Vector3.zero;
    }





    public Vector3 SteeringCohesion(Pawn pawn)
    {
        Vector3 centerOfMass = pawn.transform.position;
        int neighboursCount = 1;

        foreach(Pawn member in Members)
        {
            // If the iterated member is the steering pawn, skip him
            if (member == pawn)
                continue;

            float dist = Vector3.Distance(member.transform.position, pawn.transform.position);
            bool isNotTooFar = !(dist < MaxAgentCohesion);

            if (isNotTooFar)
                continue;

            centerOfMass += member.transform.position;
            neighboursCount++;
        }

        if (neighboursCount == 1)
            return Vector3.zero;

        centerOfMass /= neighboursCount;

        return SteeringSeek(pawn, centerOfMass);
    }





    public Vector3 SteeringAlignment(Pawn pawn)
    {
        Vector3 averageHeading = Vector3.zero;
        int neighboursCount = 0;

        foreach(Pawn member in Members)
        {
            float dist = Vector3.Distance(member.transform.position, pawn.transform.position);
            bool tooFarOrNotMoving = !(dist < MaxAgentCohesion && member.IsMoving());

            if (tooFarOrNotMoving)
                continue;

            averageHeading += member.NavMeshAgent.velocity.normalized;
            neighboursCount++;
        }

        if (neighboursCount == 0)
            return Vector3.zero;

        averageHeading /= neighboursCount;

        Vector3 desired = averageHeading * pawn.NavMeshAgent.speed;
        Vector3 force = desired - pawn.NavMeshAgent.velocity;
        return force * MaxAgentForce / pawn.NavMeshAgent.speed;
    }





    public void AddMember(Pawn pawn, bool regenerateGrid = true)
    {
        Members.Add(pawn);
        pawn.Flock = this;

        if (regenerateGrid)
        {
            //int memberCount = Members.Count();
            //Grid.Height = Mathf.CeilToInt(memberCount / Grid.Width);
            Grid.Generate();
        }
    }





    public void AddMember(List<Pawn> pawns)
    {
        foreach(Pawn pawn in pawns)
        {
            AddMember(pawn, false);
        }

        //int memberCount = Members.Count();
        //Grid.Height = Mathf.CeilToInt(memberCount / Grid.Width);
        Grid.Generate();
    }





    //public void Go(Vector3 destination)
    //{
    //    transform.position = CalculatePosition();

    //    Face(destination);

    //    Members = SortMembers(destination);

    //    for (int i = 0; i < Members.Count; i++)
    //    {
    //        Pawn thisWarrior = Members[i];
    //        int row = Mathf.CeilToInt(i / UnitsPerRow);
    //        int col = (i - row * UnitsPerRow);

    //        row -= Mathf.CeilToInt(Members.Count / (UnitsPerRow * 2));
    //        col -= Mathf.CeilToInt(UnitsPerRow / 2);

    //        Vector3 offset = new(col, 0, row);
    //        Vector3 newPos = destination + offset;

    //        newPos = transform.rotation * (newPos - destination) + destination;

    //        thisWarrior.GoTo(newPos);
    //    }
    //}





    //public void Face(Vector3 point)
    //{
    //    Vector3 lookPos = point - transform.position;
    //    lookPos.y = 0;
    //    Quaternion rotation = Quaternion.LookRotation(lookPos);

    //    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1);
    //}





    //public Vector3 CalculatePosition()
    //{
    //    if(Members.Count == 1)
    //    {
    //        return Members.First().transform.position;
    //    }

    //    Bounds bounds = new(Members.First().transform.position, Vector3.zero);

    //    foreach(Pawn member in Members)
    //    {
    //        bounds.Encapsulate(member.transform.position);
    //    }

    //    return bounds.center;
    //}





    public void Disband()
    {
        Destroy(gameObject);
    }

    //public List<Pawn> SortMembers(Vector3 destination)
    //{
    //    List<Pawn> unsorted = Members;
    //    List<Pawn> sorted = new();

    //    while(unsorted.Count > 0)
    //    {
    //        Pawn farthest = unsorted.First();
    //        float dist = Vector3.Distance(farthest.transform.position, destination);

    //        foreach (Pawn member in unsorted)
    //        {
    //            float thisDist = Vector3.Distance(member.transform.position, destination);

    //            if(thisDist > dist)
    //            {
    //                farthest = member;
    //                dist = thisDist;
    //            }
    //        }

    //        unsorted.Remove(farthest);
    //        sorted.Add(farthest);
    //    }

    //    return sorted;
    //}
}
