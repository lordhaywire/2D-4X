using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    public class Banker
    {
        /// <summary>
        /// Shouldn't check learning be in GenerateWorkAmoutWithSkillCheck?
        /// This generates the work amount per person which is divide by the number of resources the 
        /// county improvement produces and then subtracts the divided amount from the work cost of each 
        /// resource. If the result is negative it does it again until it is positive or zero.
        /// Each time is goes through the loop it generates 1 of the resource.
        /// </summary>
        /// <param name="countyData"></param>
        /// <param name="countyPopulation"></param>
        public static void ApplyWorkPerPerson(CountyData countyData, CountyPopulation countyPopulation)
        {
            if (countyPopulation.currentCountyImprovement == null)
            {
                return;
            }
            GD.Print($"{countyData.countyName} Someone is working at {countyPopulation.currentCountyImprovement.improvementName}");
            countyPopulation.currentCountyImprovement.allDailyWorkAmountAtImprovementCompleted
                += GenerateWorkAmountWithSkillCheck(countyPopulation);
            GD.Print("All Daily Work Amount At Improvement Completed: " + countyPopulation.currentCountyImprovement.allDailyWorkAmountAtImprovementCompleted);
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
                if (countyData.CheckEnoughCountyScavengables(AllEnums.CountyGoodType.CannedFood) == false)
                {
                    return;
                }
                int amountGenerated = GenerateScavengedResourceWithSkillCheck(countyPopulation);
                int amount = Mathf.Min(amountGenerated, countyData.scavengableCannedFood);
                AddCountyResource(countyData, AllEnums.CountyGoodType.CannedFood, amount);
                countyData.RemoveResourceFromAvailableCountyTotals(AllEnums.CountyGoodType.CannedFood, amount);
            }
            else
            {
                if (countyData.CheckEnoughCountyScavengables(AllEnums.CountyGoodType.Remnants) == false)
                {
                    return;
                }
                int amountGenerated = GenerateScavengedResourceWithSkillCheck(countyPopulation);
                int amount = Mathf.Min(amountGenerated, countyData.scavengableRemnants);
                AddCountyResource(countyData, AllEnums.CountyGoodType.Remnants, amount);
                countyData.RemoveResourceFromAvailableCountyTotals(AllEnums.CountyGoodType.Remnants, amount);
            }
            // Learning skillcheck.
            // Just for testing it is set to fast.
            SkillData.CheckLearning(countyPopulation, countyPopulation.skills[AllEnums.Skills.Scavenge], AllEnums.LearningSpeed.fast);
        }

        /// <summary>
        /// Return the work amount for a single person that should be subtracted from the resource cost.
        /// </summary>
        /// <param name="countyPopulation"></param>
        /// <returns></returns>
        public static int GenerateWorkAmountWithSkillCheck(CountyPopulation countyPopulation)
        {
            //CountyImprovementData countyImprovementData = countyPopulation.currentCountyImprovement;
            int skillLevel = countyPopulation.skills[countyPopulation.currentCountyImprovement.workSkill].skillLevel;
            int workAmount;
            if (SkillData.Check(countyPopulation, skillLevel, countyPopulation.skills[countyPopulation.currentCountyImprovement.workSkill].attribute, false) == true)
            {
                workAmount = Globals.Instance.dailyWorkAmount + Globals.Instance.dailyWorkAmountBonus;
                return workAmount;
            }
            else
            {
                workAmount = Globals.Instance.dailyWorkAmount;
                return workAmount;
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

        public static void IncreaseResearchAmountWithBonus(CountyPopulation countyPopulation
            , ResearchItemData researchItemData, List<ResearchItemData> researchableResearch)
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
            if (researchItemData.CheckIfResearchDone())
            {
                researchableResearch.Remove(researchItemData);
            }
        }

        public void AddStoryEventCountyResource(StoryEventData storyEventData)
        {
            //// GD.Print($"Faction: {storyEventData.eventCounty.countyData.factionData.factionName} is adding " +
            //   $"{storyEventData.resourceAmount} {storyEventData.resource.name}");
            if (storyEventData.good.perishable == AllEnums.Perishable.Perishable)
            {
                storyEventData.eventCounty.countyData.goods[storyEventData.good.countyGoodType].Amount
                    += storyEventData.resourceAmount;
            }
            TopBarControl.Instance.UpdateResourceLabels();
        }

        public void AddStorageToCounty(CountyData countyData, CountyImprovementData countyImprovementData)
        {
            if (countyImprovementData.CheckIfStorageImprovement())
            {
                foreach (KeyValuePair<GoodData, ProductionData> keyValuePair in countyImprovementData.outputGoods)
                {
                    if (keyValuePair.Key.countyGoodType == AllEnums.CountyGoodType.StorageNonperishable)
                    {
                        countyData.nonperishableStorage += keyValuePair.Value.storageAmount;
                        // GD.Print($"{countyData.countyName} now has {countyData.nonperishableStorage} nonperishable storage.");
                    }
                    else
                    {
                        countyData.perishableStorage += keyValuePair.Value.storageAmount;
                        // GD.Print($"{countyData.countyName} now has {countyData.perishableStorage} perishable storage.");
                    }
                }
            }
        }

        // This is currently unused.  I am pretty sure we don't need this.
        /*
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
        */
        public void AddLeaderInfluence(FactionData factionData)
        {
            factionData.factionGood[AllEnums.FactionGoodType.Influence].Amount
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
                if (countyPopulation.currentResearchItemData.CheckIfResearchDone())
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
                if (countyPopulation.currentResearchItemData.CheckIfResearchDone())
                {
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
            EventLog.Instance.AddLog($"{countyPopulation.firstName} - " +
                $"{TranslationServer.Translate(countyPopulation.currentResearchItemData.researchName)}: {researchAmount}");
            //// GD.Print($"Amount of research {countyPopulation.firstName} did: {researchAmount}");
            // This will trigger the getter setting and mark the research as complete if the Amount is
            // higher or equal to the cost.
            countyPopulation.currentResearchItemData.AmountOfResearchDone
                += researchAmount;
            //// GD.Print($"Amount of Research Done: {countyPopulation.CurrentResearchItemData.AmountOfResearchDone}");
        }

        public static void AddCountyResource(CountyData countyData, AllEnums.CountyGoodType countyResourceType, int amount)
        {
            countyData.goods[countyResourceType].Amount += amount;
        }

        public static void ChargeForHero(FactionData factionData)
        {
            //// GD.Print("Player Influence: " + Globals.Instance.playerFactionData.Influence);
            factionData.factionGood[AllEnums.FactionGoodType.Influence].Amount
                -= Globals.Instance.costOfHero;
        }

        public bool CheckBuildingCost(FactionData factionData, CountyData countyData
            , CountyImprovementData countyImprovementData)
        {
            // GD.Print("Checking Building Cost...");
            // This is here so the county improvement can be shown in the Research pannel.
            if (countyData == null)
            {
                // GD.Print("County Data is null so Check Building Cost is skipped.");
                return false;
            }

            if (countyImprovementData.countyResourceConstructionCost != null)
            {
                foreach (KeyValuePair<GoodData, int> keyValuePair in countyImprovementData.countyResourceConstructionCost)
                {
                    AllEnums.CountyGoodType resourceType = keyValuePair.Key.countyGoodType;
                    if (countyData.goods[resourceType].Amount < keyValuePair.Value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // Charge for building and also assign it to the underConstructionList.
        public void ChargeForBuilding(FactionData factionData, CountyData countyData
            , CountyImprovementData countyImprovementData)
        {
            if (countyImprovementData.countyResourceConstructionCost != null)
            {
                foreach (KeyValuePair<GoodData, int> keyValuePair
                    in countyImprovementData.countyResourceConstructionCost)
                {
                    AllEnums.CountyGoodType resourceType = keyValuePair.Key.countyGoodType;
                    countyData.goods[resourceType].Amount -= keyValuePair.Value;
                    /* GD.Print($"{countyImprovementData.improvementName} costs " +
                        $"{countyImprovementData.countyResourceConstructionCost[keyValuePair.Key]} and" +
                    $" was charged to {countyData.countyName} those cost was : {countyData.countyResources[resourceType].name} {keyValuePair.Value}");
                    */
                }
            }
        }

        public void TestiGoods(Godot.Collections.Dictionary<Variant, ProductionData> testDic)
        {
            foreach(KeyValuePair<Variant, ProductionData> keyValuePair in testDic )
            {
                //GD.Print("WhateveR: " + keyValuePair.Key.GoodName);
            }
        }
        /// <summary>
        /// This should go through the list of completed county improvements and does the math
        /// to generate the goods produced.
        /// </summary>
        /// <param name="countyData"></param>
        public static void CalculateWorkToGoodsProduction(CountyData countyData)
        {
            foreach (CountyImprovementData countyImprovementData in countyData.completedCountyImprovements)
            {
                // This is checking max workers because it needs to skip county improvements that don't have
                // workers, such as a warehouse.
                //TestiGoods(countyImprovementData.countyOutputGoods);
                if (countyImprovementData.outputGoods?.Count > 0
                    && countyImprovementData.maxWorkers > 0)
                {
                    foreach (KeyValuePair<GoodData, ProductionData> keyValuePair in countyImprovementData.outputGoods)
                    {
                        // Reset todays goods amount generated before it does all the calculations.
                        // It needs to keep this number for the player UI until it hits PopulationAI.WorkDayOverForPopulation.
                        keyValuePair.Value.todaysGoodsAmountGenerated = 0;

                        // The work amount isn't divided by the number of resources.  The work amount
                        // is applied to each resource and the amount of goods generated should reflect that.
                        keyValuePair.Value.workAmount
                            += countyImprovementData.allDailyWorkAmountAtImprovementCompleted;

                        GD.Print($"{countyData.countyName} : {countyImprovementData.improvementName} " +
                            $"- Work Amount For Each Resource For Today: {keyValuePair.Value.workAmount}");

                        if (keyValuePair.Value.workCost <= keyValuePair.Value.workAmount)
                        {
                            keyValuePair.Value.todaysGoodsAmountGenerated
                                = keyValuePair.Value.workAmount / keyValuePair.Value.workCost;
                            keyValuePair.Value.workAmount = keyValuePair.Value.workAmount % keyValuePair.Value.workCost;
                        }

                        GD.Print($"{countyData.countyName} {countyImprovementData.improvementName} todays goods " +
                            $"generated: {keyValuePair.Value.todaysGoodsAmountGenerated}");

                        AddCountyResource(countyData, keyValuePair.Key.countyGoodType, keyValuePair.Value.todaysGoodsAmountGenerated);
                    }
                    // Reset all the county improvement work so that the next day it will generate with new skill checks.
                    countyImprovementData.allDailyWorkAmountAtImprovementCompleted = 0;
                }
            }
        }
    }
}

