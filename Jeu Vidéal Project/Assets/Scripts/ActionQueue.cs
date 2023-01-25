using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionQueue
{
    public List<Action> Actions = new();
    public bool IsLoop { get; set; } = false;

    public bool IsEmpty()
    {
        return Actions.Count == 0;
    }


    //public void Perform()
    //{
    //    yield return new WaitForSeconds(Actions.First().CastingTime);
    //}


    public void Next()
    {
        Action currentAction = Actions.First();
        Actions.Remove(currentAction);

        if (IsLoop)
        {
            Actions.Insert(Actions.Count(), currentAction);
        }
    }



    [Command(requiresAuthority = false)]
    public void Add(Action action)
    {
        RpcHandleAddToQueue(action);
    }

    [Command(requiresAuthority = false)]
    public void Clear()
    {
        RpcHandleEmptyQueue();
    }



    [ClientRpc]
    private void RpcHandleAddToQueue(Action action)
    {
        Actions.Add(action);
    }



    [ClientRpc]
    private void RpcHandleEmptyQueue()
    {
        Actions.Clear();
    }
}
