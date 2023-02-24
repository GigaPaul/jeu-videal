using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class Pawn : MonoBehaviour
{
    [Header("Alignment")]
    public Faction Faction;
    public Settlement Settlement;

    [Header("Attributes")]
    public int EncounteredVillagers = 0;
    [Range(0, 360)]
    public int FieldOfView = 120;
    public int ViewRange = 5;

    [Range(1, 100)]
    public int MaxHitPoints;
    public int HitPoints { get; private set; }
    public List<Settlement> TraderVisitedSettlements = new();
    public float Radius = 1.5f;

    // Status
    public bool IsAlive => HitPoints > 0;

    public List<string> Occupations = new();

    public PawnMovement _PawnMovement;
    public Transform RotationTarget;
    public FlockAgent FlockAgent;
    public Animator Animator;
    public ActionManager ActionManager;
    public NavMeshAgent NavMeshAgent;
    public MultiAimConstraint HeadAim;
    public RigBuilder RigBuilder;

    public Transform RigTarget;
    public Transform FocusElement;
    public Transform Model;
    public GameObject HoverRing;
    public GameObject FocusRing;

    #nullable enable
    public Transform? Target;
    public Transform? StareTarget { get; set; }
    public FlockManager? Flock { get; set; }
    #nullable disable

    [Header("Debug")]
    public bool IsBeingDebugged = false;



    private void Awake()
    {
        // If the pawn is meant to be playable from the start, apply the player faction. Otherwise, apply the wanderer faction
        if (Faction == null)
        {
            Faction = FindObjectsOfType<Faction>().FirstOrDefault(i => i.Label == "Wanderers");
        }


        Vector2 fov = HeadAim.data.limits;
        fov.y = FieldOfView / 2;
        fov.x = -(FieldOfView / 2);
        HeadAim.data.limits = fov;

        RigBuilder.Build();
        HitPoints = MaxHitPoints;
    }





    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(AIRoutine), 0, 1);
        InvokeRepeating(nameof(ManageActions), 0, 0.25f);
    }





    private void LateUpdate()
    {
        //if(IsAlive)
        //{
        //    Rotate();

        //    //if (IsFlocking())
        //    //{
        //    //    GoForward();
        //    //}
        //}
    }

    void FixedUpdate()
    {
        ManageRings();

        //if(HitPoints == 0 && !Animator.GetBool("Death"))
        //{
        //    Animator.SetTrigger("Death");
        //}

        //if(IsAlive) 
        //{
        //    float speed = IsFlocking() && !FlockAgent.HasReachedPosition() ? 
        //        NavMeshAgent.speed : 
        //        NavMeshAgent.velocity.magnitude / NavMeshAgent.speed;

        //    //Animator.SetLayerWeight(1, speed);

        //    // Forward
        //    Vector3 localVelocity = IsFlocking() && !FlockAgent.HasReachedPosition() ? 
        //        new(0, 0, 1) : 
        //        transform.InverseTransformDirection(NavMeshAgent.velocity) / NavMeshAgent.speed;



        //    //Animator.SetFloat("Z Velocity", localVelocity.z);
        //    //Animator.SetFloat("X Velocity", localVelocity.x);
        //}
    }





    private void AIRoutine()
    {
        if(IsAlive)
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
    }





    private void ManageActions()
    {
        if(IsAlive)
        {
            // If the current action is unloaded (Pawn just reached his target)
            if (!ActionManager.QueueIsEmpty() && ActionManager.GetCurrentAction().IsUnloaded())
            {
                if(HasReachedDestination())
                {
                    // Load the current action
                    ActionManager.GetCurrentAction().Load();
                }
            }
        }
    }





    //private void Pathfind()
    //{
    //    bool cannotPathfind = !(!IsFlocking() && !ActionManager.QueueIsEmpty() && !HasReachedDestination());

    //    if (cannotPathfind)
    //        return;
        
        
    //    if(!ActionManager.GetCurrentAction().IsUnloaded())
    //    {
    //        ActionManager.ResetCurrentAction();
    //    }


    //    if (ActionManager.GetCurrentTarget() == null)
    //    {
    //        if(ActionManager.GetCurrentDestination() == null)
    //        {
    //            ActionManager.GetCurrentAction().Destination = transform.position;
    //        }
    //    }
    //    else
    //    {
    //        Vector3 subPos = (ActionManager.GetCurrentTarget().position - transform.position).normalized * Radius;

    //        Vector3 targetPosition = ActionManager.GetCurrentTarget().position - subPos;
    //        targetPosition.y = Terrain.activeTerrain.SampleHeight(targetPosition);
    //        ActionManager.GetCurrentAction().Destination = targetPosition;
    //    }


    //    NavMeshPath path = new();
    //    bool canBeReached = NavMeshAgent.CalculatePath(ActionManager.GetCurrentDestination(), path);


    //    if (!canBeReached)
    //    {
    //        Vector3 pawnDirection = ActionManager.GetCurrentDestination() + (transform.position - ActionManager.GetCurrentDestination()).normalized;

    //        if (NavMesh.SamplePosition(pawnDirection, out NavMeshHit navHit, 10f, NavMesh.AllAreas))
    //        {
    //            ActionManager.GetCurrentAction().Destination = navHit.position;
    //        }
    //    }

    //    NavMeshAgent.SetDestination(ActionManager.GetCurrentDestination());
    //}





    //private void GoForward()
    //{
    //    if (FlockAgent.HasReachedPosition())
    //        return;

    //    float speed = NavMeshAgent.speed;

    //    float distance = FlockAgent.GetDistanceFromTarget();
    //    float maxDistance = 5;

    //    float quotient = 1;

    //    if(distance < maxDistance)
    //    {
    //        quotient = distance / maxDistance;
    //    }

    //    speed += speed * quotient;

    //    if(IsBeingDebugged)
    //        Debug.Log(quotient);

    //    transform.position += Time.deltaTime * speed * transform.forward;
    //}





    //private void Rotate()
    //{
    //    Vector3 target = Vector3.zero;
    //    float finalRotationSpeed = RotationSpeed;

    //    // If the pawn must be turned towards a target
    //    //if (RotationTarget != null)
    //    if (false)
    //    {
    //        target = RotationTarget.position;
    //    }
    //    else
    //    {
    //        // If the pawn is part of a flock
    //        if(IsFlocking())
    //        {
    //            if(FlockAgent.RotationTarget != null)
    //            {
    //                target = FlockAgent.RotationTarget;
    //                finalRotationSpeed *= 2;
    //            }
    //        }
    //        else
    //        {
    //            // If the pawn must rally the destination of an ongoing action
    //            if (!ActionManager.QueueIsEmpty() && !ActionManager.GetCurrentAction().IsInactive())
    //            {
    //                Vector3 lookPosition = NavMeshAgent.velocity;
                    
    //                if(ActionManager.GetCurrentTarget() != null)
    //                {
    //                    lookPosition = ActionManager.GetCurrentTarget().position;
    //                }
    //                else if (ActionManager.GetCurrentDestination() != null && ActionManager.GetCurrentDestination() != Vector3.zero)
    //                {
    //                    lookPosition = ActionManager.GetCurrentDestination();
    //                }

    //                target = lookPosition;
    //            }
    //        }
    //    }

    //    // Don't continue if the resulting target is zero
    //    if (target == Vector3.zero)
    //        return;

    //    target -= transform.position;
    //    target.y = 0;

    //    Quaternion rotation = Quaternion.LookRotation(target);
    //    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * finalRotationSpeed);
    //}





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


    public bool IsEnemyWith(Pawn pawn)
    {
        return Faction.IsAtWarWith(pawn.Faction);
    }


    public bool HasReachedDestination()
    {
        if (ActionManager.QueueIsEmpty())
            return true;

        if(ActionManager.GetCurrentTarget() != null)
        {
            float distance = Vector3.Distance(transform.position, ActionManager.GetCurrentTarget().position);
            float flooredDistance = Mathf.Floor(distance * 10) / 10;
            return flooredDistance <= Radius;
        }
        else
        {
            float distance = Vector3.Distance(transform.position, ActionManager.GetCurrentDestination());
            float flooredDistance = Mathf.Floor(distance);
            return flooredDistance <= NavMeshAgent.stoppingDistance;
        }
    }

    public bool IsMoving()
    {
        bool IsCalculatingPath = NavMeshAgent.pathPending;
        bool HasNotReachedDestination = Mathf.Floor(NavMeshAgent.remainingDistance) > NavMeshAgent.stoppingDistance;
        bool IsTraveling = NavMeshAgent.hasPath && NavMeshAgent.velocity.sqrMagnitude > 0f;

        return IsCalculatingPath || HasNotReachedDestination || IsTraveling;
    }


    public bool IsFlocking()
    {
        return Flock != null && Flock.Commander != this;
    }





    public void GoTo(Vector3 destination, bool isQueueing = false)
    {
        Action walk = new()
        {
            Label = "Moving...",
            Destination = destination
        };

        Do(walk, false, isQueueing);
    }


    public void GoTo(Transform target, bool isQueueing = false)
    {
        Action walk = new()
        {
            Label = "Moving...",
            Target = target
        };

        Do(walk, false, isQueueing);
    }





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
        if(ActionManager.QueueIsEmpty())
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
                        Action fight = new()
                        {
                            Label = "Fighting",
                            Target = pawn.transform
                        };





                        fight.StartingScript = async () =>
                        {
                            Animator.SetBool("IsFighting", true);

                            while (IsAlive && pawn.IsAlive)
                            {
                                fight.TokenSource.Token.ThrowIfCancellationRequested();

                                if (!Animator.GetBool("Punch"))
                                {
                                    Punch(pawn);
                                    int length = Animator.GetCurrentAnimatorClipInfo(0).Length * 1000;
                                    //int wait = Random.Range(0, 5000);
                                    int wait = 0;
                                    await Action.WaitFor(length + wait, fight.TokenSource.Token);
                                    //await Task.Delay();
                                }

                                await Action.WaitFor(250, fight.TokenSource.Token);
                                //await Task.Delay(250, fight.TokenSource.Token);
                            }

                            Animator.SetBool("IsFighting", false);
                        };



                        Do(fight);
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

    }





    public void Do(Action action, bool isImmediate = false, bool isQueueing = false)
    {
        action.Actor = this;

        if (!isQueueing)
        {
            ActionManager.ClearActionQueue();
        }

        if (isImmediate)
        {
            ActionManager.Queue.Insert(0, action);
        }
        else
        {
            ActionManager.Queue.Add(action);
        }
    }





    public void Punch(Pawn pawn)
    {
        Animator.SetTrigger("Punch");
        pawn.TakeDamage(10);
    }

    public void TakeDamage(int damage)
    {
        HitPoints -= damage;

        if(HitPoints < 0)
        {
            HitPoints = 0;
        }
    }





    public void Routine()
    {
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
        //else if(Occupations.Contains("warrior"))
        //{
        //    if(Settlement != null)
        //    {
        //        WarriorRoutine();
        //    }
        //}
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
            Destination = waypoint,
            StartingScript = () =>
            {
                EncounteredVillagers = 0;
                Say($"Hello {nearestSettlement.Label}!");

                LocalVillagers = FindObjectsOfType<Pawn>().Where(i => i.Settlement == nearestSettlement && i.Occupations.Contains("trader")).ToList();

                foreach (Pawn villager in LocalVillagers)
                {
                    

                    Action buyFromTrader = new()
                    {
                        Label = "Buying",
                        Target = transform,
                        StartingScript = () =>
                        {
                            villager.Say("Hello trader!");
                            EncounteredVillagers++;
                            return Task.FromResult(0);
                        }
                    };

                    villager.Do(buyFromTrader);
                }

                return Task.FromResult(0);
            },

            SuccessCondition = () =>
            {
                return EncounteredVillagers == LocalVillagers.Count;
            },

            SuccessScript = () =>
            {
                Say("I've seen everyone in this village, bye!");

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
            Target = randomWorkingStation
        };

        working.StartingScript = async () =>
        {
            Animator.SetBool("IsWorking", true);
            await Task.Delay((int)(waitingTime * 1000), working.TokenSource.Token);
            Animator.SetBool("IsWorking", false);
        };

        Action returnResources = new()
        {
            Label = "Returning resources",
            Target = Settlement.Storage,
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
            Target = Settlement.Inn,
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
            Destination = wanderPoint
        };

        wander.StartingScript = async () =>
        {
            await Task.Delay((int)(waitingTime * 1000), wander.TokenSource.Token);
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
