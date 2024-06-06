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
            if (!banker.CheckEnoughCountyFood(county))
            {
                banker.FindFoodBuilding(county, out CountyImprovementData foodBuilding);

                if (foodBuilding != null)
                {
                    GD.Print($"{county.countyData.factionData.factionName}: {county.countyData.countyName} " +
                             $"wants to build a {foodBuilding.improvementName}");

                    if (banker.CheckBuildingCost(county.countyData.factionData, foodBuilding))
                    {
                        banker.ChargeForBuilding(county.countyData.factionData, foodBuilding);
                        banker.BuildImprovement(county.countyData, foodBuilding);
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
    }
}