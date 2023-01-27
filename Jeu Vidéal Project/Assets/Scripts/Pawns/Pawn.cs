using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Pawn : NetworkBehaviour
{
    public Faction Faction;
    public Settlement Settlement;
    public List<Settlement> TraderVisitedSettlements = new();
    public CharacterController controller { get; set; }
    //public Patrol patrol { get; set; }
    public ActionQueue ActionQueue { get; set; }
    public Action ActionToStart { get; set; }
    public Action OngoingAction { get; set; }
    private Coroutine OngoingCoroutine { get; set; }
    #nullable enable
    Transform? Target { get; set; }
    #nullable disable
    //Vector3? positionTarget { get; set; }
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
        //patrol = new();
        ActionQueue = new();
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
        if (ActionToStart != null)
        {
            OngoingCoroutine = StartCoroutine(StartAction());
        }
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
            if (!IsPlayable())
            {
                Routine();
            }
        }
        // The patrol isn't empty
        else
        {
            // Go to the current waypoint
            if (Target == null || Target != ActionQueue.Actions.First().Target)
            {
                Target = ActionQueue.Actions.First().Target;
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
                Face(Target.position);

                // Move towards the target
                Vector3 RelativePos = Target.position - transform.position;
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
            }
            // Pawn has arrived at destination
            else
            {
                if(OngoingAction == null && ActionToStart == null)
                {
                    //// Go to next waypoint
                    //patrol.Next();
                    if (AnimatorController.GetBool("IsWalking"))
                    {
                        AnimatorController.SetBool("IsWalking", false);
                    }


                    //ActionQueue.Perform();
                    ActionToStart = ActionQueue.Actions.First();
                }
            }
        }
    }

    private IEnumerator StartAction()
    {
        OngoingAction = ActionToStart;
        ActionToStart = null;

        Task task = OngoingAction.Perform();
        yield return new WaitUntil(() => task.IsCompleted);

        ActionQueue.Next();
        OngoingAction = null;
    }

    public void StopAction()
    {

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
        //return patrol.IsEmpty();
        return ActionQueue.IsEmpty();
    }



    public void GoTo(Vector3 targetPosition, bool isQueueing = false)
    {
        GameObject targetGameObject = new();
        Transform target = targetGameObject.transform;
        target.position = targetPosition;

        CreateMovementAction(target, isQueueing);
    }

    public void GoTo(Transform target, bool isQueueing = false)
    {
        CreateMovementAction(target, isQueueing);
    }


    private void CreateMovementAction(Transform target, bool isQueueing)
    {
        //float castingTime = 3;

        Action goAction = new()
        {
            Label = "Moving...",
            Target = target,
            Result = async () => {
                //await Task.Delay((int)(castingTime * 1000));
                await Task.Delay(0);
            }
        };

        Do(goAction, isQueueing);
    }



    public void Face(Vector3 position)
    {
        Vector3 relativeTarget = (position - transform.position).normalized;
        rotationTarget = relativeTarget;
    }



    public void Do(Action action, bool isQueueing = false)
    {
        action.Actor = this;

        if (!isQueueing)
        {
            ActionQueue.Clear();

            OngoingAction = null;
            ActionToStart = null;

            if(OngoingCoroutine != null)
            {
                StopCoroutine(OngoingCoroutine);
            }
        }

        ActionQueue.Add(action);
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


        //GoTo(nearestSettlement.transform.position);
        float tradingTime = 5;

        Action traderAction = new()
        {
            Label = "Trading",
            Target = nearestSettlement.transform,
            Result = async () =>
            {
                Say($"Hello {nearestSettlement.Label}!");

                List<Pawn> LocalVillagers = FindObjectsOfType<Pawn>().Where(i => i.Settlement == nearestSettlement && !i.Occupations.Any()).ToList();

                foreach(Pawn villager in LocalVillagers)
                {
                    

                    Action buyFromTrader = new()
                    {
                        Label = "Buying",
                        Target = transform,
                        Result = async () =>
                        {
                            villager.Say("Hello trader!");
                            await Task.Delay(0);
                        }
                    };

                    villager.Do(buyFromTrader);
                }

                await Task.Delay((int)(tradingTime * 1000));
            }
        };

        Do(traderAction);
    }

    public void VillagerRoutine()
    {
        float maxRadius = Settlement.Size;

        // Get a random position in the settlement's range
        float angle = Random.Range(0, Mathf.PI * 2);
        float radius = Mathf.Sqrt(Random.Range(0f, 1)) * maxRadius;
        Vector3 randomPlace = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
        //Transform wanderPoint = Settlement.transform;

        //GameObject wanderGameObject = new();
        Transform wanderPoint = new GameObject().transform;
        wanderPoint.position = Settlement.transform.position + randomPlace;
        //Vector3 wanderPoint = Settlement.transform.position + randomPlace;

        // Go to this random position
        //GoTo(wanderPoint);

        float waitingTime = 3;

        Action wander = new()
        {
            Label = "Wandering",
            Target = wanderPoint,
            Result = async () =>
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
}
