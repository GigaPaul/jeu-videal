using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachment : MonoBehaviour
{
    #nullable enable
    public Transform? Item { get; private set; }
    #nullable disable

    public void Drop()
    {
        if(Item == null)
        {
            return;
        }

        Destroy(Item.gameObject);
    }

    public void Attach(Transform item)
    {
        Drop();
        Item = item;
        Item.SetPositionAndRotation(transform.position, transform.rotation);
        Item.SetParent(transform);
    }
}
