using Godot;

namespace PlayerSpace
{
    public class Banker
    {
        public void AddResearchAmount(ResearchItemData researchItemData, int amount)
        {
            researchItemData.AmountOfResearchDone += amount;
        }

        // This should be updated with canned goods instead of vegetables.
        public void GenerateScavengedResources(County county, CountyPopulation countyPopulation)
        {
            int randomResourceNumber = Globals.Instance.random.Next(0, 2);
            if (randomResourceNumber == 0)
            {
                AddResourceToCounty(county, AllEnums.CountyResourceType.Vegetables, true
                    , GenerateScavengedResourceWithSkillCheck(countyPopulation.skills[AllEnums.Skills.Scavenge].skillLevel));
            }
            else
            {
                AddResourceToCounty(county, AllEnums.CountyResourceType.Scrap, false
                    , GenerateScavengedResourceWithSkillCheck(countyPopulation.skills[AllEnums.Skills.Scavenge].skillLevel));
            }
        }

        public int GenerateScavengedResourceWithSkillCheck(int skillLevel)
        {
            SkillData skillData = new();
            int amount;
            if (skillData.Check(skillLevel) == true)
            {
                amount = Globals.Instance.dailyScavengedAmount + Globals.Instance.dailyScavengedAmountBonus;
            }
            else
            {
                amount = Globals.Instance.dailyScavengedAmount;
            }
            return amount;
        }

        // This is weird.  We aren't rolling for a bonus amount?  Also why are we passing in the amount?
        public void IncreaseResearchAmountBonus(CountyPopulation countyPopulation
            , ResearchItemData researchItemData, int amount)
        {
            SkillData skillData = new();
            if (skillData.Check(countyPopulation.skills[researchItemData.skill].skillLevel) == true)
            {
                researchItemData.AmountOfResearchDone += amount;
            }
        }

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
            factionData.Influence += Globals.Instance.dailyInfluenceGain + AddLeaderBonusInfluence(factionData);
        }

        // This should probably either in the perk data for the bonus, or it should be a generic perk bonus check.
        // Or both.
        public static int AddLeaderBonusInfluence(FactionData factionData)
        {
            PerkData perkData = new();
            int bonus = 0;
            if (perkData.CheckForPerk(factionData.factionLeader, AllEnums.Perks.LeaderOfPeople) == true)
            {
                bonus = Globals.Instance.leaderOfPeopleInfluenceBonus;
                return bonus;
            }
            return bonus;
        }

        public void AddHeroResearch(FactionData factionData)
        {
            // We have it go through all the heroes because heroes could be researching in other faction territories.
            foreach (CountyPopulation countyPopulation in factionData.allHeroesList)
            {
                if (countyPopulation.currentActivity == AllEnums.Activities.Research)
                {
                    SkillData skillData = new();

                    bool passedCheck = skillData.Check(countyPopulation.skills[AllEnums.Skills.Research].skillLevel);

                    // This needs to be broken into two different things.  One increases the research
                    // the other checks for a bonus.
                    IncreaseResearcherResearch(countyPopulation, passedCheck);

                    // Only the researchers learn research skill.  Normal population who is just adding a tiny bit of research
                    // does not get a learning check.
                    skillData.CheckLearning(countyPopulation, countyPopulation.skills[AllEnums.Skills.Research]);
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
        public bool CheckEnoughCountyFactionResource(County county, AllEnums.FactionResourceType resource)
        {
            int amountOfFood = CountFactionResourceOfType(county, resource);
            return amountOfFood >= Globals.Instance.minimumFood;
        }
    }
}

