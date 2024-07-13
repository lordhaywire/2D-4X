using Godot;

namespace PlayerSpace
{
    public class Banker
    {
        public static void AddResearchAmount(ResearchItemData researchItemData, int amount)
        {
            researchItemData.AmountOfResearchDone += amount;
        }

        // This includes the armies in the county, but it only works while the army is one person.
        public static int CountEveryoneInCounty(CountyData countyData)
        {
            int numberOfPeople = countyData.countyPopulationList.Count()
                + countyData.herosInCountyList.Count() + countyData.visitingHeroList.Count()
                + countyData.armiesInCountyList.Count();
            //GD.Print($"{countyData.countyName} has {numberOfPeople} people.");
            return numberOfPeople;
        }

        public static void GenerateScavengedResources(County county, CountyPopulation countyPopulation)
        {
            int randomResourceNumber = Globals.Instance.random.Next(0, 2);
            if (randomResourceNumber == 0)
            {
                AddResourceToCounty(county, AllEnums.CountyResourceType.CannedFood
                    , GenerateScavengedResourceWithSkillCheck(countyPopulation.skills[AllEnums.Skills.Scavenge].skillLevel));
            }
            else
            {
                AddResourceToCounty(county, AllEnums.CountyResourceType.Remnants
                    , GenerateScavengedResourceWithSkillCheck(countyPopulation.skills[AllEnums.Skills.Scavenge].skillLevel));
            }
        }

        public static int GenerateWorkResourceWithSkillCheck(CountyImprovementData countyImprovementData, int skillLevel)
        {
            SkillData skillData = new();
            int amount;
            if (skillData.Check(skillLevel) == true)
            {
                amount = countyImprovementData.dailyResourceGenerationAmount + countyImprovementData.dailyResourceGenerationBonus;
                return amount;
            }
            else
            {
                amount = countyImprovementData.dailyResourceGenerationAmount;
                return amount;
            }
        }
        public static int GenerateScavengedResourceWithSkillCheck(int skillLevel)
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
        public static void IncreaseResearchAmountBonus(CountyPopulation countyPopulation
            , ResearchItemData researchItemData, int amount)
        {
            SkillData skillData = new();
            if (skillData.Check(countyPopulation.skills[researchItemData.skill].skillLevel) == true)
            {
                researchItemData.AmountOfResearchDone += amount;
            }
        }


        public void AddStoryEventCountyResource(StoryEventData storyEventData)
        {
            //GD.Print($"Faction: {storyEventData.eventCounty.countyData.factionData.factionName} is adding " +
             //   $"{storyEventData.resourceAmount} {storyEventData.resource.name}");
            if (storyEventData.resource.perishable)
            {
                storyEventData.eventCounty.countyData.countyResources[storyEventData.resource.countyResourceType].amount
                    += storyEventData.resourceAmount;
            }
            TopBarControl.UpdateCountyResources();
        }

        // This is currently unused.  I am pretty sure we don't need this.
        public static int CountCountyResourceOfType(CountyData countyData, AllEnums.CountyResourceType resourceType)
        {
            int amount = 0;
            foreach (CountyResourceData resourceData in countyData.countyResources.Values)
            {
                if (resourceData.countyResourceType == resourceType)
                {
                    amount += resourceData.amount;
                }
            }
            return amount;
        }
        public static int CountFactionResourceOfType(CountyData countyData, AllEnums.FactionResourceType type)
        {
            int amount = 0;
            foreach (CountyResourceData resourceData in countyData.countyResources.Values)
            {
                if (resourceData.factionResourceType == type)
                {
                    amount += resourceData.amount;
                    //GD.Print($"{countyData.countyName} is counting food: {resourceData.name} {resourceData.amount}");
                }
            }
            return amount;
        }

        public void AddLeaderInfluence(FactionData factionData)
        {
            factionData.factionResources[AllEnums.FactionResourceType.Influence].amount 
                += Globals.Instance.dailyInfluenceGain + AddLeaderBonusInfluence(factionData);
        }

        // This should probably either in the perk data for the bonus, or it should be a generic perk bonus check.
        // Or both.
        public static int AddLeaderBonusInfluence(FactionData factionData)
        {
            int bonus = 0;
            if (factionData.factionLeader.CheckForPerk(AllEnums.Perks.LeaderOfPeople) == true)
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
                if (countyPopulation.activity == AllEnums.Activities.Research)
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

            //GD.Print($"Amount of Research Done: {countyPopulation.CurrentResearchItemData.AmountOfResearchDone}");
        }

        public static void AddResourceToCounty(County county, AllEnums.CountyResourceType countyResourceType, int amount)
        {
            county.countyData.countyResources[countyResourceType].amount += amount;

            // Update the top bar if the player has a county selected.
            if (Globals.Instance.SelectedLeftClickCounty == county)
            {
                TopBarControl.UpdateCountyResources();
            }
        }

        public void ChargeForHero(FactionData factionData)
        {
            //GD.Print("Player Influence: " + Globals.Instance.playerFactionData.Influence);
            factionData.factionResources[AllEnums.FactionResourceType.Influence].amount 
                -= Globals.Instance.costOfHero;
        }

        public bool CheckBuildingCost(FactionData factionData, CountyImprovementData countyImprovementData)
        {
            return factionData.factionResources[AllEnums.FactionResourceType.Influence].amount 
                >= countyImprovementData.influenceCost;
        }

        // Charge for building and also assign it to the underConstructionList.
        public void ChargeForBuilding(FactionData factionData, CountyImprovementData countyImprovementData)
        {
            factionData.factionResources[AllEnums.FactionResourceType.Influence].amount 
                -= countyImprovementData.influenceCost;
        }

        // This needs to be checking county level food.
        public bool CheckEnoughCountyFactionResource(County county, AllEnums.FactionResourceType resource)
        {
            int amountOfFood = CountFactionResourceOfType(county.countyData, resource);
            return amountOfFood >= Globals.Instance.minimumFood;
        }
    }
}

