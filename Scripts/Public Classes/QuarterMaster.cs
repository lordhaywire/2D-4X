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

            foreach (KeyValuePair<AllEnums.CountyGoodType, GoodData> keyValuePair in countyData.goods)
            {
                if (keyValuePair.Value.equipmentData?.equipmentType == (AllEnums.EquipmentType)i)
                {
                    equipmentList.Add(keyValuePair.Value);
                }
            }

            // Sorts the list by ascending order.
            equipmentList = equipmentList.OrderBy(e => e.equipmentData.equipmentTier).ToList();

            // If the newest equipment is set to false, then it picks the lowest level of equipment,
            // otherwise it picks the highest level of equipment.
            if (populationData.useNewestEquipment == false)
            {
                populationData.equipment[i - 1] = equipmentList.FirstOrDefault();
            }
            else
            {
                populationData.equipment[i - 1] = equipmentList.LastOrDefault();
            }

            foreach (GoodData goodData in equipmentList)
            {
                GD.Print("GoodData in Equipment List: " + goodData.goodName + i);
            }
            equipmentList.Clear();
        }
    }
}