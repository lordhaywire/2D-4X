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
        private readonly int scrapBeforeScavenge = 500; // Less then this amount will make people scavenge.

        private readonly List<CountyPopulation> possibleWorkers = [];
        private readonly List<CountyPopulation> workersToRemove = []; // List to collect county populations to be removed from the possibleWorkers.

        private County county;

        // Goes through all the population and adds a set number to research.
        // It should check what they are doing and try to add that research then if they aren't doing anything
        // it should add to a random research that isn't done yet.

        // Don't forget about idle heroes researching other things.
        public void PopulationResearch(County county)
        {
            List<ResearchItemData> researchableResearch = [];
            Banker banker = new();

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

                foreach (ResearchItemData researchItemData in researchableResearch)
                {
                    /*
                    if (countyPopulation.currentActivity == researchItemData.skill)
                    {
                        whatPopulationIsResearching = researchItemData;
                        GD.Print($"{countyPopulation.firstName} preferred skill is having them research {researchItemData.researchName}");
                    }
                    */
                }

                if (whatPopulationIsResearching == null)
                {
                    Random random = new();
                    int randomResearchNumber = random.Next(0, researchableResearch.Count);
                    whatPopulationIsResearching = researchableResearch[randomResearchNumber];
                    GD.Print($"{countyPopulation.firstName} is randomly researching: " +
                        $"{whatPopulationIsResearching.researchName}");
                }

                // Have the banker add the research to the research.
                banker.AddResearchAmount(whatPopulationIsResearching, Globals.Instance.populationResearchIncrease);
                banker.IncreaseResearchAmountBonus(countyPopulation, whatPopulationIsResearching, Globals.Instance.populationResearchBonus);
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
                PerkData perkData = new();
                // Go through everyone and if they are idle, helpful and loyal add them to the possibleWorkers list.
                if (countyPopulation.currentActivity == AllEnums.Activities.Idle
                    && CheckLoyalty(countyPopulation) == true
                    && perkData.CheckForPerk(countyPopulation, AllEnums.Perks.Unhelpful) == false)
                {
                    GD.Print($"{county.countyData.countyName}: {countyPopulation.firstName} is idle, is loyal and is not unhelpful.");
                    possibleWorkers.Add(countyPopulation);
                }
            }
        }

        private void CheckForPreferredWork()
        {
            GD.Print($"{county.countyData.countyName}: Checking for Preferred Work!");
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
            int amountOfFood = county.countyData.perishableResources[AllEnums.CountyResourceType.Fish].amount
                + county.countyData.perishableResources[AllEnums.CountyResourceType.Vegetables].amount;
            GD.Print($"{county.countyData.countyName} Amount of food: " + amountOfFood);
            EnounghStored(amountOfFood, foodBeforeScavenge);
        }

        private void CheckForScavengingScrap()
        {
            GD.Print($"{county.countyData.countyName} Amount of scrap: " + county.countyData.nonperishableResources[AllEnums.CountyResourceType.Scrap].amount);
            EnounghStored(county.countyData.nonperishableResources[AllEnums.CountyResourceType.Scrap].amount, scrapBeforeScavenge);
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

        private bool CheckLoyalty(CountyPopulation countyPopulation)
        {
            if (countyPopulation.loyaltyAttribute >= willWorkLoyalty)
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