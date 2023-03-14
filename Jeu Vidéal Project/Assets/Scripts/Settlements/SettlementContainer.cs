using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("SettlementCollection")]
public class SettlementContainer
{
    [XmlArray("Settlements")]
    [XmlArrayItem("Settlement")]
    public List<SettlementData> Data = new();





    public static SettlementContainer Load(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);

        XmlSerializer serializer = new XmlSerializer(typeof(SettlementContainer));

        StringReader reader = new(xml.text);

        SettlementContainer settlements = serializer.Deserialize(reader) as SettlementContainer;
        reader.Close();

        return settlements;
    }
}
