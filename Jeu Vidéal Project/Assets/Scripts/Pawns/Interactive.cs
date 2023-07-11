using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour
{
    [Header("Interactive")]
    public string Name;
    public bool Focusable = false;
    public float RingRadius;
    public StatusRings Rings { get; set; }



    protected virtual void Awake()
    {
        InstantiateStatusRings();
    }





    private void InstantiateStatusRings()
    {
        GameObject ringsPrefab = Resources.Load("Prefabs/StatusRings") as GameObject;
        StatusRings rings = ringsPrefab.GetComponent<StatusRings>();

        Rings = Instantiate(rings, transform);

        if (RingRadius != 0f)
        {
            float yHover = Rings.Hover.transform.localScale.y;
            Rings.Hover.transform.localScale = new(RingRadius, yHover, RingRadius);

            float yFocus = Rings.Focus.transform.localScale.y;
            Rings.Focus.transform.localScale = new(RingRadius, yFocus, RingRadius);
        }
    }





    public virtual void Focus()
    {
        Globals.FocusedInteractive = this;
        Rings.Focus.gameObject.SetActive(true);
    }



    public virtual void Unfocus()
    {
        Globals.FocusedInteractive = null;
        Rings.Focus.gameObject.SetActive(false);
    }


    public virtual void HoverIn()
    {
        if(Globals.HoveredInteractive != null)
        {
            Globals.HoveredInteractive.HoverOut();
        }

        Globals.HoveredInteractive = this;
        Rings.Hover.gameObject.SetActive(true);
    }


    public virtual void HoverOut()
    {
        Globals.HoveredInteractive = this;
        Rings.Hover.gameObject.SetActive(false);
    }





    public bool IsFocused()
    {
        return Globals.FocusedInteractive == this;
    }


    public bool IsHovered()
    {
        return Globals.HoveredInteractive == this;
    }


    public bool IsPawn()
    {
        return GetComponent<Pawn>() != null;
    }


    public bool IsPawn(out Pawn result)
    {
        result = GetComponent<Pawn>();

        return result != null;
    }
}
