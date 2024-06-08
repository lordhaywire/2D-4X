using Godot;
using PlayerSpace;

namespace PlayerSpace
{
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
            if (!banker.CheckEnoughCountyFactionResource(county, AllEnums.FactionResourceType.Food))
            {
                CountyImprovementData foodBuilding = FindCountyImpovementOfType(county, AllEnums.FactionResourceType.Food);

                if (foodBuilding != null)
                {
                    GD.Print($"{county.countyData.factionData.factionName}: {county.countyData.countyName} " +
                             $"wants to build a {foodBuilding.improvementName}");

                    if (banker.CheckBuildingCost(county.countyData.factionData, foodBuilding))
                    {
                        banker.ChargeForBuilding(county.countyData.factionData, foodBuilding);
                        BuildImprovement(county.countyData, foodBuilding);
                    }
                    else
                    {
                        GD.Print($"{county.countyData.factionData.factionName} doesn't have enough resources to build {foodBuilding.improvementName}.");
                    }
                }
                else
                {
                    GD.Print("No suitable food building found.");
                }
            }
            else
            {
                GD.Print($"{county.countyData.factionData.factionName} has enough food.");
            }
        }


        public void BuildImprovement(CountyData countyData, CountyImprovementData countyImprovementData)
        {
            countyImprovementData.underConstruction = true;
            countyData.underConstructionCountyImprovements.Add(countyImprovementData);
            GD.Print($"{countyData.factionData.factionName} is building {countyImprovementData.improvementName}.");
        }
        public static CountyImprovementData FindCountyImpovementOfType(County county, AllEnums.FactionResourceType factionResourceType)
        {
            foreach (CountyImprovementData countyImprovementData in county.countyData.allCountyImprovements)
            {
                if (countyImprovementData.underConstruction || countyImprovementData.isBuilt)
                {
                    //GD.Print($"{improvementData.improvementName} is already being built.");
                    return null;
                }
                if (countyImprovementData.resourceData.factionResourceType == factionResourceType)
                {
                    //GD.Print($"{factionData.factionName} found {improvementData.improvementName} in {countyDataItem.countyName}.");
                    return countyImprovementData;
                }
            }
            return null;
        }
    }
}