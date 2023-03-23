using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public delegate void PawnScript();

public class Action
{
    public string Label { get; set; }
    public Pawn Actor { get; set; }
    public bool IsOneShot { get; set; } = false;
    #nullable enable
    public Transform? Target { get; set; }
    public Vector3 Destination { get; set; }

    /// <summary>
    ///     Script executed when the actor reaches the destination.
    /// </summary>
    public Func<Task>? StartingScript { get; set; }

    /// <summary>
    ///     Script executed after StartingScript has been executed and if SuccessCondition has been achieved.
    /// </summary>
    public Func<Task>? SuccessScript { get; set; }

    /// <summary>
    /// Script executed before the end of the action even after a cancellation.
    /// </summary>
    public PawnScript? EndingScript { get; set; }

    public CancellationTokenSource TokenSource = new();



    public Func<bool>? SuccessCondition { get; set; }
#nullable disable
    public int Status { get; set; } = 0;










    public async Task PerformStartingScript()
    {
        if(StartingScript != null)
        {
            TokenSource = new();

            try
            {
                await StartingScript.Invoke();
            }
            catch(OperationCanceledException)
            {
                Debug.Log("The action has been canceled.");
            }
            finally
            {
                TokenSource.Dispose();
            }
        }
    }





    public async Task PerformSuccessScript()
    {
        if (SuccessScript != null)
        {
            await SuccessScript.Invoke();
        }
    }




    public float RemainingDistance()
    {

        float remainingDistance = Vector3.Distance(Destination, Actor.transform.position);
        return Mathf.Floor(remainingDistance * 10) / 10;
    }





    public void End()
    {
        if(EndingScript == null)
        {
            return;
        }

        EndingScript.Invoke();
    }





    public void Unload()
    {
        //TokenSource.Cancel();
        Status = 0;
    }





    public void Load()
    {
        Status = 1;
    }





    public void StartInitialization()
    {
        Status = 2;
    }





    public void CompleteInitialization()
    {
        Status = 3;
    }





    public void Start()
    {
        Status = 4;
    }





    public void Stop()
    {
        Status = 5;
    }










    // BOOLEANS
    public bool IsUnloaded()
    {
        return Status == 0;
    }





    public bool IsLoading()
    {
        return Status == 1;
    }





    public bool IsInitializing()
    {
        return Status == 2;
    }





    public bool IsInitialized()
    {
        return Status == 3;
    }





    public bool IsRunning()
    {
        return Status == 4;
    }





    public bool IsInactive()
    {
        return Status == 5;
    }





    public bool HasEnded()
    {
        if(SuccessCondition == null)
        {
            return true;
        }

        return SuccessCondition.Invoke();
    }
}
