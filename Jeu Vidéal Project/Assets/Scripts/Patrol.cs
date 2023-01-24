using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Patrol
{
    public List<Vector3> Waypoints = new ();
    public bool IsLoop { get; set; } = false;

    public bool IsEmpty()
    {
        return Waypoints.Count == 0;
    }


    public void Next()
    {
        Vector3 currentWaypoint = Waypoints.First();
        Waypoints.Remove(currentWaypoint);

        if(IsLoop)
        {
            Waypoints.Insert(Waypoints.Count(), currentWaypoint);
        }
    }



    [Command(requiresAuthority = false)]
    public void Add(Vector3 position)
    {
        RpcHandleAddToPatrol(position);
    }

    [Command(requiresAuthority = false)]
    public void Clear()
    {
        RpcHandleEmptyPatrol();
    }



    [ClientRpc]
    private void RpcHandleAddToPatrol(Vector3 position)
    {
        Waypoints.Add(position);
    }



    [ClientRpc]
    private void RpcHandleEmptyPatrol()
    {
        Waypoints.Clear();
    }
}
