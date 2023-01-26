using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatBubble : MonoBehaviour
{
    public string Text { get; set; } = "";
    public float Duration { get; set; } = 10;
    public Transform Origin { get; set; }


    private void Awake()
    {
        StartCoroutine(Appear());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        Vector3 Position = Camera.main.WorldToScreenPoint(Origin.position);
        Rect Canvas = GameObject.Find("Canvas").GetComponent<RectTransform>().rect;
        Rect Rect = GetComponent<RectTransform>().rect;

        float widthCenter = Rect.width / 2;
        float heightCenter = Rect.height / 2;

        float left = Position.x - widthCenter;
        float right = Position.x + widthCenter;
        float bottom = Position.y - heightCenter;
        float top = Position.y + heightCenter;

        if (left < 0)
            Position.x = widthCenter;
        else if (right > Canvas.width)
            Position.x = Canvas.width - widthCenter;

        if (bottom < 0)
            Position.y = heightCenter;
        else if(top > Canvas.height)
            Position.y = Canvas.height - heightCenter;
            

        transform.position = Position;
    }

    private IEnumerator Appear()
    {
        yield return new WaitForSeconds(Duration);
        Destroy(gameObject);
    }
}
