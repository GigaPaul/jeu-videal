using Mirror;
using System.Collections.Generic;

public class ActionQueue : List<Action>
{



    [Command(requiresAuthority = false)]
    new public void Add(Action action)
    {
        RpcHandleAdd(action);
    }





    [ClientRpc]
    private void RpcHandleAdd(Action action)
    {
        base.Add(action);
    }





    [Command(requiresAuthority = false)]
    new public void Clear()
    {
        RpcHandleClear();
    }





    [ClientRpc]
    private void RpcHandleClear()
    {
        base.Clear();
    }
}
