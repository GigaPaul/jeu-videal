using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Pawn : MonoBehaviour
{
    public Faction Faction;
    public Settlement Settlement;
    public List<Settlement> TraderVisitedSettlements = new();
    public CharacterController Controller { get; set; }
    Vector3 RotationTarget { get; set; }
    public float MaxSpeed { get; set; }
    public float RotationSpeed { get; set; }
    public GameObject HoverRing;
    public GameObject FocusRing;
    public Animator AnimatorController { get; set; }
    public Vector3 SpawnPoint { get; set; }
    public List<string> Occupations = new();
    public ActionManager ActionManager { get; set; }
    public int EncounteredVillagers = 0;
    private MultiAimConstraint HeadAim;
    private RigBuilder RigBuilder;
    public Transform RigTarget;
    public Transform FocusElement;
    public int FieldOfView = 120;
    public int ViewRange = 5;
    public Transform Model;

#nullable enable
    Transform? Target { get; set; }
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
            Faction = Globals.Factions.FirstOrDefault(i => i.Label == "Wanderers");
        }

        ActionManager = GetComponent<ActionManager>();
        HeadAim = GetComponentInChildren<MultiAimConstraint>();
        RigBuilder = GetComponentInChildren<RigBuilder>();

        Vector2 fov = HeadAim.data.limits;
        fov.y = FieldOfView / 2;
        fov.x = -(FieldOfView / 2);
        HeadAim.data.limits = fov;

        RigBuilder.Build();
    }



    // Start is called before the first frame update
    void Start()
    {

        //horizontalDirection = 0f;
        //verticalDirection = 0f;
        MaxSpeed = 3;
        RotationSpeed = 10;
        Controller = GetComponent<CharacterController>();
        AnimatorController = GetComponentInChildren<Animator>();
        SpawnPoint = transform.position;


        //GameObject stateRingsPrefab = Resources.Load("Prefabs/Rings") as GameObject;
        //GameObject stateRings = Instantiate(stateRingsPrefab, transform.position + Vector3.up * 0.15f, Quaternion.identity, transform);
        //HoverRing = stateRings.transform.Find("HoverRing").gameObject;
        //FocusRing = stateRings.transform.Find("FocusRing").gameObject;
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
        if (RotationTarget != Vector3.zero)
        {
            Quaternion AbsoluteRotation = Quaternion.LookRotation(RotationTarget);
            Quaternion NewRotation = Quaternion.Euler(0, AbsoluteRotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, NewRotation, Time.deltaTime * RotationSpeed);
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

    private void Move()
    {
        if(IsIdle())
        {
            if (!IsPlayable())
            {
                Routine();
            }
        }
        // The patrol isn't empty
        else
        {
            // Go to the current waypoint
            if (Target == null || Target != ActionManager.GetCurrentTarget())
            {
                Target = ActionManager.Queue.First().Target;
            }

            float magnitude = 0.1f;

            if(Target.GetComponent<CharacterController>())
            {
                float padding = 1;
                magnitude = Target.GetComponent<CharacterController>().radius + padding;
            }

            bool IsNotAtDestination = (Target.position - transform.position).sqrMagnitude >= magnitude;

            if (IsNotAtDestination)
            {

                // Begin to walk
                if (!AnimatorController.GetBool("IsWalking"))
                {
                    AnimatorController.SetBool("IsWalking", true);
                }








                // Face the target
                Face(Target.position);

                // Move towards the target
                Vector3 RelativePos = Target.position - transform.position;
                Quaternion TargetedRotation = Quaternion.LookRotation(RelativePos);
                Vector3 RotationOffset = TargetedRotation.eulerAngles - transform.rotation.eulerAngles;
                //Vector3 Direction = controller.transform.forward * verticalDirection + controller.transform.right * horizontalDirection;
                Vector3 Direction = Controller.transform.forward;
                Vector3 Movement = Quaternion.Euler(0, RotationOffset.y, 0) * Direction;
                Controller.Move(MaxSpeed * Time.deltaTime * Movement);

                // Keep the character on ground level (Maybe need to be redone)
                float newY = Terrain.activeTerrain.SampleHeight(transform.position);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                //
            }
            // Pawn has arrived at destination
            else
            {
                if(ActionManager.GetCurrentAction().IsUnloaded())
                {
                    //// Go to next waypoint
                    if (AnimatorController.GetBool("IsWalking"))
                    {
                        AnimatorController.SetBool("IsWalking", false);
                    }

                    ActionManager.GetCurrentAction().Load();
                }
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
        return ActionManager.QueueIsEmpty();
    }





    public void GoTo(Vector3 targetPosition, bool isQueueing = false)
    {
        Transform target = new GameObject().transform;
        target.position = targetPosition;

        CreateMovementAction(target, isQueueing);
    }


    public void GoTo(Transform target, bool isQueueing = false)
    {
        CreateMovementAction(target, isQueueing);
    }





    private void CreateMovementAction(Transform target, bool isQueueing)
    {
        Action goAction = new()
        {
            Label = "Moving...",
            Target = target
        };

        Do(goAction, false, isQueueing);
    }





    public void Face(Vector3 position)
    {
        Vector3 relativeTarget = (position - transform.position).normalized;
        RotationTarget = relativeTarget;
    }




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
            // If the target is from another faction
            if (pawn.Faction != Faction)
            {
                // If the target is a warrior from another faction
                if (pawn.Occupations.Contains("warrior"))
                {
                    bool isInsideVillage = Vector3.Distance(transform.position, Settlement.transform.position) <= Settlement.Size;
                    if(isInsideVillage)
                    {
                        Action insult = new()
                        {
                            Label = "Insulting",
                            StartingScript = async () =>
                            {
                                Face(pawn.transform.position);
                                Say("Stay away from my village.");
                                pawn.Face(transform.position);
                                await Task.Delay(1000);
                            },
                            IsOneShot = true
                        };

                        Do(insult, true, true);
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

        if(action.Target == null)
        {
            action.Target = transform;
        }

        if(isImmediate)
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

        Action traderAction = new()
        {
            Label = "Trading",
            Target = nearestSettlement.transform,
            StartingScript = async () =>
            {
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

                await Task.Delay(0);
            },

            SuccessCondition = () =>
            {
                return EncounteredVillagers == LocalVillagers.Count;
            },

            SuccessScript = () =>
            {
                EncounteredVillagers = 0;
                Say("I've seen everyone in this village, bye!");
                return Task.FromResult(0);
            }
        };

        Do(traderAction);
    }



    public void WarriorRoutine()
    {
        ActionManager.IsLoop = true;

        foreach(Transform waypoint in Settlement.Patrol)
        {
            GoTo(waypoint.position, true);
        }

        //Vector3 Patrol1 = new(Settlement.transform.position.x - Settlement.Size, 0, Settlement.transform.position.z - Settlement.Size);
        //Vector3 Patrol2 = new(Settlement.transform.position.x - Settlement.Size, 0, Settlement.transform.position.z + Settlement.Size);
        //Vector3 Patrol3 = new(Settlement.transform.position.x + Settlement.Size, 0, Settlement.transform.position.z + Settlement.Size);
        //Vector3 Patrol4 = new(Settlement.transform.position.x + Settlement.Size, 0, Settlement.transform.position.z - Settlement.Size);

        //GoTo(Patrol1, true);
        //GoTo(Patrol2, true);
        //GoTo(Patrol3, true);
        //GoTo(Patrol4, true);
    }





    public void WorkerRoutine()
    {
        Transform randomWorkingStation = Settlement.WorkingStations[Random.Range(0, Settlement.WorkingStations.Count)];
        float waitingTime = 4.833f;
        ActionManager.IsLoop = true;

        Action working = new()
        {
            Label = "Working",
            Target = randomWorkingStation,
            StartingScript = async () =>
            {
                AnimatorController.SetBool("IsWorking", true);
                await Task.Delay((int)(waitingTime * 1000));
                AnimatorController.SetBool("IsWorking", false);
            }
        };

        Action returnResources = new()
        {
            Label = "Returning resources",
            Target = Settlement.transform
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
                Face(Settlement.transform.position);
                AnimatorController.SetBool("IsBartending", true);
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
        float maxRadius = Settlement.Size;

        // Get a random position in the settlement's range
        float angle = Random.Range(0, Mathf.PI * 2);
        float radius = Mathf.Sqrt(Random.Range(0f, 1)) * maxRadius;
        Vector3 randomPlace = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

        Transform wanderPoint = new GameObject().transform;
        wanderPoint.position = Settlement.transform.position + randomPlace;

        float waitingTime = 3;

        Action wander = new()
        {
            Label = "Wandering",
            Target = wanderPoint,
            StartingScript = async () =>
            {
                await Task.Delay((int)(waitingTime * 1000));
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
