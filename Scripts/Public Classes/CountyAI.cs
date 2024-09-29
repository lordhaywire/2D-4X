using Godot;
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
        if (!county.countyData.CheckEnoughCountyFactionResource(AllEnums.FactionResourceType.Food))
        {
            CountyImprovementData foodBuilding = FindCountyImpovementOfType(county, AllEnums.FactionResourceType.Food);

            if (foodBuilding != null)
            {
                /*
                GD.Print($"{county.countyData.factionData.factionName}: {county.countyData.countyName} " +
                         $"wants to build a {foodBuilding.improvementName}");
                */
                if (banker.CheckBuildingCost(county.countyData.factionData, foodBuilding))
                {
                    banker.ChargeForBuilding(county.countyData.factionData, foodBuilding);
                    BuildImprovement(county.countyData, foodBuilding);
                }
                else
                {
                    //GD.Print($"{county.countyData.factionData.factionName} doesn't have enough resources to build {foodBuilding.improvementName}.");
                }
            }
            else
            {
                GD.Print("No suitable food building found.");
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
        countyImprovementData.improvementName = $"{TranslationServer.Translate(countyImprovementData.improvementName)} " 
            + (countyData.underConstructionCountyImprovements.Count + 1).ToString();
        countyData.underConstructionCountyImprovements.Add(countyImprovementData);
        countyData.underConstructionCountyImprovements.Sort((x,y) => string.Compare(x.improvementName, y.improvementName));
    }
    public static CountyImprovementData FindCountyImpovementOfType(County county, AllEnums.FactionResourceType factionResourceType)
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