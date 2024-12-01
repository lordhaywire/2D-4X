using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;

public class CountyAI
{
    public void DecideBuildingCountyImprovements(County county)
    {
        if (county.countyData.factionData.isPlayer)
        {
            return;
        }

        Banker banker = new();

        // Check if the county has enough food. If not, build a food building.
        if (!county.countyData.CheckEnoughCountyFactionResource(AllEnums.FactionGoodType.Food))
        {
            CountyImprovementData foodBuilding = FindCountyImpovementOfType(county, AllEnums.FactionGoodType.Food);

            if (foodBuilding != null)
            {
                /*
                GD.Print($"{county.countyData.factionData.factionName}: {county.countyData.countyName} " +
                         $"wants to build a {foodBuilding.improvementName}");
                */
                if (banker.CheckBuildingCost(county.countyData, foodBuilding))
                {
                    banker.ChargeForBuilding(county.countyData, foodBuilding);
                    BuildImprovement(county.countyData, foodBuilding);
                }
                else
                {
                    //GD.Print($"{county.countyData.factionData.factionName} doesn't have enough resources to build {foodBuilding.improvementName}.");
                }
            }
            else
            {
                //GD.Print("No suitable food building found.");
            }
        }
        else
        {
            //GD.Print($"{county.countyData.factionData.factionName} has enough food.");
        }
    }

    public void BuildImprovement(CountyData countyData, CountyImprovementData countyImprovementData)
    {
        countyImprovementData.status = AllEnums.CountyImprovementStatus.UnderConstruction;
        // Numbers improvement if there is more then 1.
        NumberBuiltImprovement(countyData, countyImprovementData);

        countyData.underConstructionCountyImprovements.Add(countyImprovementData);
    }
    
    private void NumberBuiltImprovement(CountyData countyData, CountyImprovementData countyImprovementData)
    {
        int underConstructionImprovements =  CountDuplicateImprovement(countyImprovementData, countyData.underConstructionCountyImprovements);
        int completedImprovements = CountDuplicateImprovement(countyImprovementData,countyData.completedCountyImprovements);
        int numberOfDuplicateImprovements = underConstructionImprovements + completedImprovements;
        countyImprovementData.numberBuilt = ++numberOfDuplicateImprovements;
        //GD.Print($"Under Construction Improvements: {underConstructionImprovements}");
        //GD.Print($"Completed Construction Improvements: {completedImprovements}");
        //GD.Print($"County Improvement Number Built: {countyImprovementData.numberBuilt}");
    }

    int CountDuplicateImprovement(CountyImprovementData originalCountyImprovementData, List<CountyImprovementData> countyImprovementDatas)
    {
        int number = 0;
        foreach (CountyImprovementData countyImprovementData in countyImprovementDatas)
        {
            if(originalCountyImprovementData.improvementName == countyImprovementData.improvementName)
            {
                number++;
            }
        }
        return number;
    }
    public static CountyImprovementData FindCountyImpovementOfType(County county, AllEnums.FactionGoodType factionResourceType)
    {
        foreach (CountyImprovementData countyImprovementData in county.countyData.factionData.allCountyImprovements)
        {
            if (countyImprovementData.factionResourceType == factionResourceType
                && countyImprovementData.status == AllEnums.CountyImprovementStatus.None)
            {
                //GD.Print($"{factionData.factionName} found {improvementData.improvementName} in {countyDataItem.countyName}.");
                return countyImprovementData;
            }
        }
        return null;
    }
}