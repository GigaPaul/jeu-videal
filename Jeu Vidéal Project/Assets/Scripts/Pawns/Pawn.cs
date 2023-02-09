using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class Pawn : MonoBehaviour
{
    public Faction Faction;
    public Settlement Settlement;
    public List<Settlement> TraderVisitedSettlements = new();
    public float RotationSpeed;
    public GameObject HoverRing;
    public GameObject FocusRing;
    public Vector3 SpawnPoint { get; set; }
    public List<string> Occupations = new();

    public CharacterController CharacterController;
    public Animator Animator;
    public ActionManager ActionManager;
    public NavMeshAgent NavMeshAgent;
    public MultiAimConstraint HeadAim;
    public RigBuilder RigBuilder;

    public int EncounteredVillagers = 0;
    public Transform RigTarget;
    public Transform FocusElement;
    public int FieldOfView = 120;
    public int ViewRange = 5;
    public Transform Model;

#nullable enable
    public Transform? Target;
    public Transform? StareTarget { get; set; }
#nullable disable

    //Debug
    public bool IsBeingDebugged = false;
    //



    private void Awake()
    {
        // If the pawn is meant to be playable from the start, apply the player faction. Otherwise, apply the wanderer faction
        //Faction = StartsPlayable ? Globals.Factions.FirstOrDefault(i => i.Label == "Player") : Globals.Factions.FirstOrDefault(i => i.Label == "Wanderers");
        if (Faction == null)
        {
            Faction = FindObjectsOfType<Faction>().FirstOrDefault(i => i.Label == "Wanderers");
        }


        Vector2 fov = HeadAim.data.limits;
        fov.y = FieldOfView / 2;
        fov.x = -(FieldOfView / 2);
        HeadAim.data.limits = fov;

        RigBuilder.Build();
    }



    // Start is called before the first frame update
    void Start()
    {
        SpawnPoint = transform.position;

        InvokeRepeating(nameof(AIRoutine), 0, 1);
        InvokeRepeating(nameof(ManageActions), 0, 0.25f);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    private void LateUpdate()
    {
        Rotate();
        //Move();
    }

    void FixedUpdate()
    {
        ManageRings();

        float speed = NavMeshAgent.velocity.magnitude / NavMeshAgent.speed;
        Animator.SetLayerWeight(1, speed);

        // Forward
        Vector3 localVelocity = transform.InverseTransformDirection(NavMeshAgent.velocity) / NavMeshAgent.speed;

        Animator.SetFloat("Z Velocity", localVelocity.z);
        Animator.SetFloat("X Velocity", localVelocity.x);
    }


    private void AIRoutine()
    {
        // If the pawn is not moving
        if (!IsMoving())
        {
            // If the pawn has nothing to do
            if (ActionManager.QueueIsEmpty())
            {
                // If the spawn isn't playable
                if (!IsPlayable())
                {
                    // Start AI routine
                    Routine();
                }
            }
        }
    }

    private void ManageActions()
    {
        // If the current action is unloaded (Pawn just reached his target)
        if (!ActionManager.QueueIsEmpty() && ActionManager.GetCurrentAction().IsUnloaded())
        {
            if (!HasReachedDestination())
            {
                Vector3 destination = ActionManager.GetCurrentDestination();
                NavMeshAgent.SetDestination(destination);
            }
            else
            {
                // Load the current action
                ActionManager.GetCurrentAction().Load();
            }
        }
    }


    private void Rotate()
    {
        if (!ActionManager.QueueIsEmpty() && !ActionManager.GetCurrentAction().IsInactive())
        {
            Vector3 lookPosition = NavMeshAgent.velocity;

            if (ActionManager.GetCurrentTargetPosition() != null && ActionManager.GetCurrentTargetPosition() != Vector3.zero)
            {
                lookPosition = (Vector3)ActionManager.GetCurrentTargetPosition() - transform.position;

            }
            
            lookPosition.y = 0;

            if(lookPosition != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(lookPosition);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * RotationSpeed);
            }
        }
    }

    private void ManageRings()
    {
        // Focus
        if(IsFocused() && !FocusRing.activeInHierarchy)
        {
            FocusRing.SetActive(true);
        }
        else if(!IsFocused() && FocusRing.activeInHierarchy)
        {
            FocusRing.SetActive(false);
        }

        // Hover
        if (IsHovered() && !HoverRing.activeInHierarchy)
        {
            HoverRing.SetActive(true);
        }
        else if (!IsHovered() && HoverRing.activeInHierarchy)
        {
            HoverRing.SetActive(false);
        }
    }

    //private void Move()
    //{
    //    // If the pawn's action queue is empty
    //    if(IsIdle())
    //    {
    //        if (!IsPlayable())
    //        {
    //            // Start AI routine
    //            Routine();
    //        }
    //    }
    //    else
    //    {
    //        // If the current action hasn't been loaded yet (Pawn needs to travel there first)
    //        if(ActionManager.GetCurrentAction().IsUnloaded())
    //        {
    //            // Check if the target is the correct one
    //            if (Target == null || Target != ActionManager.GetCurrentTarget())
    //            {
    //                Target = ActionManager.GetCurrentTarget();
    //            }





    //            // Start walking animation
    //            if (!Animator.GetBool("IsWalking"))
    //            {
    //                Animator.SetBool("IsWalking", true);
    //            }





    //            // Face the target
    //            Face(Target.position);

    //            // Move towards the target
    //            Vector3 RelativePos = Target.position - transform.position;
    //            Quaternion TargetedRotation = Quaternion.LookRotation(RelativePos);
    //            Vector3 RotationOffset = TargetedRotation.eulerAngles - transform.rotation.eulerAngles;
    //            Vector3 Direction = CharacterController.transform.forward;
    //            Vector3 Movement = Quaternion.Euler(0, RotationOffset.y, 0) * Direction;
    //            CharacterController.Move(MaxSpeed * Time.deltaTime * Movement);

    //            // Keep the character on ground level (Maybe need to be redone)
    //            float newY = Terrain.activeTerrain.SampleHeight(transform.position);
    //            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    //        }
    //    }
    //}

    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    if(hit.transform.GetComponent<Terrain>() == null)
    //    {
    //        // If the pawn is colliding whith his target
    //        if (hit.transform == Target)
    //        {
    //            // If the current action is unloaded (Pawn just reached his target)
    //            if (ActionManager.GetCurrentAction().IsUnloaded())
    //            {
    //                // Stop the walking animation
    //                if (Animator.GetBool("IsWalking"))
    //                {
    //                    Animator.SetBool("IsWalking", false);
    //                }

    //                // Load the current action
    //                ActionManager.GetCurrentAction().Load();
    //            }
    //        }
    //    }
    //}



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
        return Faction != null && Faction.IsPlayable();
    }



    //public bool IsIdle()
    //{
    //    return !IsMoving() && ActionManager.QueueIsEmpty();
    //}

    public bool IsEnemyWith(Pawn pawn)
    {
        return Faction.IsAtWarWith(pawn.Faction);
    }

    public bool HasReachedDestination()
    {
        float distance = Vector3.Distance(transform.position, ActionManager.GetCurrentDestination());
        float flooredDistance = Mathf.Floor(distance);
        return flooredDistance <= NavMeshAgent.stoppingDistance;
    }

    public bool IsMoving()
    {
        bool IsCalculatingPath = NavMeshAgent.pathPending;
        bool HasNotReachedDestination = Mathf.Floor(NavMeshAgent.remainingDistance) > NavMeshAgent.stoppingDistance;
        bool IsTraveling = NavMeshAgent.hasPath && NavMeshAgent.velocity.sqrMagnitude > 0f;

        return IsCalculatingPath || HasNotReachedDestination || IsTraveling;
    }





    public void GoTo(Vector3 targetPosition, bool isQueueing = false)
    {
        //Transform target = new GameObject().transform;
        //target.position = targetPosition;

        //CreateMovementAction(target, isQueueing);
        Action goAction = new()
        {
            Label = "Moving...",
            TargetPosition = targetPosition
        };

        Do(goAction, false, isQueueing);
    }


    //public void GoTo(Transform target, bool isQueueing = false)
    //{
    //    CreateMovementAction(target, isQueueing);
    //}





    //private void CreateMovementAction(Transform target, bool isQueueing)
    //{

    //    Action goAction = new()
    //    {
    //        Label = "Moving...",
    //        Destination = target
    //    };

    //    Do(goAction, false, isQueueing);
    //}





    //public void Face(Vector3 position)
    //{
    //    Vector3 relativeTarget = (position - transform.position).normalized;
    //    RotationTarget = relativeTarget;
    //}

    //public void StopFacing()
    //{
    //    RotationTarget = null;
    //}




    #nullable enable
    public void LookAt(Transform? target)
    {
        StareTarget = target;

        if(target != null && target.GetComponentInParent<Pawn>())
        {
            Pawn targetPawn = target.GetComponentInParent<Pawn>();
            Analyze(targetPawn);
        }
    }
    #nullable disable



    public void Analyze(Pawn pawn)
    {
        // If the pawn is a warrior
        if (Occupations.Contains("warrior"))
        {
            // If the target is from a rival faction
            if (pawn.Faction != Faction && IsEnemyWith(pawn))
            {
                // If the target is a warrior from another faction
                if (pawn.Occupations.Contains("warrior"))
                {
                    //Action insult = new()
                    //{
                    //    Label = "Insulting",
                    //    StartingScript = async () =>
                    //    {
                    //        //Face(pawn.transform.position);
                    //        Say("Stay away from my village.");
                    //        //pawn.Face(transform.position);
                    //        await Task.Delay(1000);
                    //    },
                    //    IsOneShot = true
                    //};

                    //Do(insult, true, true);
                }
            }
        }
    }





    public void Do(Action action, bool isImmediate = false, bool isQueueing = false)
    {
        action.Actor = this;

        if (!isQueueing)
        {
            ActionManager.ClearActionQueue();
        }

        if(action.TargetPosition == null)
        {
            action.TargetPosition = transform.position;
        }

        Vector3 targetPosition = (Vector3)action.TargetPosition;

        float x = targetPosition.x;
        float y = Terrain.activeTerrain.SampleHeight(transform.position);
        float z = targetPosition.z;
        action.Destination = new Vector3(x, y, z);

        //if(action.Destination.GetComponent<Collider>() == null)
        //{
        //    SphereCollider collider = action.Destination.gameObject.AddComponent<SphereCollider>();
        //    collider.radius = 0.1f;
        //}


        NavMeshPath path = new();
        bool canBeReached = NavMeshAgent.CalculatePath(action.Destination, path);


        if (!canBeReached)
        {
            //if (IsBeingDebugged)
            //    Debug.Log($"Can't be reached");

            Vector3 pawnDirection = action.Destination + (transform.position - action.Destination).normalized;

            if (NavMesh.SamplePosition(pawnDirection, out NavMeshHit navHit, 10f, NavMesh.AllAreas))
            //if(NavMesh.FindClosestEdge(destination, out NavMeshHit navHit, NavMesh.AllAreas))
            //if (NavMesh.Raycast((Vector3)action.Destination, transform.position, out NavMeshHit navHit, NavMesh.AllAreas))
            {
                //Vector3 pawnDirection = transform.position - navHit.position;
                //action.Destination = navHit.position + pawnDirection.normalized;
                //TestSphere.position = ;
                action.Destination = navHit.position;

                //if (IsBeingDebugged)
                //    Debug.Log(action.Destination);
            }
        }


        //if (IsBeingDebugged)
        //    Debug.Log($"{action.Destination} -> {transform.position}");


        if (isImmediate)
        {
            ActionManager.Queue.Insert(0, action);
        }
        else
        {
            ActionManager.Queue.Add(action);
        }
    }





    public void Routine()
    {
        //Debug.Log(transform.gameObject.name);
        if(Occupations.Contains("trader"))
        {
            if(Faction.Label == "Wanderers")
            {
                TraderRoutine();
            }
            else
            {
                if (Settlement != null)
                {
                    UnemployedRoutine();
                }
            }
        }
        else if(Occupations.Contains("warrior"))
        {
            if(Settlement != null)
            {
                WarriorRoutine();
            }
        }
        else if(Occupations.Contains("worker"))
        {
            if (Settlement != null)
            {
                WorkerRoutine();
            }
        }
        else if (Occupations.Contains("innkeeper"))
        {
            if (Settlement != null)
            {
                InnkeeperRoutine();
            }
        }
        else if(!Occupations.Any())
        {
            if (Settlement != null)
            {
                UnemployedRoutine();
            }
        }
    }

    public void TraderRoutine()
    {
        if(TraderVisitedSettlements.Count == Globals.Settlements.Count)
        {
            Settlement lastVisitedSettlement = TraderVisitedSettlements.Last();
            TraderVisitedSettlements.Clear();
            TraderVisitedSettlements.Add(lastVisitedSettlement);
        }


        List<Settlement> unvisitedSettlements = Globals.Settlements.Where(i => !TraderVisitedSettlements.Contains(i)).ToList();
        #nullable enable
        float nextWaypointDist = Mathf.Infinity;
        Settlement? nearestSettlement = null;
        #nullable disable

        foreach(Settlement settlement in unvisitedSettlements)
        {
            float thisDist = Vector3.Distance(settlement.transform.position, transform.position);

            if(nearestSettlement == null || thisDist < nextWaypointDist)
            {
                nextWaypointDist = thisDist;
                nearestSettlement = settlement;
            }
        }

        TraderVisitedSettlements.Add(nearestSettlement);


        List<Pawn> LocalVillagers = new();

        //Transform waypoint = new GameObject().transform;
        Vector3 waypoint = nearestSettlement.GetRandomPoint(0.5f);

        Action traderAction = new()
        {
            Label = "Trading",
            TargetPosition = waypoint,
            StartingScript = async () =>
            {
                EncounteredVillagers = 0;
                Say($"Hello {nearestSettlement.Label}!");

                LocalVillagers = FindObjectsOfType<Pawn>().Where(i => i.Settlement == nearestSettlement && i.Occupations.Contains("trader")).ToList();

                foreach (Pawn villager in LocalVillagers)
                {
                    

                    Action buyFromTrader = new()
                    {
                        Label = "Buying",
                        TargetPosition = transform.position,
                        StartingScript = () =>
                        {
                            villager.Say("Hello trader!");
                            EncounteredVillagers++;
                            return Task.FromResult(0);
                        }
                    };

                    villager.Do(buyFromTrader);
                }

                await Task.Delay(0);
            },

            SuccessCondition = () =>
            {
                return EncounteredVillagers == LocalVillagers.Count;
            },

            SuccessScript = () =>
            {
                Say("I've seen everyone in this village, bye!");
                //Destroy(waypoint.gameObject);
                return Task.FromResult(0);
            }
        };

        Do(traderAction);
    }



    public void WarriorRoutine()
    {
        //ActionManager.IsLoop = true;

        //foreach(Transform waypoint in Settlement.Patrol)
        //{
        //    GoTo(waypoint.position, true);
        //}
    }





    public void WorkerRoutine()
    {
        Transform randomWorkingStation = Settlement.WorkingStations[Random.Range(0, Settlement.WorkingStations.Count)];
        float waitingTime = 4.833f;
        ActionManager.IsLoop = true;

        Action working = new()
        {
            Label = "Working",
            TargetPosition = randomWorkingStation.position,
            StartingScript = async () =>
            {
                Animator.SetBool("IsWorking", true);
                await Task.Delay((int)(waitingTime * 1000));
                Animator.SetBool("IsWorking", false);
            }
        };

        Action returnResources = new()
        {
            Label = "Returning resources",
            TargetPosition = Settlement.Storage.position,
            StartingScript = () =>
            {
                Settlement.ResourceStock += 10;
                return Task.FromResult(0);
            }
        };

        Do(working);
        Do(returnResources, false, true);
    }





    public void InnkeeperRoutine()
    {
        Action bartending = new()
        {
            Label = "Bartending",
            TargetPosition = Settlement.Inn.position,
            StartingScript = () =>
            {
                Animator.SetBool("IsBartending", true);
                return Task.FromResult(0);
            },
            SuccessCondition = () =>
            {
                return false;
            }
        };

        Do(bartending);
    }





    public void UnemployedRoutine()
    {

        //Transform wanderPoint = new GameObject().transform;
        Vector3 wanderPoint = Settlement.GetRandomPoint();

        float waitingTime = 3;

        Action wander = new()
        {
            Label = "Wandering",
            TargetPosition = wanderPoint,
            StartingScript = async () =>
            {
                await Task.Delay((int)(waitingTime * 1000));
                //Destroy(wanderPoint.gameObject);
            }
        };

        Do(wander);
    }

    public void Say(string text)
    {
        Object ChatBubbleResource = Resources.Load("Prefabs/ChatBubble");

        GameObject Parent = GameObject.Find("Chat").gameObject;
        Transform ChatPosition = transform.Find("ChatBox").transform;

        GameObject ChatBubbleGameObject = Instantiate(ChatBubbleResource) as GameObject;
        ChatBubble ChatBubble = ChatBubbleGameObject.GetComponent<ChatBubble>();

        ChatBubble.transform.SetParent(Parent.transform, false);
        ChatBubble.Origin = ChatPosition;
        ChatBubble.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    public void AssignTo(string occupation)
    {
        Occupations.Clear();
        Occupations.Add(occupation);
        ActionManager.ClearActionQueue();
    }

    public bool HasInSights(Transform target)
    {
        Vector3 RelativeTarget = (target.position - transform.position).normalized;

        float yAngle = Vector3.Angle(transform.forward, RelativeTarget);
        float xAngle = Vector3.Angle(transform.up, RelativeTarget);

        bool yTargetSightable = yAngle <= FieldOfView / 2;
        bool xTargetSightable = 90 - FieldOfView / 2 <= xAngle && xAngle <= 90 + FieldOfView / 2;

        return yTargetSightable && xTargetSightable;
    }
}
