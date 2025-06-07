using Godot;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;
public class Quartermaster
{
    public static void EquipHeroesAndSubordinates(PopulationData populationData)
    {
        EquipPopulation(populationData, populationData.useNewestEquipment); 
        GD.Print($"{populationData.firstName} has been equipped.");
        
        foreach (PopulationData subordinate in populationData.heroSubordinates)
        {
            if (subordinate.activity != AllEnums.Activities.Service) continue;
            EquipPopulation(subordinate, populationData.useNewestEquipment);
            GD.Print($"{subordinate.firstName} has been equipped.");
        }
    }
    
    /// <summary>
    /// I didn't take into account if the heroes are no in a friendly county.
    /// </summary>
    /// <param name="populationData"></param>
    /// <param name="useNewestEquipment"></param>
    private static void EquipPopulation(PopulationData populationData, bool useNewestEquipment)
    {
        County county = (County)Globals.Instance.countiesParent.GetChild(populationData.location);
        CountyData countyData = county.countyData;

        for (int i = 1; i <= populationData.equipment.Length; i++)
        {
            List<GoodData> sortedEquipmentList = GenerateEquipmentList(countyData, i);
            // If the useNewestEquipment is set to false, then it picks the lowest level of equipment,
            // otherwise it picks the highest level of equipment.
            GoodData goodToEquip = useNewestEquipment
                ? sortedEquipmentList.LastOrDefault()
                : sortedEquipmentList.FirstOrDefault(g => g.Amount >= 1);
            if (goodToEquip != null)
            {
                //GD.Print("Good to equip: " + goodToEquip.goodName);
                CheckForAlreadyEquippedAndAdjustCountyGoods(countyData, goodToEquip, populationData);
                
            }
            else
            {
                //GD.Print("Good to equip is null.");
            }

            // This is just for testing.
            /*
            foreach (GoodData goodData in equipmentList)
            {
                GD.Print("GoodData in Equipment List: " + goodData.goodName + i);
            }
            */
            sortedEquipmentList.Clear();
        }
    }


    private static void CheckForAlreadyEquippedAndAdjustCountyGoods(CountyData countyData, GoodData goodToEquip, PopulationData populationData)
    {
        // If there is no equipment equipped, then take one from the county goods and equip it.
        if (populationData.equipment[AllEnums.GetCorrectEquipmentSlot(goodToEquip.equipmentData.equipmentType)] == null)
        {
            populationData.equipment[AllEnums.GetCorrectEquipmentSlot(goodToEquip.equipmentData.equipmentType)] 
                = goodToEquip;
            // Take 1 good away from the County goods.
            Haulmaster.AdjustCountyGoodAmount(countyData, goodToEquip.countyGoodType, -1);
        }
        else
        {
            // If the goodData is already equipped, then don't do anything.
            if (populationData.equipment[AllEnums.GetCorrectEquipmentSlot(goodToEquip.equipmentData.equipmentType)].goodName 
                == goodToEquip.goodName)
            {
                GD.Print($"{goodToEquip.goodName} is already equipped.");
            }
            else
            {
                // In all other cases, put the populations equipped equipment back, then take the new equipment away from
                // the county goods.
                // Put back the original equipped equipment.
                Haulmaster.AdjustCountyGoodAmount(countyData, 
                    populationData.equipment[AllEnums.GetCorrectEquipmentSlot(goodToEquip.equipmentData.equipmentType)].countyGoodType
                    , 1);
                // Get the equipment from the county goods.
                Haulmaster.AdjustCountyGoodAmount(countyData, goodToEquip.countyGoodType, -1);
                // Equip the new goodToEquip on the populationData.
                populationData.equipment[AllEnums.GetCorrectEquipmentSlot(goodToEquip.equipmentData.equipmentType)] 
                    = goodToEquip;
                //GD.Print($"{goodToEquip.goodName} is different from currently equipped.");
            }
        }
    }

    private static List<GoodData> GenerateEquipmentList(CountyData countyData, int i)
    {
        List<GoodData> equipmentList = [];

        foreach (KeyValuePair<AllEnums.CountyGoodType, GoodData> keyValuePair in countyData.goods)
        {
            if (keyValuePair.Value.equipmentData?.equipmentType == (AllEnums.EquipmentType)i
                && keyValuePair.Value.Amount > 0)
            {
                equipmentList.Add(keyValuePair.Value);
            }
        }

        // Sorts the list by ascending order.
        return [.. equipmentList.OrderBy(e => e.equipmentData.equipmentTier)];
    }
    
    public static int GetEquipmentBonus(PopulationData populationData, AllEnums.EquipmentType equipmentType)
    {
        int equipmentBonus = 0;
        if (populationData.equipment[AllEnums.GetCorrectEquipmentSlot(equipmentType)] != null)
        {
            equipmentBonus = populationData.equipment[AllEnums.GetCorrectEquipmentSlot(equipmentType)].equipmentData.equipmentBonus;
        }

        return equipmentBonus;
    }
}