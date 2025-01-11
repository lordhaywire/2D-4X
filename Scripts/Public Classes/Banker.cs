using Godot;
using System.Collections.Generic;

namespace PlayerSpace;

public class Banker
{
    // This includes the armies in the county, but it only works while the army is one person.
    public static int CountEveryoneInCounty(CountyData countyData)
    {
        int numberOfPeople = countyData.populationDataList.Count
            + countyData.heroesInCountyList.Count + countyData.visitingHeroList.Count
            + countyData.armiesInCountyList.Count;
        //GD.Print($"{countyData.countyName} has {numberOfPeople} people.");
        return numberOfPeople;
    }

    public static void GenerateScavengedResources(CountyData countyData, PopulationData populationData)
    {
        int randomResourceNumber = Globals.Instance.random.Next(0, 2);

        if (randomResourceNumber == 0)
        {
            if (countyData.CheckEnoughCountyScavengables(AllEnums.CountyGoodType.CannedFood) == false)
            {
                return;
            }
            int amountGenerated = GenerateScavengedResourceWithSkillCheck(populationData);
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
            int amountGenerated = GenerateScavengedResourceWithSkillCheck(populationData);
            int amount = Mathf.Min(amountGenerated, countyData.scavengableRemnants);
            AddCountyResource(countyData, AllEnums.CountyGoodType.Remnants, amount);
            countyData.RemoveResourceFromAvailableCountyTotals(AllEnums.CountyGoodType.Remnants, amount);
        }
    }

    // This should be in PopulationWork.
    public static int GenerateScavengedResourceWithSkillCheck(PopulationData populationData)
    {
        int skillLevel = populationData.skills[AllEnums.Skills.Scavenge].skillLevel;
        int amount;
        if (SkillData.Check(populationData, skillLevel
            , populationData.skills[AllEnums.Skills.Scavenge].attribute, false) == true)
        {
            amount = Globals.Instance.dailyScavengedAmount + Globals.Instance.dailyScavengedAmountBonus;
        }
        else
        {
            amount = Globals.Instance.dailyScavengedAmount;
        }
        return amount;
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
        TopBarControl.Instance.UpdateTopBarGoodLabels();
    }

    public void AddLeaderInfluence(FactionData factionData)
    {
        factionData.factionGoods[AllEnums.FactionGoodType.Influence].Amount
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
    /// <param name="populationData"></param>
    public static void AddResearchForOfficeResearch(PopulationData populationData)
    {
        // Perform the research skill check.
        bool passedCheck = SkillData.Check(populationData, populationData.skills[AllEnums.Skills.Research].skillLevel,
            populationData.skills[AllEnums.Skills.Research].attribute, false);

        // Increase research progress and check learning.
        IncreaseResearcherResearch(populationData, passedCheck);
    }

    
    /*
    private static void StopHeroResearcherFromResearching(PopulationData populationData)
    {
        populationData.RemoveFromResearch();
        // This needs to be below RemoveFromResearch because that sets the population's activity to work.
        populationData.UpdateActivity(AllEnums.Activities.Idle);
        if (populationData.factionData.isPlayer)
        {
            ResearchControl.Instance.assignedResearchers.Remove(populationData);
        }
    }
    */
    private static void IncreaseResearcherResearch(PopulationData populationData, bool passedCheck)
    {
        int bonusResearchIncrease = 0;
        if (passedCheck == true)
        {
            bonusResearchIncrease = Globals.Instance.random.Next(1, Globals.Instance.researchIncreaseBonus);
        }
        int researchAmount = Globals.Instance.researcherResearchIncrease + bonusResearchIncrease;
        //if (populationData.factionData == Globals.Instance.playerFactionData)
        //{
            EventLog.Instance.AddLog($"{populationData.location} " +
                $"{populationData.firstName} - {populationData.interestData.name} " +
                $"{TranslationServer.Translate(populationData.currentResearchItemData.researchName)}" +
                $": {researchAmount}");
        //}
        //// GD.Print($"Amount of research {populationData.firstName} did: {researchAmount}");
        // This will trigger the getter setting and mark the research as complete if the Amount is
        // higher or equal to the cost.
        populationData.currentResearchItemData.AmountOfResearchDone
            += researchAmount;
        //// GD.Print($"Amount of Research Done: {populationData.CurrentResearchItemData.AmountOfResearchDone}");
    }

    public static void AddCountyResource(CountyData countyData, AllEnums.CountyGoodType countyResourceType, int amount)
    {
        countyData.goods[countyResourceType].Amount += amount;
    }

    public static void ChargeForHero(FactionData factionData)
    {
        //// GD.Print("Player Influence: " + Globals.Instance.playerFactionData.Influence);
        factionData.factionGoods[AllEnums.FactionGoodType.Influence].Amount
            -= Globals.Instance.costOfHero;
    }

    public static bool CheckBuildingCost(CountyData countyData, CountyImprovementData countyImprovementData)
    {
        // GD.Print("Checking Building Cost...");
        // This is here so the county improvement can be shown in the Research pannel.
        if (countyData == null)
        {
            // GD.Print("County Data is null so Check Building Cost is skipped.");
            return false;
        }

        if (countyImprovementData.goodsConstructionCost != null)
        {
            foreach (KeyValuePair<GoodData, int> keyValuePair in countyImprovementData.goodsConstructionCost)
            {
                AllEnums.GoodType goodType = keyValuePair.Key.goodType;
                AllEnums.CountyGoodType countyResourceType = keyValuePair.Key.countyGoodType;
                AllEnums.FactionGoodType factionResourceType = keyValuePair.Key.factionGoodType;

                // If the good type is a county good, or both it checks the county amount of the resource
                // currently only Remnants is both and the player is taking from the county amount.
                if (goodType == AllEnums.GoodType.CountyGood || goodType == AllEnums.GoodType.Both)
                {
                    if (countyData.goods[countyResourceType].Amount < keyValuePair.Value)
                    {
                        return false;
                    }
                }
                else
                {
                    if (countyData.factionData.factionGoods[factionResourceType].Amount < keyValuePair.Value)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    // Charge for building and also assign it to the underConstructionList.
    public static void ChargeForBuilding(CountyData countyData, CountyImprovementData countyImprovementData)
    {
        if (countyImprovementData.goodsConstructionCost != null)
        {
            foreach (KeyValuePair<GoodData, int> keyValuePair
                in countyImprovementData.goodsConstructionCost)
            {
                AllEnums.GoodType goodType = keyValuePair.Key.goodType;
                AllEnums.CountyGoodType countyResourceType = keyValuePair.Key.countyGoodType;
                AllEnums.FactionGoodType factionResourceType = keyValuePair.Key.factionGoodType;

                // If the good type is a county good, or both it checks the county amount of the resource
                // currently only Remnants is both and the player is taking from the county amount.
                if (goodType == AllEnums.GoodType.CountyGood || goodType == AllEnums.GoodType.Both)
                {
                    countyData.goods[countyResourceType].Amount -= keyValuePair.Value;
                }
                else
                {
                    countyData.factionData.factionGoods[factionResourceType].Amount -= keyValuePair.Value;
                }
                
                /* GD.Print($"{countyImprovementData.improvementName} costs " +
                    $"{countyImprovementData.countyResourceConstructionCost[keyValuePair.Key]} and" +
                $" was charged to {countyData.countyName} those cost was : {countyData.countyResources[resourceType].name} {keyValuePair.Value}");
                */
            }
        }
    }
}

