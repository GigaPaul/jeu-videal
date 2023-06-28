using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicAction", menuName = "Action/BasicAction")]
public class Action : ScriptableObject
{
    public string Label;
    public string Id;
    public bool IsOneShot = false;
    public bool IsWaypoint = false;
    /// <summary>
    /// Time in ms the Pawn will wait after reaching the destination of the action
    /// </summary>
    public int StartWaitingTime;
    /// <summary>
    /// Time in ms the Pawn will wait after the action succeeds
    /// </summary>
    public int EndWaitingTime;

    public enum StatusType
    {
        unloaded,
        loading,
        initializing,
        initialized,
        running,
        inactive
    }
    public StatusType Status { get; set; } = StatusType.unloaded;


    public Pawn Actor { get; set; }
    #nullable enable
    public Transform? Target { get; set; }
    #nullable disable
    public Vector3 Destination { get; set; }

    public virtual Task OnStart()
    { 
        if(StartWaitingTime == 0)
        {
            return Task.FromResult(0);
        }
        else
        {
            //await Task.Delay(StartWaitingTime);
            return Task.FromResult(0);
        }
    }

    public virtual Task OnSuccess() {
        if (EndWaitingTime == 0)
        {
            return Task.FromResult(0);
        }
        else
        {
            //await Task.Delay(EndWaitingTime);
            return Task.FromResult(0);
        }
    }

    public virtual Task OnEnd() { return Task.FromResult(0); }

    public float RemainingDistance()
    {

        float remainingDistance = Vector3.Distance(Destination, Actor.transform.position);
        return Mathf.Floor(remainingDistance * 10) / 10;
    }


    #nullable enable
    public static Action? Find(string id)
    {
        return Instantiate(Resources.Load<Action>($"Actions/{id}"));
    }
    #nullable disable


    public virtual bool IsValid()
    {
        return true;
    }

    public virtual bool HasSucceeded()
    {
        return true;
    }
}
