using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public Coroutine OngoingCoroutine { get; set; }
    private Pawn Pawn { get; set; }
    public ActionQueue Queue = new();
    public bool IsLoop { get; set; } = false;










    private void Awake()
    {
        Pawn = GetComponent<Pawn>();
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





    public Vector3 GetCurrentDestination()
    {
        return GetCurrentAction().Destination;
    }





    #nullable enable
    public Transform? GetCurrentTarget()
    {
        return GetCurrentAction()?.Target;
    }





    public Action? GetCurrentAction()
    {
        return Queue.FirstOrDefault();
    }
    #nullable disable





    public void ClearActionQueue()
    {
        if(!QueueIsEmpty())
        {
            CancelCurrentAction();
            Queue.Clear();
        }
    }





    public void CancelCurrentAction()
    {
        if (GetCurrentAction() != null)
        {
            if (OngoingCoroutine != null)
            {
                StopCoroutine(OngoingCoroutine);
            }

            NextAction();
            //Pawn.NavMeshAgent.ResetPath();
        }

    }





    public void ResetCurrentAction()
    {
        if (GetCurrentAction() != null)
        {
            if (OngoingCoroutine != null)
            {
                StopCoroutine(OngoingCoroutine);
            }

            GetCurrentAction().Unload();
        }
    }










    // BOOLEANS
    public bool QueueIsEmpty()
    {
        return !Queue.Any();
    }

    public bool CurrentActionIsPlaying()
    {
        return !QueueIsEmpty() && !GetCurrentAction().IsUnloaded();
    }
}
