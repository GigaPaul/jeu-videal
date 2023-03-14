using System.Collections.Generic;
using UnityEngine;

public class CultureManager : MonoBehaviour
{
    public const string path = "Database/cultures";
    public List<Culture> Cultures = new();

    void Awake()
    {
        CultureContainer container = CultureContainer.Load(path);
        Cultures.AddRange(container.Cultures);
    }
}
