using Godot;

namespace PlayerSpace
{
    public class Banker
    {
        public void CountIdleWorkers(County county)
        {
            int idleWorkers = 0;
            foreach (CountyPopulation person in county.countyData.countyPopulationList)
            {
                if (person.currentActivity == AllEnums.Activities.Idle && person.nextActivity == AllEnums.Activities.Idle)
                {
                    idleWorkers++;
                }
            }
            county.countyData.IdleWorkers = idleWorkers;
        }
        public void AddStoryEventCountyResource(StoryEventData storyEventData)
        {
            GD.Print($"Faction: {storyEventData.eventCounty.countyData.factionData.factionName} is adding " +
                $"{storyEventData.resourceAmount} {storyEventData.resource.resourceName}");
            if (storyEventData.resource.perishable)
            {
                storyEventData.eventCounty.countyData.perishableResources[storyEventData.resource.countyResourceType].amount
                    += storyEventData.resourceAmount;
            }
            TopBarControl.Instance.UpdateTopBarWithCountyResources();
        }

        public static int CountFactionResourceOfType(County county, AllEnums.FactionResourceType type)
        {
            int amount = 0;
            foreach (ResourceData resourceData in county.countyData.perishableResources.Values)
            {
                if (resourceData.factionResourceType == type)
                {
                    amount += resourceData.amount;
                }
            }
            foreach (ResourceData resourceData in county.countyData.nonperishableResources.Values)
            {
                if (resourceData.factionResourceType == type)
                {
                    amount += resourceData.amount;
                }
            }
            return amount;
        }

        public void AddLeaderInfluence(FactionData factionData)
        {
            PerkData perkData = new();
            if (perkData.CheckForPerk(factionData.factionLeader, AllEnums.Perks.LeaderOfPeople) == true)
            {
                factionData.Influence += Globals.Instance.dailyInfluenceGain;
            }
        }

        public void AddHeroResearch(FactionData factionData)
        {
            // We have it going through all the heroes because heroes could be researching in other faction territories.
            foreach (CountyPopulation countyPopulation in factionData.allHeroesList)
            {
                if (countyPopulation.currentActivity == AllEnums.Activities.Research)
                {
                    SkillHandling skillHandling = new();

                    bool passedCheck = skillHandling.Check(countyPopulation.skills[AllEnums.Skills.Research].skillLevel);

                    // This needs to be broken into two different things.  One increased the research
                    // the other checks for a bonus.
                    IncreaseResearcherResearch(countyPopulation, passedCheck);

                    // Only the researchers learn research skill.  Normal population who is just adding a tiny bit of research
                    // does not get a learning check.
                    skillHandling.CheckLearning(countyPopulation, countyPopulation.skills[AllEnums.Skills.Research]);
                }
            }
        }

        private static void IncreaseResearcherResearch(CountyPopulation countyPopulation, bool passedCheck)
        {
            int bonusResearchIncrease = 0;
            if (passedCheck == true)
            {
                bonusResearchIncrease = Globals.Instance.random.Next(1, Globals.Instance.researchIncreaseBonus);
            }
            countyPopulation.CurrentResearchItemData.AmountOfResearchDone
                += Globals.Instance.researcherResearchIncrease + bonusResearchIncrease;

            GD.Print($"Amount of Research Done: {countyPopulation.CurrentResearchItemData.AmountOfResearchDone}");
        }
        public void AddResourceToCounty(County county, AllEnums.CountyResourceType countyResourceType, bool perishable, int amount)
        {

            if (perishable == true)
            {
                county.countyData.perishableResources[countyResourceType].amount += amount;
            }
            else
            {
                county.countyData.nonperishableResources[countyResourceType].amount += amount;
            }
            // Update the top bar if the player has a county selected.
            if (Globals.Instance.SelectedLeftClickCounty == county)
            {
                TopBarControl.Instance.UpdateTopBarWithCountyResources();
            }
        }

        public void ChargeForHero()
        {
            //GD.Print("Player Influence: " + Globals.Instance.playerFactionData.Influence);
            Globals.Instance.playerFactionData.Influence -= Globals.Instance.costOfHero;
        }

        public bool CheckBuildingCost(FactionData factionData, CountyImprovementData countyImprovementData)
        {
            return factionData.Influence >= countyImprovementData.influenceCost;
        }

        // Charge for building and also assign it to the underConstructionList.
        public void ChargeForBuilding(FactionData factionData, CountyImprovementData countyImprovementData)
        {
            factionData.Influence -= countyImprovementData.influenceCost;
        }

        // This needs to be checking county level food.
        public bool CheckEnoughCountyFood(County county)
        {
            int amountOfFood = CountFactionResourceOfType(county, AllEnums.FactionResourceType.Food);
            return amountOfFood >= Globals.Instance.minimumFood;
        }

        public void BuildImprovement(CountyData countyData, CountyImprovementData countyImprovementData)
        {
            countyImprovementData.underConstruction = true;
            countyData.underConstructionCountyImprovements.Add(countyImprovementData);
            GD.Print($"{countyData.factionData.factionName} is building {countyImprovementData.improvementName}.");
        }

        public void FindFoodBuilding(County county, out CountyImprovementData countyImprovementData)
        {
            countyImprovementData = null;

            foreach (CountyImprovementData improvementData in county.countyData.allCountyImprovements)
            {
                if (improvementData.underConstruction || improvementData.isBuilt)
                {
                    //GD.Print($"{improvementData.improvementName} is already being built.");
                    return;
                }
                if (improvementData.resourceData.factionResourceType == AllEnums.FactionResourceType.Food)
                {
                    //GD.Print($"{factionData.factionName} found {improvementData.improvementName} in {countyDataItem.countyName}.");
                    countyImprovementData = improvementData;
                    return;
                }
            }
        }
    }
}

