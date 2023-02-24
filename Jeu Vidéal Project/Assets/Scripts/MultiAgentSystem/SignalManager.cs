using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SignalManager : MonoBehaviour
{
    private Pawn Pawn { get; set; }
    private List<Pawn> RecentlyCheckedPawns { get; set; } = new();


    private void Awake()
    {
        Pawn = GetComponent<Pawn>();
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TargetCoroutine());
    }





    IEnumerator TargetCoroutine()
    {
        while (Pawn.IsAlive)
        {
            Task task = WatchForTarget();
            yield return new WaitUntil(() => task.IsCompleted);
        }
    }





    async Task WatchForTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Pawn.ViewRange);

        Pawn NearestPawnFound = null;
        float SmallestAngle = Mathf.Infinity;

        foreach (var collider in hitColliders)
        {
            Pawn HitPawn = collider.gameObject.GetComponentInParent<Pawn>();
            if (HitPawn != null && HitPawn != this && !RecentlyCheckedPawns.Contains(HitPawn))
            {
                if (Pawn.HasInSights(HitPawn.transform))
                {
                    Vector3 DirectionToTarget = (HitPawn.transform.position - transform.position).normalized;
                    float ThisAngle = Vector3.Angle(transform.forward, DirectionToTarget);

                    if (ThisAngle < SmallestAngle)
                    {
                        NearestPawnFound = HitPawn;
                        SmallestAngle = ThisAngle;
                    }
                }
            }
        }

        if (NearestPawnFound == null && RecentlyCheckedPawns.Count > 0)
            RecentlyCheckedPawns.Clear();

        int minDelay = 1000;
        int delay = minDelay;
        #nullable enable
        Transform? TargetToStare = null;
        #nullable disable

        if(NearestPawnFound != null)
        {
            TargetToStare = NearestPawnFound.FocusElement;
            delay = Random.Range(minDelay, minDelay + 1000);
            RecentlyCheckedPawns.Add(NearestPawnFound);
        }

        Pawn.LookAt(TargetToStare);
        await Task.Delay(delay);
    }
}
