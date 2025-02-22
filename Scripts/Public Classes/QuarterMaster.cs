using Godot;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;
public class Quartermaster
{
    public static void EquipHeroes(PopulationData populationData)
    {
        County county = (County)Globals.Instance.countiesParent.GetChild(populationData.location);
        CountyData countyData = county.countyData;
        List<GoodData> equipmentList = [];

        for (int i = 1; i <= populationData.equipment.Length; i++)
        {
            if (populationData.useNewestEquipment == false)
            {
                foreach (KeyValuePair<AllEnums.CountyGoodType, GoodData> keyValuePair in countyData.goods)
                {
                    if (keyValuePair.Value.equipmentData.equipmentType == (AllEnums.EquipmentType)i)
                    {
                        equipmentList.Add(keyValuePair.Value);
                    }
                }

                // We will want to move this up to the top.
                equipmentList = equipmentList.OrderBy(e => e.equipmentData.equipmentTier).ToList();
                populationData.equipment[i - 1] = equipmentList.FirstOrDefault();
                foreach (GoodData goodData in equipmentList)
                {
                    GD.Print("GoodData in Equipment List: " + goodData.goodName + i);
                }
                equipmentList.Clear();
            }
        }
    }
}