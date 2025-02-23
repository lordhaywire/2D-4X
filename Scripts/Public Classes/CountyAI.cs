using Godot;

namespace PlayerSpace;

public class CountyAI
{
    public void DecideIfHeroUsesNewestEquipment(County county)
    {

    }
    // This needs to take into account the new HeroPersonalities.
    public void DecideBuildingCountyImprovements(County county)
    {
        if (county.countyData.factionData.isPlayer)
        {
            return;
        }

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
                if (Banker.CheckBuildingCost(county.countyData, foodBuilding))
                {
                    Banker.ChargeForBuilding(county.countyData, foodBuilding);
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

    // I think this needs to be moved somewhere too.
    public void BuildImprovement(CountyData countyData, CountyImprovementData countyImprovementData)
    {
        countyImprovementData.status = AllEnums.CountyImprovementStatus.UnderConstruction;
        // Numbers improvement if there is more then 1.
        NumberBuiltImprovement(countyData, countyImprovementData);

        countyData.underConstructionCountyImprovementList.Add(countyImprovementData);
    }
    

    // What does this do, and why is it here?
    private void NumberBuiltImprovement(CountyData countyData, CountyImprovementData countyImprovementData)
    {
        int underConstructionImprovements =  CountDuplicateImprovement(countyImprovementData, countyData.underConstructionCountyImprovementList);
        int completedImprovements = CountDuplicateImprovement(countyImprovementData,countyData.completedCountyImprovementList);
        int numberOfDuplicateImprovements = underConstructionImprovements + completedImprovements;
        countyImprovementData.numberBuilt = ++numberOfDuplicateImprovements;
        //GD.Print($"Under Construction Improvements: {underConstructionImprovements}");
        //GD.Print($"Completed Construction Improvements: {completedImprovements}");
        //GD.Print($"County Improvement Number Built: {countyImprovementData.numberBuilt}");
    }

    // This probably needs to be moved to CountyImprovementData, or like the Banker or some shit.
    private static int CountDuplicateImprovement(CountyImprovementData originalCountyImprovementData
        , Godot.Collections.Array<CountyImprovementData> countyImprovementDatas)
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

    // This probably needs to be moved to CountyImprovementData, or like the Banker or some shit.
    public static CountyImprovementData FindCountyImpovementOfType(County county
        , AllEnums.FactionGoodType factionResourceType)
    {
        foreach (CountyImprovementData countyImprovementData in county.countyData.factionData.allCountyImprovements)
        {
            if (countyImprovementData.factionResourceType == factionResourceType)
            {
                GD.Print($"{county.countyData.factionData.factionName} found {countyImprovementData.improvementName} " +
                    $"in {county.countyData.countyName}.");
                return countyImprovementData;
            }
        }
        return null;
    }
}