using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("DateTime")]
    public TextMeshProUGUI Date;
    public TextMeshProUGUI Time;
    public TimeManager _TimeManager { get; set; }


    private void Start()
    {
        _TimeManager = FindObjectOfType<TimeManager>();
    }



    private void FixedUpdate()
    {
        Date.text = _TimeManager.CurrentDate.ToString("dd/MM/yyyy");
        Time.text = _TimeManager.CurrentDate.ToString("HH:mm");
    }
}
