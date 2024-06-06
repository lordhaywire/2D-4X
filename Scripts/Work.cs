using Godot;
using System.Linq;

namespace PlayerSpace
{
    public class Work
    {
        private readonly Banker banker = new();
        private County county;

        private readonly int dailyScavengedFood = 5;
        private readonly int dailyScavengedRemnants = 5;
        // End work for all of the world population!        
        public void WorkDayOverForPopulation(County county)
        {
            this.county = county;
            GD.Print("Work Hour Zero");
            CompleteScavengingPerPerson();
            CompleteBuildingPerPerson();
            CheckConstructionComplete();
            GiveIdlePeopleBonusHappiness(); // This would have to be somewhere else, I think.  Like up above.
        }

        private static void GiveIdlePeopleBonusHappiness()
        {
            GD.Print("Give Idle People Bonus Happyness.");
        }

        // Once they have generated they have generated what they have scavenged they are set to idle
        // so they only scavenge every other day.
        private void CompleteScavengingPerPerson()
        {
            foreach (CountyPopulation countyPopulation in county.countyData.countyPopulationList)
            {
                if (countyPopulation.currentActivity == AllEnums.Activities.Scavenge)
                {
                    Activities activities = new();
                    GD.Print($"{countyPopulation.firstName} is generating resources.");
                    GenerateScavengedResources();
                    activities.UpdateNext(countyPopulation, AllEnums.Activities.Idle);
                }
            }
        }

        // This should be updated with canned goods instead of vegetables.
        // We need to skill check this for bonus resources.
        private void GenerateScavengedResources()
        {
            int randomResourceNumber = Globals.Instance.random.Next(0, 2);
            if (randomResourceNumber == 0)
            {
                banker.AddResourceToCounty(county, AllEnums.CountyResourceType.Vegetables, true, dailyScavengedFood);
            }
            else
            {
                banker.AddResourceToCounty(county, AllEnums.CountyResourceType.Scrap, false, dailyScavengedRemnants);
            }
        }

        private void CompleteBuildingPerPerson()
        {
            // This is fucked.
            // Go through all the counties and have people building add their work to the building.

            foreach (CountyPopulation person in county.countyData.countyPopulationList)
            {
                //GD.Print($"If this is blank then it is null: {person?.CurrentConstruction}");
                if (person.CurrentConstruction != null)
                {
                    // This needs to check if the county improvement is being built or if they are working there.
                    person.CurrentConstruction.currentAmountOfConstruction++; // This is eventually going to be a skill check.

                    // Checks to see if the building is completed.
                    if (person.CurrentConstruction.currentAmountOfConstruction >= person.CurrentConstruction.maxAmountOfConstruction)
                    {
                        // This is having every population working on that building set that building as built.
                        // So it is repeating the setting to true a bunch of times.  This is ineffecient code.
                        // Some of the population will be working on different buildings too....
                        county.countyData.completedCountyImprovements.Add(person.CurrentConstruction);
                        person.CurrentConstruction.isBuilt = true;
                        person.CurrentConstruction.underConstruction = false;
                    }
                }
            }
        }

        // Go through everyone in this county again and clear out their job if their building is done.
        private void CheckConstructionComplete()
        {
            Banker banker = new();

            foreach (CountyPopulation countyPopulation in county.countyData.countyPopulationList)
            {
                // ? is null checking currentImprovement.
                if (countyPopulation.CurrentConstruction?.isBuilt == true)
                {
                    Activities activities = new();
                    countyPopulation.CurrentConstruction = null;
                    countyPopulation.NextConstruction = null;
                    activities.UpdateNext(countyPopulation, AllEnums.Activities.Idle);
                }
            }
            banker.CountIdleWorkers(county);
        }
    }
}