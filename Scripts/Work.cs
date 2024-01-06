using Godot;

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
                if (factionData.factionLeader.leaderOfPeoplePerk == true)
                {
                    factionData.Influence += Globals.Instance.dailyInfluenceGain;
                    //GD.PrintRich($"[rainbow]{factionData.factionName} has {factionData.Influence}.");
                }
                else
                {
                    //GD.Print($"The leader of {factionData.factionName} isn't a leader of people.");
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

            foreach(CountyPopulation person in countyData.countyPopulation)
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
            foreach (SelectCounty selectCounty in Globals.Instance.countiesParent.GetChildren())
            {
                foreach (CountyPopulation person in selectCounty.countyData.countyPopulation)
                {
                    // ? is null checking currentImprovement.
                    if (person.currentImprovement?.isBuilt == true)
                    {
                        person.currentImprovement = null;
                        person.nextImprovement = null;
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
            // Go through all the counties and have people building add their work to the building.
            foreach (SelectCounty county in Globals.Instance.countiesParent.GetChildren())
            {
                foreach (CountyPopulation person in county.countyData.countyPopulation)
                {
                    if (person.currentImprovement != null)
                    {
                        //GD.Print($"{person.firstName} is building {person.currentImprovement.improvementName}.");

                        person.currentImprovement.currentAmountOfConstruction++; // This is eventually going to be a skill check.

                        // Checks to see if the building is completed.
                        if (person.currentImprovement.currentAmountOfConstruction >= person.currentImprovement.maxAmountOfConstruction)
                        {
                            // This is having every population working on that building set that building as built.
                            // So it is repeating the setting to true a bunch of times.  This is ineffecient code.
                            // Some of the population will be working on different buildings too....
                            person.currentImprovement.isBuilt = true;
                            person.currentImprovement.isBeingBuilt = false;
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