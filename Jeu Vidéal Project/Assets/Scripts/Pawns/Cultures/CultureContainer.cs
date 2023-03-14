using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("CultureCollection")]
public class CultureContainer
{
    [XmlArray("Cultures")]
    [XmlArrayItem("Culture")]
    public List<Culture> Cultures = new();





    public static CultureContainer Load(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);

        XmlSerializer serializer = new XmlSerializer(typeof(CultureContainer));

        StringReader reader = new(xml.text);
        CultureContainer cultures = serializer.Deserialize(reader) as CultureContainer;
        reader.Close();

        return cultures;
    }
}
