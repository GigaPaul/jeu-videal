using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameLogger : MonoBehaviour
{
    public static GameLogger Get()
    {
        return FindObjectOfType<GameLogger>();
    }

    public static void Log(string text, Color textColor)
    {
        // Just in case I forget the punctuation, for consistency
        if(!text.EndsWith(".") && !text.EndsWith("!") && !text.EndsWith("?"))
        {
            text += ".";
        }

        GameLogger gameLogger = Get();

        GameLog logObject = Instantiate(Resources.Load<GameLog>("Prefabs/UI/Log"));
        TextMeshProUGUI textMesh = logObject.GetComponent<TextMeshProUGUI>();
        textMesh.text = text;
        textMesh.color = textColor;

        logObject.transform.SetParent(gameLogger.transform);
    }


    public static void LogError(string text)
    {
        Log(text, Color.red);
    }
}
