using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public class SettlementManager : MonoBehaviour
{
    public List<Settlement> Settlements = new();
    private const string path = "Database/settlements";

    // Start is called before the first frame update
    void Start()
    {
        LoadDataFromDb();
    }




    private void LoadDataFromDb()
    {
        SettlementContainer container = SettlementContainer.Load(path);
        List<Settlement> settlements = FindObjectOfType<SettlementManager>().Settlements;
        List<Culture> cultures = FindObjectOfType<CultureManager>().Cultures;

        foreach (SettlementData data in container.Data)
        {
            if (settlements.ElementAtOrDefault(data.Id) == null)
            {
                continue;
            }

            if (cultures.ElementAtOrDefault(data.IdCulture) == null)
            {
                continue;
            }

            Settlement settlement = settlements[data.Id];
            Culture culture = cultures[data.IdCulture];
            settlement._Culture = culture;


            for (int i = 0; i <= data.InitialPopulation; i++)
            {
                settlement.CreateVillager();
            }
        }
    }
}
