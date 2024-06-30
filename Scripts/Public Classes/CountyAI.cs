using Godot;

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

        // This doesn't work until we assign people to the list of workers on the County Improvement.
        public static void CheckIfCountyImprovementsAreDone(CountyData countyData)
        {
            foreach (CountyImprovementData countyImprovementData in countyData.underConstructionCountyImprovements)
            {
                // If the county improvement is done, make everyone working on it idle.
                // Set their current work to null.
                if (countyImprovementData.CheckIfCountyInprovementDone())
                {
                    foreach(CountyPopulation countyPopulation in countyImprovementData.countyPopulationAtImprovement)
                    {
                        countyPopulation.UpdateActivity(AllEnums.Activities.Idle);
                        countyPopulation.UpdateCurrentWork(null);
                    }
                    // Set countyImprovement status to Complete
                    countyImprovementData.SetCountyImprovementComplete();
                    // Clear the people on the county improvement list.
                    countyImprovementData.countyPopulationAtImprovement.Clear();
                    // Move the county improvement to the correct list and remove it from the old list.
                    countyData.MoveCountyImprovementToCompletedList(countyImprovementData);
                }
            }
        }

        public void BuildImprovement(CountyData countyData, CountyImprovementData countyImprovementData)
        {
            countyImprovementData.status = AllEnums.CountyImprovementStatus.UnderConstruction;
            countyData.underConstructionCountyImprovements.Add(countyImprovementData);
            //GD.Print($"{countyData.factionData.factionName} is building {countyImprovementData.improvementName}.");
        }
        public static CountyImprovementData FindCountyImpovementOfType(County county, AllEnums.FactionResourceType factionResourceType)
        {
            foreach (CountyImprovementData countyImprovementData in county.countyData.allCountyImprovements)
            {
                if (countyImprovementData.resourceData.factionResourceType == factionResourceType
                    && countyImprovementData.status == AllEnums.CountyImprovementStatus.None)
                {
                    //GD.Print($"{factionData.factionName} found {improvementData.improvementName} in {countyDataItem.countyName}.");
                    return countyImprovementData;
                }
            }
            return null;
        }
    }
}