using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pawn : NetworkBehaviour
{
    public Faction Faction;
    public Settlement Settlement;
    public List<Settlement> TraderVisitedSettlements = new();
    public CharacterController controller { get; set; }
    public Patrol patrol { get; set; }
    Vector3? positionTarget { get; set; }
    Vector3 rotationTarget { get; set; }
    //public float horizontalDirection { get; set; }
    //public float verticalDirection { get; set; }
    public float maxSpeed { get; set; }
    public float rotationSpeed { get; set; }
    public GameObject hoverRing { get; set; }
    public GameObject focusRing { get; set; }
    public Animator AnimatorController { get; set; }
    public Vector3 SpawnPoint { get; set; }
    public List<string> Occupations = new();

    //Debug
    public bool IsBeingDebugged = false;
    //



    private void Awake()
    {
        // If the pawn is meant to be playable from the start, apply the player faction. Otherwise, apply the wanderer faction
        //Faction = StartsPlayable ? Globals.Factions.FirstOrDefault(i => i.Label == "Player") : Globals.Factions.FirstOrDefault(i => i.Label == "Wanderers");
        if (Faction == null)
        {
            Faction = Globals.Factions.FirstOrDefault(i => i.Label == "Wanderers");
        }
    }



    // Start is called before the first frame update
    void Start()
    {

        //horizontalDirection = 0f;
        //verticalDirection = 0f;
        maxSpeed = 3;
        rotationSpeed = 10;
        controller = GetComponent<CharacterController>();
        patrol = new();
        AnimatorController = GetComponent<Animator>();
        SpawnPoint = transform.position;


        GameObject stateRingsPrefab = Resources.Load("Prefabs/Rings") as GameObject;
        GameObject stateRings = Instantiate(stateRingsPrefab, transform.position + Vector3.up * 0.15f, Quaternion.identity, transform);
        hoverRing = stateRings.transform.Find("HoverRing").gameObject;
        focusRing = stateRings.transform.Find("FocusRing").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        Rotate();
        Move();
    }

    void FixedUpdate()
    {
        ManageRings();
    }

    private void Rotate()
    {
        if (rotationTarget != Vector3.zero)
        {
            Quaternion AbsoluteRotation = Quaternion.LookRotation(rotationTarget);
            Quaternion NewRotation = Quaternion.Euler(0, AbsoluteRotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, NewRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void ManageRings()
    {
        // Focus
        if(IsFocused() && !focusRing.activeInHierarchy)
        {
            focusRing.SetActive(true);
        }
        else if(!IsFocused() && focusRing.activeInHierarchy)
        {
            focusRing.SetActive(false);
        }

        // Hover
        if (IsHovered() && !hoverRing.activeInHierarchy)
        {
            hoverRing.SetActive(true);
        }
        else if (!IsHovered() && hoverRing.activeInHierarchy)
        {
            hoverRing.SetActive(false);
        }
    }

    private void Move()
    {
        if(IsIdle())
        {
            if(AnimatorController.GetBool("IsWalking"))
            {
                AnimatorController.SetBool("IsWalking", false);
            }

            if (!IsPlayable())
            {
                Routine();
            }
        }
        // The patrol isn't empty
        else
        {
            // Go to the current waypoint
            if(positionTarget == null || positionTarget != patrol.Waypoints.First())
            {
                positionTarget = patrol.Waypoints.First();
            }

            // Begin to walk
            //if (verticalDirection != 1)
            //{
            //    verticalDirection = 1;
            //    AnimatorController.SetBool("IsWalking", true);
            //}
            if (!AnimatorController.GetBool("IsWalking"))
            {
                AnimatorController.SetBool("IsWalking", true);
            }

            // Face the target
            Face((Vector3)positionTarget);

            // Move towards the target
            Vector3 RelativePos = (Vector3)positionTarget - transform.position;
            Quaternion TargetedRotation = Quaternion.LookRotation(RelativePos);
            Vector3 RotationOffset = TargetedRotation.eulerAngles - transform.rotation.eulerAngles;
            //Vector3 Direction = controller.transform.forward * verticalDirection + controller.transform.right * horizontalDirection;
            Vector3 Direction = controller.transform.forward;
            Vector3 Movement = Quaternion.Euler(0, RotationOffset.y, 0) * Direction;
            controller.Move(maxSpeed * Time.deltaTime * Movement);

            // Keep the character on ground level (Maybe need to be redone)
            float newY = Terrain.activeTerrain.SampleHeight(transform.position);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            //

            // Pawn has arrived at destination
            if (((Vector3)positionTarget - transform.position).sqrMagnitude < 0.1)
            {
                // Go to next waypoint
                patrol.Next();
                //if(patrol.IsEmpty())
                //{ 
                //    positionTarget = null;
                //}



                //// If this waypoint was the last one of the patrol
                //if(positionTarget == patrol.Waypoints.Last())
                //{
                //    // If the patrol is a loop
                //    if(patrol.IsLoop)
                //    {
                //        GoTo(patrol.Waypoints.First());
                //    }
                //    // If the patrol isn't a loop
                //    else
                //    {
                //        if (verticalDirection != 0)
                //        {
                //            verticalDirection = 0;
                //            positionTarget = null;
                //            AnimatorController.SetBool("IsWalking", false);
                //            patrol.Clear();
                //        }
                //    }
                //}
                //// If the waypoint wasn't the last waypoint of the patrol
                //else
                //{
                //    int waypointId = patrol.Waypoints.IndexOf((Vector3)positionTarget);
                //    GoTo(patrol.Waypoints[waypointId + 1]);
                //}


                //patrol.RemoveAt(0);
                //if (patrol.Count > 0)
                //{
                //    GoTo(patrol[0]);
                //}
                //// Pawn is finally idle
                //else
                //{
                //    positionTarget = null;
                //}
            }
        }
    }



    public bool IsFocused()
    {
        return Globals.FocusedPawn == this;
    }


    public bool IsHovered()
    {
        return Globals.HoveredPawn == this;
    }


    public bool IsPlayable()
    {
        return Faction.IsPlayable();
    }



    public bool IsIdle()
    {
        return patrol.IsEmpty();
    }



    public void GoTo(Vector3 target, bool isQueueing = false)
    {
        // Si la patrouille est vide ou qu'on ne veut pas rajouter à la file
        if(!isQueueing)
        {
            patrol.Clear();
        }

        patrol.Add(target);
        //positionTarget = target;
    }



    public void Face(Vector3 position)
    {
        Vector3 relativeTarget = (position - transform.position).normalized;
        rotationTarget = relativeTarget;
    }





    public void Routine()
    {
        if(Occupations.Contains("trader"))
        {
            TraderRoutine();
        }
        else if(Occupations.Contains("warrior"))
        {

        }
        else
        {
            VillagerRoutine();
        }
    }

    public void TraderRoutine()
    {
        patrol.IsLoop = true;

        Vector3 abourgPosition = Globals.Settlements.FirstOrDefault(i => i.Label == "Abourg").transform.position;
        Vector3 bescheimPosition = Globals.Settlements.FirstOrDefault(i => i.Label == "Bescheim").transform.position;
        Vector3 cevillePosition = Globals.Settlements.FirstOrDefault(i => i.Label == "Ceville").transform.position;

        GoTo(abourgPosition);
        GoTo(bescheimPosition, true);
        GoTo(cevillePosition, true);
        //        if(TraderVisitedSettlements.Count == Globals.Settlements.Count)
        //        {
        //            Settlement lastVisitedSettlement = TraderVisitedSettlements.Last();
        //            TraderVisitedSettlements.Clear();
        //            TraderVisitedSettlements.Add(lastVisitedSettlement);
        //        }


        //        List<Settlement> unvisitedSettlements = Globals.Settlements.Where(i => !TraderVisitedSettlements.Contains(i)).ToList();
        //        #nullable enable
        //        float nextWaypointDist = Mathf.Infinity;
        //        Transform waypoint = transform;
        //#nullable disable

        //        foreach(Settlement unvisSetlmt in unvisitedSettlements)
        //        {
        //            foreach(Settlement setlmt in unvisitedSettlements)
        //            {
        //                float thisDist = Vector3.Distance(setlmt.transform.position, waypoint.position);
        //                if(thisDist < nextWaypointDist)
        //                {
        //                    nextWaypointDist = thisDist;
        //                    waypoint = setlmt.transform;
        //                }
        //            }

        //            Settlement NearestSettlement = waypoint.GetComponent<Settlement>();
        //            unvisitedSettlements.Remove(NearestSettlement);
        //            TraderVisitedSettlements.Add(NearestSettlement);
        //            AddToPatrol(waypoint.position);
        //        }
        //foreach (Settlement setlmt in unvisitedSettlements)
        //{
        //    float thisDist = Vector3.Distance(setlmt.transform.position, transform.position);

        //    if(thisDist < nearestSettlementDist)
        //    {
        //        nearestSettlementDist = thisDist;
        //        nearestSettlement = setlmt;
        //    }
        //}

        //AddToPatrol(nearestSettlement.transform.position);
    }

    public void VillagerRoutine()
    {
        float radius = 10;
        float x = Random.Range(Settlement.transform.position.x - radius, Settlement.transform.position.x + radius);
        float z = Random.Range(Settlement.transform.position.z - radius, Settlement.transform.position.z + radius);

        Vector3 wanderPoint = new(x, 0, z);
        GoTo(wanderPoint);
    }
}
