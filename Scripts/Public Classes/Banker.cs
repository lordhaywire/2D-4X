using Godot;
using System;

namespace PlayerSpace
{
    public class Banker
    {
        public static void Work(CountyData countyData, CountyPopulation countyPopulation)
        {
            if (countyData.factionData.isPlayer == true)
            {
                GD.PrintRich($"[color=green]{countyPopulation.firstName} is working at {countyPopulation.currentCountyImprovement.improvementName}[/color]");
            }
            if (countyPopulation.currentCountyImprovement.countyResourceType != AllEnums.CountyResourceType.None
                && countyPopulation.currentCountyImprovement.factionResourceType != AllEnums.FactionResourceType.None)
            {
                countyData.countyResources[countyPopulation.currentCountyImprovement.countyResourceType].Amount +=
                    GenerateWorkResourceWithSkillCheck(countyPopulation);
            }
            else if(countyPopulation.currentCountyImprovement.factionResourceType 
                == AllEnums.FactionResourceType.Research)
            {
                GD.Print($"{countyPopulation.firstName} is pointlessly working at a research office.");
            }
            else
            {
                GD.Print($"{countyPopulation.firstName} is working at some place that is producing nothing.");
            }
        }
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

        public static void GenerateScavengedResources(CountyData countyData, CountyPopulation countyPopulation)
        {
            int randomResourceNumber = Globals.Instance.random.Next(0, 2);
            
            if (randomResourceNumber == 0)
            {
                if (countyData.CheckEnoughCountyScavengables(AllEnums.CountyResourceType.CannedFood) == false)
                {
                    return;
                }
                int amountGenerated = GenerateScavengedResourceWithSkillCheck(countyPopulation);
                int amount = Mathf.Min(amountGenerated, countyData.scavengableCannedFood);
                AddResourceToCounty(countyData, AllEnums.CountyResourceType.CannedFood, amount);
                countyData.RemoveResourceFromAvailableCountyTotals(AllEnums.CountyResourceType.CannedFood, amount);
            }
            else
            {
                if (countyData.CheckEnoughCountyScavengables(AllEnums.CountyResourceType.Remnants) == false)
                {
                    return;
                }
                int amountGenerated = GenerateScavengedResourceWithSkillCheck(countyPopulation);
                int amount = Mathf.Min(amountGenerated, countyData.scavengableRemnants);
                AddResourceToCounty(countyData, AllEnums.CountyResourceType.Remnants, amount);
                countyData.RemoveResourceFromAvailableCountyTotals(AllEnums.CountyResourceType.Remnants, amount);
            }
            // Learning skillcheck.
            // Just for testing it is set to fast.
            SkillData.CheckLearning(countyPopulation, countyPopulation.skills[AllEnums.Skills.Scavenge], AllEnums.LearningSpeed.fast);
        }

        public static int GenerateWorkResourceWithSkillCheck(CountyPopulation countyPopulation)
        {
            CountyImprovementData countyImprovementData = countyPopulation.currentCountyImprovement;
            int skillLevel = countyPopulation.skills[countyPopulation.currentCountyImprovement.workSkill].skillLevel;
            int amount;
            if (SkillData.Check(countyPopulation, skillLevel, countyPopulation.skills[countyPopulation.currentCountyImprovement.workSkill].attribute, false) == true)
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
        public static int GenerateScavengedResourceWithSkillCheck(CountyPopulation countyPopulation)
        {
            int skillLevel = countyPopulation.skills[AllEnums.Skills.Scavenge].skillLevel;
            int amount;
            if (SkillData.Check(countyPopulation, skillLevel, countyPopulation.skills[AllEnums.Skills.Scavenge].attribute, false) == true)
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
            if (SkillData.Check(countyPopulation, countyPopulation.skills[researchItemData.skill].skillLevel
                , countyPopulation.skills[researchItemData.skill].attribute, false) == true)
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
                storyEventData.eventCounty.countyData.countyResources[storyEventData.resource.countyResourceType].Amount
                    += storyEventData.resourceAmount;
            }
            TopBarControl.Instance.UpdateResourceLabels();
        }

        // This is currently unused.  I am pretty sure we don't need this.
        public static int CountCountyResourceOfType(CountyData countyData, AllEnums.CountyResourceType resourceType)
        {
            int amount = 0;
            foreach (CountyResourceData resourceData in countyData.countyResources.Values)
            {
                if (resourceData.countyResourceType == resourceType)
                {
                    amount += resourceData.Amount;
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
                    bool passedCheck = SkillData.Check(countyPopulation, countyPopulation.skills[AllEnums.Skills.Research].skillLevel
                        , countyPopulation.skills[AllEnums.Skills.Research].attribute, false);

                    // This needs to be broken into two different things.  One increases the research
                    // the other checks for a bonus.
                    IncreaseResearcherResearch(countyPopulation, passedCheck);

                    // Only the researchers learn research skill.  Normal population who is just adding a tiny bit of research
                    // does not get a learning check.
                    SkillData.CheckLearning(countyPopulation, countyPopulation.skills[AllEnums.Skills.Research]
                        , AllEnums.LearningSpeed.medium);
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

        public static void AddResourceToCounty(CountyData countyData, AllEnums.CountyResourceType countyResourceType, int amount)
        {
            countyData.countyResources[countyResourceType].Amount += amount;

            //TopBarControl.Instance.UpdateResourceLabels(); 
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
    }
}

