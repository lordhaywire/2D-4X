using Godot;

namespace PlayerSpace
{
    public partial class Work : Node
    {
        public override void _Ready()
        {
            Clock.Instance.HourZero += DayStart;
            Clock.Instance.WorkDayOver += WorkDayOverForPopulation;
        }

        private void DayStart()
        {
            GenerateLeaderInfluence();
        }

        private static void GenerateLeaderInfluence()
        {
            for (int i = 0; i < Globals.Instance.factions.Count; i++)
            {
                FactionData faction = Globals.Instance.factions[i];
                if (faction.factionLeader.leaderOfPeoplePerk == true)
                {
                    faction.Influence += Globals.Instance.dailyInfluenceGain;
                }
                else
                {
                    GD.Print($"The leader of {faction.factionName} isn't a leader of people.");
                }
            }
        }

        // End work for all of the world population!        
        private static void WorkDayOverForPopulation()
        {
            CompleteWorkPerPerson();
            CheckWorkComplete();
        }

        // Go through everyone in this county again and clear out their job if their building is done.
        private static void CheckWorkComplete()
        {
            foreach (SelectCounty county in Globals.Instance.countiesParent.GetChildren())
            {
                foreach (CountyPopulation person in county.countyData.countyPopulation)
                {
                    // ? is null checking currentImprovement.
                    if(person.currentImprovement?.isBuilt == true)
                    {
                        person.currentImprovement = null;
                        person.currentActivity = AllText.Jobs.IDLE;
                        person.nextActivity = AllText.Jobs.IDLE;
                        GD.Print($"{person.firstName} is {person.nextActivity}.");
                    }           
                }
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
                        GD.Print($"{person.firstName} they are building {person.currentImprovement.improvementName}.");
                        
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
                foreach(CountyImprovementData countyImprovementData in county.countyData.underConstructionCountyImprovements)
                {
                    GD.Print($"{countyImprovementData.improvementName} has completed {countyImprovementData.currentAmountOfConstruction} work.");
                }
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