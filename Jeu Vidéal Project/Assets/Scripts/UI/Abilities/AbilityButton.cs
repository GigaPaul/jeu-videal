using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    public string ShortCut { get; set; }
    public Ability _Ability { get; set; }
    public Sprite DefaultSprite;
    public RectTransform CoolDownRect;


    private void Update()
    {
        
    }


    public void Load(Ability ability)
    {
        _Ability = ability;
        //GetComponentInChildren<TextMeshProUGUI>().text = ability.Id;

        //if(ability.SpriteName == "")
        //{
        //    return;
        //}

        //Sprite sprite = Resources.Load<Sprite>("Images/" + ability.SpriteName);
        Image image = GetComponent<Image>();


        if (ability._Sprite != null)
        {
            image.sprite = ability._Sprite;
        }
        else
        {
            image.sprite = Resources.Load<Sprite>("Images/unknown");
        }
    }


    public void Unload()
    {
        _Ability = null;
        //GetComponentInChildren<TextMeshProUGUI>().text = "Empty";
        Image image = GetComponent<Image>();
        image.sprite = DefaultSprite;
    }


    public void Cast()
    {
        if(_Ability == null)
        {
            return;
        }

        if(Globals.FocusedPawn == null)
        {
            return;
        }

        Globals.FocusedPawn.Cast(_Ability);
    }
}
