using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameLog : MonoBehaviour
{
    private float LifeTime;
    public float LifeSpan = 3;
    public float AlphaSpeed = 2;
    public TextMeshProUGUI TextMesh { get; set; }

    private void Awake()
    {
        TextMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        LifeTime += Time.deltaTime;

        if(LifeTime >= LifeSpan)
        {
            Color color = TextMesh.color;
            color.a -= Time.deltaTime * AlphaSpeed;
            TextMesh.color = color;

            // When the text is transparent, destroy the message
            if(TextMesh.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
