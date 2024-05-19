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
            CompleteWorkPerPerson();
            CheckWorkComplete();
        }

        public void CountIdleWorkers(CountyData countyData)
        {
            // I don't think this is very efficient.
            int idleWorkers = 0;

            foreach (CountyPopulation person in countyData.countyPopulationList)
            {
                if (person.currentActivity == AllText.Activities.IDLE && person.nextActivity == AllText.Activities.IDLE)
                {
                    idleWorkers++;
                }
            }
            countyData.IdleWorkers = idleWorkers;
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
                        person.currentActivity = AllText.Activities.IDLE;
                        person.nextActivity = AllText.Activities.IDLE;
                        //GD.Print($"{person.firstName} is {person.nextActivity}.");
                    }
                }
                CountIdleWorkers(selectCounty.countyData);
            }
        }
        private static void CompleteWorkPerPerson()
        {
            // This is fucked.
            // Go through all the counties and have people building add their work to the building.
            foreach (County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
            {
                foreach (CountyPopulation person in county.countyData.countyPopulationList)
                {
                    if (person.CurrentConstruction != null)
                    {
                        //GD.Print($"{person.firstName} is building {person.currentImprovement.improvementName}.");
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
                /*
                foreach (CountyImprovementData countyImprovementData in county.countyData.underConstructionCountyImprovements)
                {
                    GD.Print($"{countyImprovementData.improvementName} has completed {countyImprovementData.currentAmountOfConstruction} work.");
                }
                */
            }
        }

        // This doesn't work.
        private void UnsubscribeEvents()
        {
            GD.Print("Exiting a tree.");
            Clock.Instance.WorkDayOver -= WorkDayOverForPopulation;
        }

    }
}