using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionMenu : MonoBehaviour
{
    public List<string> actions { get; set; }
    public GameObject actionPrefab;
    public RectTransform canvas;
    public RectTransform rt { get; set; }
    #nullable enable
    public Vector3? target { get; set; }
    #nullable disable
    // Start is called before the first frame update
    void Start()
    {
        actions = new();
        rt = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if(target != null)
        {
            Vector2 position = Camera.main.WorldToScreenPoint((Vector3)target);


            if(position.x < rt.rect.width / 2)
                position.x = rt.rect.width / 2;
            else if(position.x + rt.rect.width / 2 > canvas.rect.width)
                position.x = canvas.rect.width - rt.rect.width / 2;


            if(position.y < rt.rect.height / 2)
                position.y = rt.rect.height / 2;
            else if (position.y + rt.rect.height / 2 > canvas.rect.height)
                position.y = canvas.rect.height - rt.rect.height / 2;



            transform.position = position;
        }
    }

    //public void GetActionsFrom(InteractiveObject interactive)
    //{
    //    ResetActions();
    //    target = interactive.rb.worldCenterOfMass;

    //    if (interactive.types.Count > 0)
    //    {
    //        actions = interactive.types;
    //    }

    //    Generate();
    //}

    //void Generate()
    //{
    //    foreach (string action in actions)
    //    {
    //        GameObject actionButton = Instantiate(actionPrefab, transform);
    //        actionButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = action;
    //    }
    //}

    //public void ResetActions()
    //{
    //    foreach (Transform child in transform)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    target = null;
    //}
}
