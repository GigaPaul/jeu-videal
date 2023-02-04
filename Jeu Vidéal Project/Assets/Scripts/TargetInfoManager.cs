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
        ActivePawnName.text = Globals.FocusedPawn?.gameObject.name;
        TargetName.text = Globals.FocusedPawn?.StareTarget?.GetComponentInParent<Pawn>().gameObject.name;
        ActionLabel.text = Globals.FocusedPawn?.ActionManager.GetCurrentAction()?.Label;
        FactionName.text = Globals.FocusedPawn?.Faction?.Label;
        SettlementName.text = Globals.FocusedPawn?.Settlement?.Label;
    }
}
