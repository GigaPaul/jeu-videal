using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public class Culture
{
    [XmlAttribute("name")]
    public string Name;

    [XmlArray("FirstNames")]
    [XmlArrayItem("FirstName")]
    public List<string> FirstNames;

    [XmlArray("LastNames")]
    [XmlArrayItem("LastName")]
    public List<string> LastNames;

    [XmlElement("NameOrder")]
    public string NameOrder;





    public string GetFullNameOf(PawnAttributes pawn)
    {
        // If no name order was set, switch to default Firstname Lastname
        string order = NameOrder ?? "FirstName LastName";

        List<string> array = order.Split(" ").ToList();


        // Put the title at its place. Prefix : begining, Infix : between first and last name, Suffix : at the end
        if (pawn._Title != null)
        {
            int index = -1;

            if (pawn._Title.IsPrefix)
            {
                index = 0;
            }
            else if (pawn._Title.IsInfix)
            {
                index = 1;
            }
            else if (pawn._Title.IsSuffix)
            {
                index = array.Count();
            }

            // Only insert if the title is a prefix, infix or suffix
            if(index >= 0)
            {
                array.Insert(index, "Title");
            }
        }


        // Replace corresponding names and titles
        string result = "";
        for(int i = 0; i < array.Count(); i++)
        {
            string str = array[i];

            // Add a space before the substring if the substring isn't the first one
            if(i > 0)
            {
                result += " ";
            }

            switch (str)
            {
                case "FirstName":
                    result += pawn.FirstName;
                    break;
                case "LastName":
                    result += pawn.LastName;
                    break;
                case "Title":
                    result += pawn._Title.Content;
                    break;
            }
        }



        return result;
    }
}
