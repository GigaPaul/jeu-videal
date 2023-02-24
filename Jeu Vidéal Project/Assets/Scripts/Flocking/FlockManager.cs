using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    [Header("Data")]
    public Pawn Commander;
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

    //private void Start()
    //{
    //    InvokeRepeating(nameof(SetAgentTargets), 0, 0.5f);
    //}

    private void FixedUpdate()
    {
        SetAgentTargets();
    }


    public void SetAgentTargets()
    {
        for(int i = 0; i < Members.Count; i++)
        {
            Pawn member = Members[i];

            if (member == Commander)
                continue;

            int row = Mathf.CeilToInt(i / UnitsPerRow);
            int col = (i - row * UnitsPerRow) - Mathf.CeilToInt(UnitsPerRow / 2);

            Vector3 destination = Commander.transform.position - Commander.transform.forward;

            Vector3 offset = new(col, 0, row);
            Vector3 newPos = destination - offset;

            newPos = Commander.transform.rotation * (newPos - destination) + destination;

            //thisWarrior.GoTo(newPos);


            FlockAgent agent = member.FlockAgent;
            agent.PositionTarget = newPos;
            //agent.TestSphere.position = newPos;
        }
    }



    public void MarchOn(Vector3 destination)
    {
        Commander.GoTo(destination);
        //flock.Go(startPos);
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



    public void AddMember(Pawn pawn)
    {
        Members.Add(pawn);
        pawn.Flock = this;
    }



    public void AddMember(List<Pawn> pawns)
    {
        foreach(Pawn pawn in pawns)
        {
            AddMember(pawn);
        }
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
