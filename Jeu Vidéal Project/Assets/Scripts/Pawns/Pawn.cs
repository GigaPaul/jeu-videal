using System;
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
    //public int EncounteredVillagers = 0;
    [Range(0, 360)]
    public int FieldOfView = 120;
    public int ViewRange = 5;

    [Range(1, 100)]
    public int MaxHitPoints;
    public int HitPoints { get; private set; }
    //public List<Settlement> TraderVisitedSettlements = new();
    //public float Radius = 1.5f;

    // Status
    public bool IsAlive => HitPoints > 0;

    public PawnAttributes Attributes { get; set; }
    public PawnMovement Movement { get; set; }
    public PawnAttachments Attachments { get; set; }
    public FlockAgent _FlockAgent { get; set; }
    public ActionManager _ActionManager { get; set; }

    public MultiAimConstraint HeadAim;
    public NavMeshAgent NavMeshAgent;
    public RigBuilder RigBuilder;
    public Animator Animator;

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
        Attributes = GetComponent<PawnAttributes>();
        Movement = GetComponent<PawnMovement>();
        Attachments = GetComponent<PawnAttachments>();
        _FlockAgent = GetComponent<FlockAgent>();
        _ActionManager = GetComponent<ActionManager>();

        InvokeRepeating(nameof(ManageActions), 0, 0.25f);
    }

    void FixedUpdate()
    {
        ManageRings();
    }





    //private void AIRoutine()
    //{
    //    if(IsAlive)
    //    {
    //        // If the pawn is not moving
    //        if (!IsMoving())
    //        {
    //            // If the pawn has nothing to do
    //            if (ActionManager.QueueIsEmpty())
    //            {
    //                // If the spawn isn't playable
    //                if (!IsPlayable())
    //                {
    //                    // Start AI routine
    //                    Routine();
    //                }
    //            }
    //        }
    //    }
    //}





    private void ManageActions()
    {
        if(IsAlive)
        {
            // If the current action is unloaded (Pawn just reached his target)
            if (!_ActionManager.QueueIsEmpty() && _ActionManager.GetCurrentAction().IsUnloaded())
            {
                if(HasReachedDestination())
                {
                    // Load the current action
                    _ActionManager.GetCurrentAction().Load();
                }
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
        if (_ActionManager.QueueIsEmpty())
        {
            return true;
        }

        bool actionIsUnloaded = _ActionManager.GetCurrentAction().IsUnloaded();
        bool hasReachedDestination = _ActionManager.GetCurrentAction().RemainingDistance() <= NavMeshAgent.stoppingDistance;

        return actionIsUnloaded && hasReachedDestination;



        //return Vector3.Distance(transform.position, NavMeshAgent.destination) <= NavMeshAgent.stoppingDistance;

        //if(ActionManager.GetCurrentTarget() != null)
        //{
        //    float distance = Vector3.Distance(transform.position, ActionManager.GetCurrentTarget().position);
        //    float flooredDistance = Mathf.Floor(distance);
        //    //float flooredDistance = Mathf.Floor(distance * 10) / 10;

        //    Transform target = ActionManager.GetCurrentTarget();
        //    float radius = NavMeshAgent.stoppingDistance;

        //    if (target.GetComponent<NavMeshAgent>())
        //    {
        //        radius += target.GetComponent<NavMeshAgent>().radius;
        //    }



        //    return flooredDistance <= radius;
        //}
        //else
        //{
        //    float distance = Vector3.Distance(transform.position, ActionManager.GetCurrentDestination());
        //    float flooredDistance = Mathf.Floor(distance);

        //    return flooredDistance <= NavMeshAgent.stoppingDistance;
        //}
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
        //if(ActionManager.QueueIsEmpty())
        //{
        //    // If the pawn is a warrior
        //    if (Occupation == "warrior")
        //    {
        //        // If the target is from a rival faction
        //        if (pawn.Faction != Faction && IsEnemyWith(pawn))
        //        {
        //            // If the target is a warrior from another faction
        //            if (pawn.Occupation == "warrior")
        //            {
        //                Action fight = new()
        //                {
        //                    Label = "Fighting",
        //                    Target = pawn.transform
        //                };





        //                fight.StartingScript = async () =>
        //                {
        //                    Animator.SetBool("IsFighting", true);

        //                    while (IsAlive && pawn.IsAlive)
        //                    {
        //                        fight.TokenSource.Token.ThrowIfCancellationRequested();

        //                        if (!Animator.GetBool("Punch"))
        //                        {
        //                            Punch(pawn);
        //                            int length = Animator.GetCurrentAnimatorClipInfo(0).Length * 1000;
        //                            //int wait = Random.Range(0, 5000);
        //                            int wait = 0;
        //                            await Action.WaitFor(length + wait, fight.TokenSource.Token);
        //                            //await Task.Delay();
        //                        }

        //                        await Action.WaitFor(250, fight.TokenSource.Token);
        //                        //await Task.Delay(250, fight.TokenSource.Token);
        //                    }

        //                    Animator.SetBool("IsFighting", false);
        //                };



        //                Do(fight);
        //                //Action insult = new()
        //                //{
        //                //    Label = "Insulting",
        //                //    StartingScript = async () =>
        //                //    {
        //                //        //Face(pawn.transform.position);
        //                //        Say("Stay away from my village.");
        //                //        //pawn.Face(transform.position);
        //                //        await Task.Delay(1000);
        //                //    },
        //                //    IsOneShot = true
        //                //};

        //                //Do(insult, true, true);
        //            }
        //        }
        //    }
        //}

    }





    public void Do(Action action, bool isImmediate = false, bool isQueueing = false)
    {
        action.Actor = this;

        if (!isQueueing)
        {
            _ActionManager.ClearActionQueue();
        }

        if (isImmediate)
        {
            _ActionManager.Queue.Insert(0, action);
        }
        else
        {
            _ActionManager.Queue.Add(action);
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





    public void Say(string text)
    {
        UnityEngine.Object ChatBubbleResource = Resources.Load("Prefabs/ChatBubble");

        GameObject Parent = GameObject.Find("Chat").gameObject;
        Transform ChatPosition = transform.Find("ChatBox").transform;

        GameObject ChatBubbleGameObject = Instantiate(ChatBubbleResource) as GameObject;
        ChatBubble ChatBubble = ChatBubbleGameObject.GetComponent<ChatBubble>();

        ChatBubble.transform.SetParent(Parent.transform, false);
        ChatBubble.Origin = ChatPosition;
        ChatBubble.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    public void AssignTo(Type type)
    {
        //Occupation = occupation
        Component[] occupations = GetComponents(type);

        foreach (Component c in occupations)
        {
            Destroy(c);
        }

        gameObject.AddComponent(type);
        _ActionManager.ClearActionQueue();
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
