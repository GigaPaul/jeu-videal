using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Action
{
    public string Label { get; set; }
    public float CastingTime { get; set; } = 3;
    public Pawn Actor { get; set; }
    public bool IsOneShot { get; set; } = false;
    #nullable enable
    public Transform? Target { get; set; }


    public Func<Task>? StartingScript { get; set; }
    public Func<Task>? SuccessScript { get; set; }



    public Func<bool>? SuccessCondition { get; set; }
#nullable disable
    public int Status { get; set; } = 0;










    public async Task PerformStartingScript()
    {
        if(StartingScript != null)
        {
            await StartingScript.Invoke();
        }
    }





    public async Task PerformSuccessScript()
    {
        if (SuccessScript != null)
        {
            await SuccessScript.Invoke();
        }
    }





    public void Unload()
    {
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
        bool result = true;

        if(SuccessCondition != null)
        {
            result = SuccessCondition.Invoke();
        }

        return result;
    }
}
