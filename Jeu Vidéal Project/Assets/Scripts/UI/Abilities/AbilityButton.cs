using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    public string ShortCut { get; set; }
    public AbilityHolder Holder { get; set; }
    public Sprite DefaultSprite;
    public RectTransform CoolDownRect;





    public void Load(AbilityHolder holder)
    {
        Holder = holder;
        Image image = GetComponent<Image>();

        if (holder.AbilityHeld._Sprite != null)
        {
            image.sprite = holder.AbilityHeld._Sprite;
        }
        else
        {
            image.sprite = Resources.Load<Sprite>("Images/unknown");
        }
    }





    public void Unload()
    {
        Holder = null;
        Image image = GetComponent<Image>();
        image.sprite = DefaultSprite;
        CoolDownRect.sizeDelta = new(CoolDownRect.rect.width, 0);
    }





    public void Cast()
    {
        if (Globals.FocusedInteractive == null)
        {
            return;
        }

        if(Holder == null)
        {
            return;
        }

        if(Globals.FocusedInteractive.IsPawn(out Pawn focusedPawn))
        {
            focusedPawn.Cast(Holder);
        }
    }





    public void UpdateCooldown()
    {
        if (Holder == null)
        {
            return;
        }

        float width = CoolDownRect.rect.width;
        float height = 0f;

        if (Holder.IsCoolingDown && Holder.AbilityHeld.HasCooldown())
        {
            float coolDownProgression = 1 - (float)Math.Floor(Holder.CoolDown / Holder.AbilityHeld.CoolDownTime * 100) / 100;
            float maxHeight = GetComponent<RectTransform>().rect.height;

            height = maxHeight * coolDownProgression;
        }

        CoolDownRect.sizeDelta = new(width, height);
    }
}
