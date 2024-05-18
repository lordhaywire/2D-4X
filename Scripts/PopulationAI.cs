using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace
{
    public partial class PopulationAI : Node
    {
        [Export] private int willWorkLoyalty = 50;
        private List<CountyImprovementData> prioritizedCountyImprovements = [];
        public override void _Ready()
        {
            Clock.Instance.FirstRun += HourZero;
            Clock.Instance.HourZero += HourZero;
        }

        private void HourZero()
        {
            AdjustPopulationActivity();
            //PrioritizeWork();
            DecideNextActivity();
        }

        private void PrioritizeWork()
        {
            foreach (County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
            {
                foreach (CountyImprovementData countyImprovementData
                    in county.countyData.underConstructionCountyImprovements)
                {
                    //prioritizedCountyImprovements =

                }


            }
        }

        // Have the world population decide what they are doing the next day.
        private void DecideNextActivity()
        {
            // Go through each county.
            foreach (County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
            {
                // Go through each person in the county.
                foreach (CountyPopulation countyPopulation in county.countyData.countyPopulationList)
                {
                    if(countyPopulation.currentActivity != AllText.Activities.IDLE)
                    {
                        GD.Print("Is not idle.");
                        return; // I am not sure this will work.
                    }
                    else
                    {
                        // Check if they are loyal and helpful.
                        if (CheckLoyalty(countyPopulation) == true && CheckForUnhelpful(countyPopulation) == false)
                        {
                            GD.Print($"{countyPopulation.firstName} is loyal and helpful.");
                            CheckForPreferredWork(county, countyPopulation);
                            //CheckForAvailableWork(county, countyPopulation);
                        }
                    }
                }
            }
        }

        private void CheckForPreferredWork(County county, CountyPopulation countyPopulation)
        {
            foreach (CountyImprovementData countyImprovementData in county.countyData.completedCountyImprovements)
            {
                if(countyPopulation.preferredSkill.skill == countyImprovementData.workSkill)
                {
                    // Check to see if there is available spot to work.
                }
            }
        }

        private static void CheckForAvailableWork(County county, CountyPopulation countyPopulation)
        {
            foreach (CountyImprovementData countyImprovementData in county.countyData.underConstructionCountyImprovements)
            {
                if (countyImprovementData.underConstruction == true)
                {
                    if (countyPopulation.nextActivity == AllText.Activities.IDLE && countyImprovementData.currentBuilders
                        < countyImprovementData.maxBuilders)
                    {
                        countyPopulation.nextActivity = AllText.Activities.BUILDING;
                        countyPopulation.nextImprovement = countyImprovementData;
                        countyImprovementData.currentBuilders++;
                        GD.Print($"{countyPopulation.firstName} {countyPopulation.lastName} is building {countyImprovementData.improvementName}");
                    }
                    else
                    {
                        GD.Print($"{countyPopulation.firstName} {countyPopulation.lastName} not getting reassigned.");
                    }
                }
                else
                {
                    GD.Print($"{countyImprovementData.improvementName} is not being built.");
                }
            }
        }
        private static bool CheckForUnhelpful(CountyPopulation countyPopulation)
        {
            bool isUnhelpful = false;
            foreach (PerkData perkData in countyPopulation.perks)
            {
                if (perkData.perkName == AllPerks.Instance.allPerks[(int)AllEnums.Perks.Unhelpful].perkName)
                {
                    isUnhelpful = true;
                }
            }
            return isUnhelpful;
        }
        private bool CheckLoyalty(CountyPopulation countyPopulation)
        {
            if (countyPopulation.loyaltyAttribute > willWorkLoyalty)
            {
                return true;
            }
            else
            {
                return false;
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