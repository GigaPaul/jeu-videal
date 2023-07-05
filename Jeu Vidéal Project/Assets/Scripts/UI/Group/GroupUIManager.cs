using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupUIManager : MonoBehaviour
{
    public List<Pawn> ControlledPawns = new();
    public GameObject ButtonPrefab;



    // Start is called before the first frame update
    void Start()
    {
        CheckControlledPawns();
    }



    private void FixedUpdate()
    {
        CheckControlledPawns();
    }





    public void CheckControlledPawns()
    {
        List<Pawn> playablePawns = FindObjectsOfType<Pawn>().Where(i => i.IsPlayable()).ToList();

        foreach(Pawn pawn in playablePawns)
        {
            AddControlledPawn(pawn);
        }
    }





    public void AddControlledPawn(Pawn pawn)
    {
        if(ControlledPawns.Contains(pawn))
        {
            return;
        }

        ControlledPawns.Add(pawn);



        GameObject newButtonGO = Instantiate(ButtonPrefab);
        GroupMemberButton newButton = newButtonGO.GetComponent<GroupMemberButton>();
        newButton.Master = pawn;

        newButtonGO.transform.SetParent(transform, false);
    }





    public void RemoveControlledPawn(Pawn pawn)
    {
        ControlledPawns.Remove(pawn);

        GroupMemberButton toDelete = GetComponentsInChildren<GroupMemberButton>().FirstOrDefault(i => i.Master == pawn);

        if(toDelete == null)
        {
            return;
        }

        Destroy(toDelete);
    }
}
