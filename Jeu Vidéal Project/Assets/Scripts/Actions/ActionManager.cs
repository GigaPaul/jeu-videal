using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public Coroutine OngoingCoroutine { get; set; }
    private Pawn Master { get; set; }
    public ActionQueue Queue = new();
    public bool IsLoop { get; set; } = false;

    #nullable enable
    public Action? CurrentAction
    {
        get { return Queue.FirstOrDefault(); }
        set
        {
            ClearActionQueue();
            Queue.Add(value); 
        }
    }
    #nullable disable










    private void Awake()
    {
        Master = GetComponent<Pawn>();
    }





    private void FixedUpdate()
    {
        CheckActionCancellation();

        // There are no actions to play
        if (QueueIsEmpty())
        {
            return;
        }

        if(CurrentAction.Status == Action.StatusType.inactive)
        {
            return;
        }


        // If the action is unloaded or loading but isn't valid, cancel it
        bool isUnloadedOrLoading = CurrentAction.Status == Action.StatusType.unloaded || CurrentAction.Status == Action.StatusType.loading;

        if (isUnloadedOrLoading && !CurrentAction.IsValid())
        {
            CancelCurrentAction();
            return;
        }



        switch(CurrentAction.Status)
        {
            case Action.StatusType.loading:
                OngoingCoroutine = StartCoroutine(InitializeAction());
                break;

            case Action.StatusType.initialized:
                CurrentAction.OnStart();
                CurrentAction.Status = Action.StatusType.running;
                break;

            case Action.StatusType.running:
                if (CurrentAction.HasSucceeded())
                {
                    OngoingCoroutine = StartCoroutine(StopAction());
                }
                break;
        }


    }





    //private void OldFixedUpdate()
    //{
    //    CheckActionCancellation();

    //    if(!QueueIsEmpty() && !CurrentAction.IsInactive())
    //    {
    //        if(CurrentAction.IsUnloaded() || CurrentAction.IsLoading())
    //        {
    //            if(!CurrentAction.AreConditionsValid())
    //            {
    //                CancelCurrentAction();
    //                return;
    //            }
    //        }
            
            
    //        if (CurrentAction.IsLoading())
    //        {
    //            OngoingCoroutine = StartCoroutine(InitializeAction());
    //        }
    //        else if(CurrentAction.IsInitialized())
    //        {
    //            StartAction();
    //        }
    //        else if(CurrentAction.IsRunning() && CurrentAction.HasEnded())
    //        {
    //            OngoingCoroutine = StartCoroutine(StopAction());
    //        }
    //    }
    //}





    private void CheckActionCancellation()
    {
        if(QueueIsEmpty())
        {
            return;
        }

        bool hasReachedDestination = CurrentAction.RemainingDistance() <= Master.NavMeshAgent.stoppingDistance;

        if (CurrentAction.Status != Action.StatusType.unloaded && !hasReachedDestination)
        {
            CurrentAction.Status = Action.StatusType.unloaded;
        }
    }










    private IEnumerator InitializeAction()
    {

        CurrentAction.Status = Action.StatusType.initializing;
        Task task = CurrentAction.OnStart();
        yield return new WaitUntil(() => task.IsCompleted);
        CurrentAction.Status = Action.StatusType.initialized;

        //Debug.Log(Pawn.gameObject.name + " : " + CurrentAction?.Label);




        //CurrentAction.StartInitialization();
        //Task task = CurrentAction.PerformStartingScript(); 
        //yield return new WaitUntil(() => task.IsCompleted);

        //Debug.Log(Pawn.gameObject.name + " : " + CurrentAction?.Label);
        //CurrentAction.CompleteInitialization();
    }





    //private void StartAction()
    //{
    //    CurrentAction.Start();
    //}





    private IEnumerator StopAction()
    {
        CurrentAction.Status = Action.StatusType.inactive;
        Task task = CurrentAction.OnSuccess();
        yield return new WaitUntil(() => task.IsCompleted);
        CurrentAction.OnEnd();
        NextAction();


        //CurrentAction.Stop();
        //Task task = CurrentAction.PerformSuccessScript();
        //yield return new WaitUntil(() => task.IsCompleted);
        //CurrentAction.End();
        //NextAction();
    }





    public void NextAction()
    {
        Action currentAction = CurrentAction;
        Queue.Remove(currentAction);

        if (IsLoop && !currentAction.IsOneShot)
        {
            currentAction.Status = Action.StatusType.unloaded;
            Queue.Insert(Queue.Count(), currentAction);
        }
    }





    public Vector3 GetCurrentDestination()
    {
        return CurrentAction.Destination;
    }





    #nullable enable
    public Transform? GetCurrentTarget()
    {
        return CurrentAction?.Target;
    }





    //public NewAction? CurrentAction
    //{
    //    return Queue.FirstOrDefault();
    //}
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
        if(CurrentAction == null)
        {
            return;
        }

        if (OngoingCoroutine != null)
        {
            StopCoroutine(OngoingCoroutine);
        }

        CurrentAction.OnEnd();

        NextAction();
    }





    public void ResetCurrentAction()
    {
        if (CurrentAction == null)
        {
            return;
        }

        if (OngoingCoroutine != null)
        {
            StopCoroutine(OngoingCoroutine);
        }

        CurrentAction.OnEnd();

        CurrentAction.Status = Action.StatusType.unloaded;
    }










    // BOOLEANS
    public bool QueueIsEmpty()
    {
        return !Queue.Any();
    }

    public bool CurrentActionIsPlaying()
    {
        return !QueueIsEmpty() && CurrentAction.Status != Action.StatusType.unloaded;
    }
}
