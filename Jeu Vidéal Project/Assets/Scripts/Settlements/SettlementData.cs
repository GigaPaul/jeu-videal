using System.Xml.Serialization;

public class SettlementData
{
    [XmlAttribute("name")]
    public string Name;

    [XmlAttribute("id")]
    public int Id;

    [XmlElement("Culture")]
    public int IdCulture;

    [XmlElement("Population")]
    public int InitialPopulation;
}
