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
            GD.Print("Check Work Complete.");
            /*
            for (int popAgain = 0; popAgain < item.Value.Count; popAgain++)
            {
                if (item.Value[popAgain].currentBuilding != null && item.Value[popAgain].currentBuilding.GetComponent<BuildingInfo>().isBuilt == true)
                {
                    item.Value[popAgain].nextActivity = AllText.Jobs.IDLE;
                    item.Value[popAgain].nextBuilding = null;
                }
            }
            */
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
                        
                        person.currentImprovement.workCompleted++; // This is eventually going to be a skill check.
                        // Checks to see if the building is completed.
                        if (person.currentImprovement.workCompleted >= person.currentImprovement.maxAmountOfWork)
                        {
                            // This is having every population working on that building set that building as built.
                            // So it is repeating the setting to true a bunch of times.  This is ineffecient code.
                            // Some of the population will be working on different buildings too....
                            person.currentImprovement.isBuilt = true;
                            person.currentImprovement.isBeingBuilt = false;
                            ;
                        }
                        
                    }
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