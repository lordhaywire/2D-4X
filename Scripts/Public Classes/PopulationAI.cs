using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace
{
    public class PopulationAI
    {
        private readonly int willWorkLoyalty = 20; // The loyalty a population needs to be willing to work.
                                                   // 50 is too high for testing, but might work well for the real game.
        private readonly int foodBeforeScavenge = 500; // Less then this amount will make people scavenge.
        private readonly int remnantsBeforeScavenge = 500; // Less then this amount will make people scavenge.

        private readonly List<CountyPopulation> possibleWorkers = [];
        private readonly List<CountyPopulation> workersToRemove = []; // List to collect county populations to be removed from the possibleWorkers.

        private County county;


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
                        county.countyData.countyResources[countyPopulation.CurrentWork.resourceData.countyResourceType].amount +=
                            Banker.GenerateWorkResourceWithSkillCheck(countyPopulation.CurrentWork
                            , countyPopulation.skills[countyPopulation.CurrentWork.workSkill].skillLevel);
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
                countyPopulation.CurrentConstruction.CurrentAmountOfConstruction
                    += Globals.Instance.dailyConstructionAmount + Globals.Instance.dailyConstructionAmountBonus;
            }
            else
            {
                countyPopulation.CurrentConstruction.CurrentAmountOfConstruction
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
        // This is now a dumb name for this method.
        public void CheckForWork(County county)
        {
            this.county = county;
            Banker banker = new();
            possibleWorkers.Clear(); // Clear the list at the start of each county.
            workersToRemove.Clear();
            AdjustPopulationActivity();
            // This needs to be at the start of the day - Produce other items based on countyimprovement such as extra storage.

            CheckForIdle();

            CheckForPreferredWork();
            CheckForAnyWork();
            CheckForConstruction();
            // Sets people to scavenge.
            CheckForScavengingFood();
            CheckForScavengingScrap();
            banker.CountIdleWorkers(county);
        }

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

        private void CheckForIdle()
        {
            // Go through each person in the county.
            foreach (CountyPopulation countyPopulation in county.countyData.countyPopulationList)
            {
                // Go through everyone and if they are idle, helpful and loyal add them to the possibleWorkers list.
                if (countyPopulation.activity == AllEnums.Activities.Idle
                    && CheckLoyalty(countyPopulation) == true
                    && countyPopulation.CheckForPerk(AllEnums.Perks.Unhelpful) == false)
                {
                    //GD.Print($"{county.countyData.countyName}: {countyPopulation.firstName} is idle, is loyal and is not unhelpful.");
                    possibleWorkers.Add(countyPopulation);
                }
            }
        }

        private void CheckForPreferredWork()
        {
            //GD.Print($"{county.countyData.countyName}: Checking for Preferred Work!");
            foreach (CountyImprovementData countyImprovementData in county.countyData.completedCountyImprovements)
            {
                foreach (CountyPopulation countyPopulation in possibleWorkers)
                {
                    if (countyPopulation.preferredSkill.skill == countyImprovementData.workSkill)
                    {
                        // If they have the preferred skill, they are added to the county improvement
                        // and marked for removal from the possibleWorkers list.
                        if (countyImprovementData.currentWorkers < countyImprovementData.maxWorkers)
                        {
                            countyImprovementData.currentWorkers++;
                            countyPopulation.NextWork = countyImprovementData;
                            workersToRemove.Add(countyPopulation);
                        }
                    }
                }
                RemoveWorkersFromPossibleWorkers();
            }
        }

        private void CheckForAnyWork()
        {
            foreach (CountyImprovementData countyImprovementData in county.countyData.completedCountyImprovements)
            {
                foreach (CountyPopulation countyPopulation in possibleWorkers)
                {
                    if (countyImprovementData.currentWorkers < countyImprovementData.maxWorkers)
                    {
                        countyImprovementData.currentWorkers++;
                        countyPopulation.NextWork = countyImprovementData;
                        workersToRemove.Add(countyPopulation);
                    }
                }
                RemoveWorkersFromPossibleWorkers();
            }
        }

        private void CheckForConstruction()
        {
            foreach (CountyImprovementData countyImprovementData in county.countyData.underConstructionCountyImprovements)
            {
                foreach (CountyPopulation countyPopulation in possibleWorkers)
                {
                    if (countyImprovementData.currentBuilders < countyImprovementData.maxBuilders)
                    {
                        countyImprovementData.currentBuilders++;
                        countyPopulation.NextConstruction = countyImprovementData;
                        workersToRemove.Add(countyPopulation);
                    }
                }
                RemoveWorkersFromPossibleWorkers();
            }
        }
        private void CheckForScavengingFood()
        {
            int amountOfFood = Banker.CountFactionResourceOfType(county.countyData, AllEnums.FactionResourceType.Food);
            //GD.Print($"{county.countyData.countyName} Amount of food: " + amountOfFood);
            EnounghStored(amountOfFood, foodBeforeScavenge);
        }

        private void CheckForScavengingScrap()
        {
            //GD.Print($"{county.countyData.countyName} Amount of scrap: " + county.countyData.resources[AllEnums.CountyResourceType.Remnants].amount);
            EnounghStored(county.countyData.countyResources[AllEnums.CountyResourceType.Remnants].amount, remnantsBeforeScavenge);
        }

        private void EnounghStored(int amountOfStored, int resourceBeforeScavenge)
        {
            Activities activities = new();
            if (amountOfStored < resourceBeforeScavenge)
            {
                foreach (CountyPopulation countyPopulation in possibleWorkers)
                {
                    activities.UpdateNext(countyPopulation, AllEnums.Activities.Scavenge);
                    workersToRemove.Add(countyPopulation);
                }
                RemoveWorkersFromPossibleWorkers();
            }
        }

        private void RemoveWorkersFromPossibleWorkers()
        {
            // Remove the collected items from the possibleWorkers list
            foreach (CountyPopulation worker in workersToRemove)
            {
                possibleWorkers.Remove(worker);
            }
            workersToRemove.Clear();
        }

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
        private bool CheckLoyalty(CountyPopulation countyPopulation)
        {
            if (countyPopulation.LoyaltyAdjusted >= willWorkLoyalty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}