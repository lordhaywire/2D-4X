using Godot;
using System;

namespace PlayerSpace
{
    public class Banker
    {
        public static void Work(CountyData countyData, CountyPopulation countyPopulation)
        {
            /*
            if (countyData.factionData.isPlayer == true)
            {
                GD.PrintRich($"[color=green]{countyPopulation.firstName} is working at {countyPopulation.currentCountyImprovement.improvementName}[/color]");
            }
            */
            if (countyPopulation.currentCountyImprovement.countyResourceType != AllEnums.CountyResourceType.None
                && countyPopulation.currentCountyImprovement.factionResourceType != AllEnums.FactionResourceType.None)
            {
                countyData.countyResources[countyPopulation.currentCountyImprovement.countyResourceType].Amount +=
                    GenerateWorkResourceWithSkillCheck(countyPopulation);
            }
            else if (countyPopulation.currentCountyImprovement.factionResourceType
                == AllEnums.FactionResourceType.Research)
            {
                GD.Print($"{countyPopulation.firstName} is pointlessly working at a research office.");
            }
            else
            {
                GD.Print($"{countyPopulation.firstName} is working at some place that is producing nothing.");
            }
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

        public static void IncreaseResearchAmountBonus(CountyPopulation countyPopulation
            , ResearchItemData researchItemData)
        {
            if (SkillData.Check(countyPopulation, countyPopulation.skills[researchItemData.skill].skillLevel
                , countyPopulation.skills[researchItemData.skill].attribute, false) == true)
            {
                researchItemData.AmountOfResearchDone 
                    += Globals.Instance.populationResearchIncrease + Globals.Instance.populationResearchBonus;
            }
            else
            {
                researchItemData.AmountOfResearchDone += Globals.Instance.populationResearchIncrease;
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

        /// <summary>
        /// Do a skill check of the person working at the research office, and increase the research.
        /// Includes skill learning.
        /// </summary>
        /// <param name="countyPopulation"></param>
        public static void AddResearchForOfficeResearch(CountyPopulation countyPopulation)
        {
            // Perform the research skill check.
            bool passedCheck = SkillData.Check(countyPopulation, countyPopulation.skills[AllEnums.Skills.Research].skillLevel,
                countyPopulation.skills[AllEnums.Skills.Research].attribute, false);

            // Increase research progress and check learning.
            IncreaseResearcherResearch(countyPopulation, passedCheck);
        }

        /// <summary>
        /// We have it go through all the heroes because heroes could be researching in other faction territories.
        /// </summary>
        /// <param name="factionData"></param>
        public static void CheckForHeroResearch(FactionData factionData)
        {
            foreach (CountyPopulation countyPopulation in factionData.allHeroesList)
            {
                // Skip to the next hero if there is no current research.
                if (countyPopulation.currentResearchItemData == null)
                {
                    continue;
                }

                // Stop researching if the current research is done and skip to the next hero.
                if (countyPopulation.currentResearchItemData.isResearchDone)
                {
                    StopHeroResearcherFromResearching(countyPopulation);
                    continue;
                }

                // Perform the research skill check.
                bool passedCheck = SkillData.Check(countyPopulation, countyPopulation.skills[AllEnums.Skills.Research].skillLevel,
                    countyPopulation.skills[AllEnums.Skills.Research].attribute, false);

                // Increase research progress and check learning.
                IncreaseResearcherResearch(countyPopulation, passedCheck);

                // Only researchers learn the research skill. Normal population does not.
                SkillData.CheckLearning(countyPopulation, countyPopulation.skills[AllEnums.Skills.Research], AllEnums.LearningSpeed.medium);

                // Re-check if the research is done after progress update and stop researching if it is.
                if (countyPopulation.currentResearchItemData.isResearchDone)
                {
                    countyPopulation.currentResearchItemData.CompleteResearch();
                    EventLog.Instance.AddLog($"{countyPopulation.currentResearchItemData.researchName} has been completed.");
                    StopHeroResearcherFromResearching(countyPopulation);
                }
            }
        }

        
        private static void StopHeroResearcherFromResearching(CountyPopulation countyPopulation)
        {
            countyPopulation.RemoveFromResearch();
            // This needs to be below RemoveFromResearch because that sets the population's activity to work.
            countyPopulation.UpdateActivity(AllEnums.Activities.Idle);
            if (countyPopulation.factionData.isPlayer)
            {
                ResearchControl.Instance.assignedResearchers.Remove(countyPopulation);
            }
        }

        private static void IncreaseResearcherResearch(CountyPopulation countyPopulation, bool passedCheck)
        {
            int bonusResearchIncrease = 0;
            if (passedCheck == true)
            {
                bonusResearchIncrease = Globals.Instance.random.Next(1, Globals.Instance.researchIncreaseBonus);
            }
            int researchAmount = Globals.Instance.researcherResearchIncrease + bonusResearchIncrease;
            EventLog.Instance.AddLog($"Amount of research {countyPopulation.firstName} did: {researchAmount}");
            GD.Print($"Amount of research {countyPopulation.firstName} did: {researchAmount}");
            countyPopulation.currentResearchItemData.AmountOfResearchDone
                += researchAmount;
            //GD.Print($"Amount of Research Done: {countyPopulation.CurrentResearchItemData.AmountOfResearchDone}");
        }

        public static void AddResourceToCounty(CountyData countyData, AllEnums.CountyResourceType countyResourceType, int amount)
        {
            countyData.countyResources[countyResourceType].Amount += amount;

            //TopBarControl.Instance.UpdateResourceLabels(); 
        }

        public static void ChargeForHero(FactionData factionData)
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

