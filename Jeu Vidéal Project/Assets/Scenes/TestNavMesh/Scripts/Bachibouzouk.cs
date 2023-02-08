    using UnityEngine;
using UnityEngine.AI;

public class Bachibouzouk : MonoBehaviour
{
    public Camera Camera;
    public NavMeshAgent NavMeshAgent;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log($"Going to {hit.point}");
                NavMeshAgent.SetDestination(hit.point);
            }
        }
    }
}