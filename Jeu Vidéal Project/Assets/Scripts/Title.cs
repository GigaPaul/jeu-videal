using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title
{
    public string Content;
    public int Type;
    public bool IsPrefix => Type == 0;
    public bool IsInfix => Type == 1;
    public bool IsSuffix => Type == 2;

    public Title(string content, int type = 1)
    {
        Content = content;
        Type = type;
    }


}
