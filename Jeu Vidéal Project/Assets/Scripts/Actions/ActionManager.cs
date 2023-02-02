using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    private Coroutine OngoingCoroutine { get; set; }
    private Pawn Pawn { get; set; }
    public ActionQueue Queue = new();
    public bool IsLoop { get; set; } = false;










    private void Awake()
    {
        Pawn = GetComponent<Pawn>();
    }





    // Start is called before the first frame update
    void Start()
    {
        
    }





    // Update is called once per frame
    void Update()
    {
        
    }





    private void FixedUpdate()
    {
        if(!QueueIsEmpty() && !GetCurrentAction().IsInactive())
        {
            if (GetCurrentAction().IsLoading())
            {
                OngoingCoroutine = StartCoroutine(InitializeAction());
            }
            else if(GetCurrentAction().IsInitialized())
            {
                StartAction();
            }
            else if(GetCurrentAction().IsRunning() && GetCurrentAction().HasEnded())
            {
                OngoingCoroutine = StartCoroutine(StopAction());
            }
        }
    }










    private IEnumerator InitializeAction()
    {
        GetCurrentAction().StartInitialization();
        Task task = GetCurrentAction().PerformStartingScript(); 
        yield return new WaitUntil(() => task.IsCompleted);
        GetCurrentAction().CompleteInitialization();
    }





    private void StartAction()
    {
        GetCurrentAction().Start();
    }





    private IEnumerator StopAction()
    {
        GetCurrentAction().Stop();
        Task task = GetCurrentAction().PerformSuccessScript();
        yield return new WaitUntil(() => task.IsCompleted);
        NextAction();
    }





    public void NextAction()
    {
        Action currentAction = GetCurrentAction();
        Queue.Remove(currentAction);

        if (IsLoop && !currentAction.IsOneShot)
        {
            currentAction.Unload();
            Queue.Insert(Queue.Count(), currentAction);
        }
    }





    public Transform GetCurrentTarget()
    {
        return GetCurrentAction().Target;
    }





    public Action GetCurrentAction()
    {
        return Queue.First();
    }





    public void ClearActionQueue()
    {
        Queue.Clear();
        CancelCurrentAction();
    }





    public void CancelCurrentAction()
    {
        if (OngoingCoroutine != null)
        {
            StopCoroutine(OngoingCoroutine);
        }
    }










    // BOOLEANS
    public bool QueueIsEmpty()
    {
        return !Queue.Any();
    }
}
