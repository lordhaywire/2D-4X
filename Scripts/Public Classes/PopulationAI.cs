using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace
{
    public class PopulationAI
    {
        // This is a dumb name for this method.
        public static void IsThereEnoughFood(CountyData countyData)
        {
            int amountOfFood = Banker.CountFactionResourceOfType(countyData, AllEnums.FactionResourceType.Food);
            int amountOfPeople = Banker.CountEveryoneInCounty(countyData);
            if (amountOfFood >= (Globals.Instance.foodToGainHappiness * amountOfPeople))
            {
                // Happiness is added when the people eat the food.
                countyData.PopulationEatsFood(countyData.herosInCountyList,
                    Globals.Instance.foodToGainHappiness);
                countyData.PopulationEatsFood(countyData.armiesInCountyList,
                    Globals.Instance.foodToGainHappiness);
                countyData.PopulationEatsFood(countyData.countyPopulationList,
                    Globals.Instance.foodToGainHappiness);
                // Visiting heroes need to be able to eat food too.  It needs to improve faction relations.
            }
            else if (amountOfFood >= (Globals.Instance.foodToGainNothing * amountOfPeople))
            {
                //GD.Print("People get jack shit for happiness.");
                // Happiness is added when the people eat the food.
                countyData.PopulationEatsFood(countyData.herosInCountyList,
                    Globals.Instance.foodToGainNothing);
                countyData.PopulationEatsFood(countyData.armiesInCountyList,
                    Globals.Instance.foodToGainNothing);
                countyData.PopulationEatsFood(countyData.countyPopulationList,
                    Globals.Instance.foodToGainNothing);
                // Visiting heroes need to be able to eat food too.  It needs to improve faction relations.

            }
            else if (amountOfFood >= amountOfPeople)
            {
                // Happiness is removed when the people eat the food.
                countyData.PopulationEatsFood(countyData.herosInCountyList,
                    Globals.Instance.foodToLoseHappiness);
                countyData.PopulationEatsFood(countyData.armiesInCountyList,
                    Globals.Instance.foodToLoseHappiness);
                countyData.PopulationEatsFood(countyData.countyPopulationList,
                    Globals.Instance.foodToLoseHappiness);
                // Visiting heroes need to be able to eat food too.  It needs to improve faction relations.

            }
            else
            {
                //GD.PrintRich($"[rainbow]People are starting to starve!!");
                // Eating the last of the food, then major penalty for starvation.
                // It will already reduce their happiness by 1 in the Population Eats Food method.
                countyData.PopulationEatsFood(countyData.herosInCountyList,
                    Globals.Instance.foodToLoseHappiness);
                countyData.PopulationEatsFood(countyData.armiesInCountyList,
                    Globals.Instance.foodToLoseHappiness);
                countyData.PopulationEatsFood(countyData.countyPopulationList,
                    Globals.Instance.foodToLoseHappiness);
                // Visiting heroes need to be able to eat food too.  It needs to improve faction relations.
            }
        }

        public static void WorkDayOverForPopulation(County county)
        {
            foreach (CountyPopulation countyPopulation in county.countyData.countyPopulationList)
            {
                switch (countyPopulation.activity)
                {
                    case AllEnums.Activities.Scavenge:
                        GD.Print($"{countyPopulation.firstName} is generating scavenged resources.");
                        Banker.GenerateScavengedResources(county, countyPopulation);
                        countyPopulation.UpdateActivity(AllEnums.Activities.Idle);
                        break;
                    case AllEnums.Activities.Build:
                        CompleteConstruction(countyPopulation);
                        // If the building is done they will be set to idle somewhere else.
                        break;
                    case AllEnums.Activities.Work:
                        // Produce resources based on the countyimprovement
                        county.countyData.countyResources[countyPopulation.CurrentCountyImprovment.resourceData.countyResourceType].amount +=
                            Banker.GenerateWorkResourceWithSkillCheck(countyPopulation.CurrentCountyImprovment
                            , countyPopulation.skills[countyPopulation.CurrentCountyImprovment.workSkill].skillLevel);
                        /*
                        GD.Print($"{countyPopulation.firstName} worked at {countyPopulation.CurrentWork.improvementName}" +
                            $" and now {county.countyData.countyName} has " +
                            $"{county.countyData.resources[countyPopulation.CurrentWork.resourceData.countyResourceType].amount}");
                        */
                        // Check loyalty to see if they still want to work there and if they don't then they
                        // get set to idle.
                        KeepWorkingAtCountyImprovement(countyPopulation);
                        break;
                    case AllEnums.Activities.Idle:
                        // Give idle people their bonus happiness.
                        countyPopulation.AddRandomHappiness(5);
                        break;
                }
            }
            GD.PrintRich($"[rainbow]{county.countyData.countyName}: Work Day Over For Population.");
        }

        private static void KeepWorkingAtCountyImprovement(CountyPopulation countyPopulation)
        {
            if (CheckLoyaltyWithSkillCheck(countyPopulation) != true)
            {
                countyPopulation.UpdateActivity(AllEnums.Activities.Idle);
            }
        }
        private static void CompleteConstruction(CountyPopulation countyPopulation)
        {
            SkillData skillData = new();
            if (skillData.Check(countyPopulation.skills[AllEnums.Skills.Construction].skillLevel))
            {
                countyPopulation.CurrentCountyImprovment.CurrentAmountOfConstruction
                    += Globals.Instance.dailyConstructionAmount + Globals.Instance.dailyConstructionAmountBonus;
            }
            else
            {
                countyPopulation.CurrentCountyImprovment.CurrentAmountOfConstruction
                    += Globals.Instance.dailyConstructionAmount;
            }
        }

        // Goes through all the population and adds a set number to research.
        // It should check what they are doing and try to add that research then if they aren't doing anything
        // it should add to a random research that isn't done yet.
        // Don't forget about idle heroes researching other things.
        public static void PopulationResearch(County county)
        {
            List<ResearchItemData> researchableResearch = [];

            // Get a list of all the research that isn't done.
            foreach (ResearchItemData researchItemData in county.countyData.factionData.researchItems)
            {
                if (researchItemData.isResearchDone == false)
                {
                    researchableResearch.Add(researchItemData);
                }
            }

            foreach (CountyPopulation countyPopulation in county.countyData.countyPopulationList)
            {
                ResearchItemData whatPopulationIsResearching = null;

                /*
                foreach (ResearchItemData researchItemData in researchableResearch)
                {
                    
                    if (countyPopulation.currentActivity == researchItemData.skill)
                    {
                        whatPopulationIsResearching = researchItemData;
                        GD.Print($"{countyPopulation.firstName} preferred skill is having them research {researchItemData.researchName}");
                    }
                    
                }
                */

                if (whatPopulationIsResearching == null)
                {
                    Random random = new();
                    int randomResearchNumber = random.Next(0, researchableResearch.Count);
                    whatPopulationIsResearching = researchableResearch[randomResearchNumber];
                    /*
                    GD.Print($"{countyPopulation.firstName} is randomly researching: " +
                        $"{whatPopulationIsResearching.researchName}");
                    */
                }

                // Have the banker add the research to the research.
                Banker.AddResearchAmount(whatPopulationIsResearching, Globals.Instance.populationResearchIncrease);
                Banker.IncreaseResearchAmountBonus(countyPopulation, whatPopulationIsResearching, Globals.Instance.populationResearchBonus);
            }
        }

        /*
        // Adjust all of the world population!
        private void AdjustPopulationActivity()
        {
            GD.Print($"{county.countyData.countyName} is adjusting their population activity.");
            // Go through this counties population.
            Activities activities = new();
            foreach (CountyPopulation person in county.countyData.countyPopulationList)
            {
                activities.UpdateCurrent(person, person.nextActivity);
                person.CurrentConstruction = person.NextConstruction;
                person.CurrentWork = person.NextWork;
            }

            // Heroes can research.
            foreach (CountyPopulation hero in county.countyData.herosInCountyList)
            {
                if (hero.token == null)
                {
                    activities.UpdateCurrent(hero, hero.nextActivity);
                }
            }
        }
        */

        // This should be moved to the Resource that Loyalty will be part of once we figure out
        // what catagory Loyalty is.  For example, it isn't a skill, or a perk.
        private static bool CheckLoyaltyWithSkillCheck(CountyPopulation countyPopulation)
        {
            SkillData skillData = new();
            if (skillData.Check(countyPopulation.LoyaltyAdjusted))
            {
                return true;
            }
            return false;
        }

    }
}