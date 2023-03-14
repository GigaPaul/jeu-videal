using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargetInfoManager : MonoBehaviour
{
    public TextMeshProUGUI ActivePawnName;
    public TextMeshProUGUI TargetName;
    public TextMeshProUGUI ActionLabel;
    public TextMeshProUGUI FactionName;
    public TextMeshProUGUI SettlementName;

    private void Awake()
    {
        //Clear();
    }

    private void FixedUpdate()
    {
        if(Globals.FocusedPawn == null)
        {
            ActivePawnName.text = "";
            TargetName.text = "";
            ActionLabel.text = "";
            FactionName.text = "";
            SettlementName.text = "";
            return;
        }

        PawnAttributes attributes = Globals.FocusedPawn.GetComponent<PawnAttributes>();
        ActivePawnName.text = attributes.GetFullName();

        string targetName = "";

        if(Globals.FocusedPawn.StareTarget != null)
        {
            PawnAttributes targetAttributes = Globals.FocusedPawn.StareTarget.GetComponentInParent<PawnAttributes>();
            targetName = targetAttributes.GetFullName();
        }

        TargetName.text = targetName;

        ActionLabel.text = Globals.FocusedPawn?.ActionManager.GetCurrentAction()?.Label;
        FactionName.text = Globals.FocusedPawn?.Faction?.Label;
        SettlementName.text = Globals.FocusedPawn?.Settlement?.Label;
    }
}
