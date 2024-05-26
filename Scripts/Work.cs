using Godot;
using System.Linq;

namespace PlayerSpace
{
    public partial class Work : Node
    {
        public static Work Instance { get; private set; }

        public override void _Ready()
        {
            Instance = this;

            Clock.Instance.HourZero += DayStart;
            Clock.Instance.WorkDayOver += WorkDayOverForPopulation;
        }

        private void DayStart()
        {
            GenerateLeaderInfluence();
        }

        private static void GenerateLeaderInfluence()
        {
            foreach (FactionData factionData in Globals.Instance.factionDatas)
            {
                foreach (PerkData perkData in factionData.factionLeader.perks)
                {
                    if (AllPerks.Instance.allPerks[(int)AllEnums.Perks.LeaderofPeople].perkName
                        == perkData.perkName)
                    {
                        factionData.Influence += Globals.Instance.dailyInfluenceGain;
                    }
                }
            }
        }

        // End work for all of the world population!        
        private void WorkDayOverForPopulation()
        {
            foreach (County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
            {
                CompleteScavengingPerPerson(county);
                CompleteBuildingPerPerson();
                CheckWorkComplete();
                GiveIdlePeopleBonusHappyness();
            }
        }

        private void CompleteScavengingPerPerson(County county)
        {
            foreach (CountyPopulation countyPopulation in county.countyData.countyPopulationList)
            {
                if(countyPopulation.currentActivity == AllEnums.Activities.Scavenge)
                {
                    GenerateScavengedResources(county);
                    CheckForEnoughFood();
                    CheckForEnoughScrap();
                    CheckForAvailableJobs();
                }
            }
        }

        // This should be updated with canned goods instead of vegetables.
        // 5 should probably be changed to a global int.
        private void GenerateScavengedResources(County county)
        {
            int randomResourceNumber = Globals.Instance.random.Next(0, 2);
            if(randomResourceNumber == 0)
            {
                Banker.Instance.AddResourceToCounty(county, AllEnums.CountyResourceType.Vegetables, true, 5);
            }
            else
            {
                Banker.Instance.AddResourceToCounty(county, AllEnums.CountyResourceType.Scrap, false, 5);
            }
        }

        private void CompleteBuildingPerPerson(County county)
        {
            // This is fucked.
            // Go through all the counties and have people building add their work to the building.

            foreach (CountyPopulation person in county.countyData.countyPopulationList)
            {
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
        private void CheckWorkComplete()
        {
            foreach (County selectCounty in Globals.Instance.countiesParent.GetChildren().Cast<County>())
            {
                foreach (CountyPopulation person in selectCounty.countyData.countyPopulationList)
                {
                    // ? is null checking currentImprovement.
                    if (person.CurrentConstruction?.isBuilt == true)
                    {
                        person.CurrentConstruction = null;
                        person.NextConstruction = null;
                        person.currentActivity = AllEnums.Activities.Idle;
                        person.nextActivity = AllEnums.Activities.Idle;
                    }
                }
                CountIdleWorkers(selectCounty.countyData);
            }
        }

        public void CountIdleWorkers(CountyData countyData)
        {
            // I don't think this is very efficient.
            int idleWorkers = 0;

            foreach (CountyPopulation person in countyData.countyPopulationList)
            {
                if (person.currentActivity == AllEnums.Activities.Idle && person.nextActivity == AllEnums.Activities.Idle)
                {
                    idleWorkers++;
                }
            }
            countyData.IdleWorkers = idleWorkers;
        }
    }
}