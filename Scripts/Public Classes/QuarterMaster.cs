using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;
public class Quartermaster
{
    /// <summary>
    /// I didn't take into account if the heroes are no in a friendly county.
    /// </summary>
    /// <param name="populationData"></param>
    public static void EquipHeroes(PopulationData populationData)
    {
        County county = (County)Globals.Instance.countiesParent.GetChild(populationData.location);
        CountyData countyData = county.countyData;
        List<GoodData> equipmentList;

        for (int i = 1; i <= populationData.equipment.Length; i++)
        {
            equipmentList = GenerateEquipmentList(countyData, i);
            // If the useNewestEquipment is set to false, then it picks the lowest level of equipment,
            // otherwise it picks the highest level of equipment.
            GoodData goodToEquip = populationData.useNewestEquipment
                ? equipmentList.LastOrDefault()
                : equipmentList.FirstOrDefault();
            if (goodToEquip != null)
            {
                //GD.Print("Good to equip: " + goodToEquip.goodName);
                CheckForAlreadyEquipedAndAdjustCountyGoods(countyData, goodToEquip, populationData);
            }
            else
            {
                //GD.Print("Good to equip is null.");
            }

            // This is just for testing.
            foreach (GoodData goodData in equipmentList)
            {
                //GD.Print("GoodData in Equipment List: " + goodData.goodName + i);
            }
            equipmentList.Clear();
        }
    }

    private static void CheckForAlreadyEquipedAndAdjustCountyGoods(CountyData countyData, GoodData goodToEquip, PopulationData populationData)
    {
        // If there is no equipment equiped then take one from the county goods and equip it.
        if (populationData.equipment[AllEnums.GetCorrectEquipmentSlot(goodToEquip.equipmentData.equipmentType)] == null)
        {
            populationData.equipment[AllEnums.GetCorrectEquipmentSlot(goodToEquip.equipmentData.equipmentType)] 
                = goodToEquip;
            // Take 1 good away from the County goods.
            Haulmaster.AdjustCountyGoodAmount(countyData, goodToEquip.countyGoodType, -1);
        }
        else
        {
            // If the goodData is already equiped then don't do anything.
            if (populationData.equipment[AllEnums.GetCorrectEquipmentSlot(goodToEquip.equipmentData.equipmentType)].goodName 
                == goodToEquip.goodName)
            {
                GD.Print($"{goodToEquip.goodName} is already equiped.");
            }
            else
            {
                // In all other cases put the populations equiped equipment back, then take the new equipment away from
                // the county goods.
                // Put back the original equiped equipment.
                Haulmaster.AdjustCountyGoodAmount(countyData, 
                    populationData.equipment[AllEnums.GetCorrectEquipmentSlot(goodToEquip.equipmentData.equipmentType)].countyGoodType
                    , 1);
                // Get the equipment from the county goods.
                Haulmaster.AdjustCountyGoodAmount(countyData, goodToEquip.countyGoodType, -1);
                // Equip the new goodToEquip on the populationData.
                populationData.equipment[AllEnums.GetCorrectEquipmentSlot(goodToEquip.equipmentData.equipmentType)] 
                    = goodToEquip;
                GD.Print($"{goodToEquip.goodName} is different then currently equiped.");
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
        return equipmentList = [.. equipmentList.OrderBy(e => e.equipmentData.equipmentTier)];
    }
}