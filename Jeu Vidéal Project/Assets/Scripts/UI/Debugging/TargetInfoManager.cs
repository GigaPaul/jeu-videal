using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargetInfoManager : MonoBehaviour
{
    public TextMeshProUGUI ActivePawnName;
    public TextMeshProUGUI JobName;
    public TextMeshProUGUI TargetName;
    public TextMeshProUGUI ActionLabel;
    public TextMeshProUGUI FactionName;
    public TextMeshProUGUI SettlementName;





    private void FixedUpdate()
    {
        if(Globals.FocusedInteractive == null)
        {
            ActivePawnName.text = "";
            JobName.text = "";
            TargetName.text = "";
            ActionLabel.text = "";
            FactionName.text = "";
            SettlementName.text = "";
            return;
        }

        PawnAttributes attributes = Globals.FocusedInteractive.GetComponent<PawnAttributes>();
        ActivePawnName.text = attributes.GetFullName();


        string jobName = "";

        if(Globals.FocusedInteractive.GetComponent<SmartPawn>() != null)
        {
            jobName = Globals.FocusedInteractive.GetComponent<SmartPawn>().Label;
        }   

        JobName.text = jobName;


        string targetName = "";

        if(Globals.FocusedInteractive.IsPawn(out Pawn focusedPawn))
        {
            if(focusedPawn.StareTarget != null)
            {
                PawnAttributes targetAttributes = focusedPawn.StareTarget.GetComponentInParent<PawnAttributes>();
                targetName = targetAttributes.GetFullName();
            }

            TargetName.text = targetName;

            ActionLabel.text = focusedPawn._ActionManager.CurrentAction?.Label;
            FactionName.text = focusedPawn.Faction?.Label;
            SettlementName.text = focusedPawn.Settlement?.Label;
        }
    }
}
