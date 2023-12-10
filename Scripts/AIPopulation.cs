using Godot;
using System;

namespace PlayerSpace
{
    public partial class AIPopulation : Node
    {
        public override void _Ready()
        {
            Clock.Instance.FirstRun += HourZero;
            Clock.Instance.HourZero += HourZero;
        }

        private void HourZero()
        {
            DecideNextActivity();
            AdjustPopulationActivity();
        }

        // Have the world population decide what they are doing the next day.
        private void DecideNextActivity()
        {
            // Go through every county.
            foreach (SelectCounty county in Globals.Instance.countiesParent.GetChildren())
            {
                // Go through this counties population.
                foreach (CountyPopulation person in county.countyData.countyPopulation)
                {
                    if (person.currentActivity != AllText.Jobs.IDLE)
                    {
                        GD.Print($"{person.firstName} {person.lastName} not getting reassigned, they are working on {person.currentActivity}");
                        continue;
                    }
                    else
                    {
                        foreach (CountyImprovementData countyImprovementData in county.countyData.underConstructionCountyImprovements)
                        {
                            if (countyImprovementData.currentWorkers < countyImprovementData.maxWorkers 
                                && countyImprovementData.isBeingBuilt == true)
                            {
                                person.nextActivity = AllText.Jobs.BUILDING;
                                person.nextImprovement = countyImprovementData;
                                countyImprovementData.currentWorkers++;
                                GD.Print($"{person.firstName} {person.lastName} is building {countyImprovementData.improvementName}");
                            }
                        }
                    }
                }
            }
        }

        // Adjust all of the world population!
        private static void AdjustPopulationActivity()
        {
            // Go through every county.
            foreach (SelectCounty county in Globals.Instance.countiesParent.GetChildren())
            {
                // Go through this counties population.
                foreach (CountyPopulation person in county.countyData.countyPopulation)
                {
                    person.currentActivity = person.nextActivity;
                    person.currentImprovement = person.nextImprovement;
                }

                foreach (CountyPopulation hero in county.countyData.heroCountyPopulation)
                {
                    if(hero.token == null)
                    {
                        hero.currentActivity = hero.nextActivity;
                        hero.currentImprovement = hero.nextImprovement;
                    }
                }
            }
        }
    }
}