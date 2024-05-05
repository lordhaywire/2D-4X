using Godot;
using System;

namespace PlayerSpace
{
    public partial class PopulationAI : Node
    {
        public override void _Ready()
        {
            Clock.Instance.FirstRun += HourZero;
            Clock.Instance.HourZero += HourZero;
        }

        private void HourZero()
        {
            AdjustPopulationActivity();
            DecideNextActivity();
        }

        // Have the world population decide what they are doing the next day.
        private static void DecideNextActivity()
        {
            // Go through every county.
            foreach (Node node in Globals.Instance.countiesParent.GetChildren())
            {
                County selectCounty = (County)node;
                // Go through every building in that county and see if it is being built.
                foreach (CountyImprovementData countyImprovementData in selectCounty.countyData.underConstructionCountyImprovements)
                {
                    if (countyImprovementData.isBeingBuilt == true)
                    {
                        // Go through this counties population.
                        foreach (CountyPopulation person in selectCounty.countyData.countyPopulationList)
                        {
                            if (person.nextActivity == AllText.Activities.IDLE && countyImprovementData.currentBuilders
                                < countyImprovementData.maxBuilders)
                            {
                                person.nextActivity = AllText.Activities.BUILDING;
                                person.nextImprovement = countyImprovementData;
                                countyImprovementData.currentBuilders++;
                                //GD.Print($"{person.firstName} {person.lastName} is building {countyImprovementData.improvementName}");
                            }
                            else
                            {
                                //GD.Print($"{person.firstName} {person.lastName} not getting reassigned.");
                            }
                        }
                    }
                    else
                    {
                        //GD.Print($"{countyImprovementData.improvementName} is not being built.");
                    }
                }
            }
        }

        // Adjust all of the world population!
        private static void AdjustPopulationActivity()
        {
            // Go through every county.
            foreach (Node node in Globals.Instance.countiesParent.GetChildren())
            {
                County selectCounty = (County)node;
                // Go through this counties population.
                foreach (CountyPopulation person in selectCounty.countyData.countyPopulationList)
                {
                    person.currentActivity = person.nextActivity;
                    person.currentImprovement = person.nextImprovement;
                }

                foreach (CountyPopulation hero in selectCounty.countyData.herosInCountyList)
                {
                    if (hero.token == null)
                    {
                        hero.currentActivity = hero.nextActivity;
                        hero.currentImprovement = hero.nextImprovement;
                    }
                }
                Work.Instance.CountIdleWorkers(selectCounty.countyData);
            }
        }
    }
}