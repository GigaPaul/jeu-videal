using UnityEngine;

[RequireComponent(typeof(Pawn))]
public abstract class SmartPawn : MonoBehaviour
{
    public Pawn _Pawn;
    public virtual string Label
    {
        get { return "Wanderer"; }
    }





    // Start is called before the first frame update
    void Start()
    {
        _Pawn = GetComponent<Pawn>();
        InvokeRepeating(nameof(RoutineController), 0, 1);
    }





    private void RoutineController()
    {
        // If the pawn is dead
        if (!_Pawn.IsAlive)
        {
            return;
        }

        // If the pawn is moving
        if (_Pawn.IsMoving())
        {
            return;
        }

        // If the pawn has tasks to do
        if (!_Pawn.ActionManager.QueueIsEmpty())
        {
            return;
        }

        // If the spawn is playable
        if (_Pawn.IsPlayable())
        {
            return;
        }

        // Start AI routine
        Routine();
    }





    protected abstract void Routine();
}
