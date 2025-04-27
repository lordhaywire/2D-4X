using System;
using Godot;
using System.Collections.Generic;

namespace PlayerSpace;

public class PopulationWorkEnd
{
    public static void WorkWeekOverForPopulation(
        Godot.Collections.Array<PopulationData> populationDataList)
    {
        foreach (PopulationData populationData in populationDataList)
        {
            switch (populationData.activity)
            {
                case AllEnums.Activities.Build:
                case AllEnums.Activities.Work:
                    PopulationAI.LoyaltyCheckToKeepWorkingAtCountyImprovement(populationData);
                    break;
            }
        }
    }



    public static void WorkDayOverForPopulation(CountyData countyData
        , Godot.Collections.Array<PopulationData> populationDataList)
    {
        foreach (PopulationData populationData in populationDataList)
        {
            /*
            if (countyData.factionData.isPlayer == true)
            {
                GD.PrintRich($"[color=blue]{Clock.Instance.GetDateAndTime()} {populationData.firstName} {populationData.activity}[/color]");
            }
            */
            switch (populationData.activity)
            {
                case AllEnums.Activities.Scavenge:
                    // Skill learning is done in the GenerateScavengedResources.
                    Banker.GenerateScavengedResources(countyData, populationData);

                    // Learning skillcheck.
                    // Just for testing it is set to fast.  The bool doesn't matter for this skill.
                    SkillData.LearningCheck(populationData, true);

                    if (populationData.isHero != true)
                    {
                        populationData.UpdateActivity(AllEnums.Activities.Idle);
                    }
                    break;
                case AllEnums.Activities.Build:
                    // Produce resources based on the countyimprovement
                    ApplyWorkPerPerson(populationData);

                    // Check for Skill Learning.
                    // The bool doesn't matter for this skill.
                    SkillData.LearningCheck(populationData, true);
                    break;
                case AllEnums.Activities.Work:

                    // If the hero hasn't been assigned a currentCountyImprovement yet, get out of here.
                    if(populationData.currentCountyImprovement == null)
                    {
                        break;
                    }
                    // Retrieve the current county improvement for the population.
                    CountyImprovementData countyImprovementData = populationData.currentCountyImprovement;

                    // Check if there are enough input goods to proceed with work.
                    bool hasEnoughInputGoods = Haulmaster.CheckEnoughGoods(countyImprovementData);

                    if (hasEnoughInputGoods)
                    {
                        Haulmaster.DeductStockPiledGoods(countyImprovementData);

                        // Perform work-related processes.
                        ApplyWorkPerPerson(populationData);

                        // Skill learning.
                        SkillData.LearningCheck(populationData, true);
                    }
                    else
                    {
                        // If there isn't enough goods add a day to days employed but idle.
                        populationData.daysEmployedButIdle++;
                    }

                    if (populationData.isHero != true)
                    {
                        KeepWorkingAtCountyImprovement(populationData);
                    }
                    break;
                case AllEnums.Activities.Research:
                    // This is commented out because we are changing research from each office researching
                    // their own project to faction wide projects.

                    // Person working at research office, or hero generates research.

                    //Banker.AddResearchForOfficeResearch(populationData);

                    //PopulationAI.KeepResearching(populationData);
                    break;
                case AllEnums.Activities.Idle:
                    // Give idle people their bonus happiness, this includes heroes that are idle.
                    populationData.AddRandomHappiness(5);
                    break;
                case AllEnums.Activities.Combat:
                    break;
                case AllEnums.Activities.Explore:
                    break;
                case AllEnums.Activities.Move:
                    break;
                case AllEnums.Activities.Recruit:
                    break;
                case AllEnums.Activities.Recruited:
                    GD.Print($"{populationData.firstName} is currently recruited.");
                    break;
                case AllEnums.Activities.Service:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        //// GD.PrintRich($"[rainbow]{countyData.countyName}: Work Day Over For Population.");
    }

    /// <summary>
    /// Uses a loyalty skill check to see if the person wants to keep working at the improvement.  It is a skill check
    /// so it is pure percental chance.
    /// Then it does another check to see if the person is employed but idle and also does another loyalty skill check.
    /// Resets the days employed and employed but idle numbers to zero if they get a check.
    /// </summary>
    /// <param name="populationData"></param>
    private static void KeepWorkingAtCountyImprovement(PopulationData populationData)
    {
        populationData.daysEmployed++;
        if (populationData.daysEmployed > Globals.Instance.daysEmployedBeforeLoyaltyCheck)
        {
            PopulationAI.LoyaltyCheckToKeepWorkingAtCountyImprovement(populationData);
            populationData.daysEmployed = 0;
        }
        if (populationData.currentCountyImprovement != null)
        {
            if (populationData.daysEmployedButIdle > Globals.Instance.daysEmployedIdleBeforeLookingForNewWork)
            {
                PopulationAI.LoyaltyCheckToKeepWorkingAtCountyImprovement(populationData);
                populationData.daysEmployedButIdle = 0;
            }
        }
        GD.Print($"{populationData.firstName} - Days Employed: {populationData.daysEmployed} Days Employed but Idle: " +
            $"{populationData.daysEmployedButIdle}");
    }



    /// <summary>
    /// Shouldn't check learning be in GenerateWorkAmoutWithSkillCheck?
    /// Applies work for each person and adds it to the county improvments All Daily Work Amount At Improvement Completed.
    /// </summary>
    /// <param name="countyData"></param>
    /// <param name="populationData"></param>
    public static void ApplyWorkPerPerson(PopulationData populationData)
    {
        if (populationData.currentCountyImprovement == null)
        {
            return;
        }
        //GD.Print($"{countyData.countyName} Someone is working at {populationData.currentCountyImprovement.improvementName}");
        populationData.currentCountyImprovement.allDailyWorkAmountAtImprovementCompleted
            += GenerateWorkAmountWithSkillCheck(populationData);
        
        GD.Print($"{populationData.location}: " +
            $"{populationData.currentCountyImprovement.improvementName}: " +
            $"All Daily {populationData.activity} Amount At Improvement Completed: "
            + populationData.currentCountyImprovement.allDailyWorkAmountAtImprovementCompleted);
        
    }

    /// <summary>
    /// Return the work amount for a single person that should be subtracted from the resource cost.
    /// </summary>
    /// <param name="populationData"></param>
    /// <returns></returns>
    public static int GenerateWorkAmountWithSkillCheck(PopulationData populationData)
    {
        //CountyImprovementData countyImprovementData = populationData.currentCountyImprovement;
        int skillLevel = populationData.skills[populationData.currentCountyImprovement.workSkill].skillLevel;
        int attributeLevel = populationData.attributes[populationData.skills[populationData.currentCountyImprovement.workSkill].attribute].attributeLevel;
        int attributeBonus = AttributeData.GetAttributeBonus(attributeLevel, false, false);

        int workAmount;
        if (SkillData.CheckWithBonuses(skillLevel, attributeBonus, 0, 0) == true) // TODO: Perk Bonus
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
    /// <summary>
    /// This should go through the list of completed county improvements and does the math
    /// to generate the goods produced.
    /// </summary>
    /// <param name="countyData"></param>
    public static void CalculateWorkToGoodsProduction(CountyData countyData)
    {
        foreach (CountyImprovementData underConstructionCountyImprovementData in
            countyData.underConstructionCountyImprovementList)
        {
            underConstructionCountyImprovementData.CurrentAmountOfConstruction
                += underConstructionCountyImprovementData.allDailyWorkAmountAtImprovementCompleted;
        }
        foreach (CountyImprovementData countyImprovementData in countyData.completedCountyImprovementList)
        {
            // This is checking max workers because it needs to skip county improvements that don't have
            // workers, such as a warehouse.
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

                    // GD.Print($"{countyData.countyName} : {countyImprovementData.improvementName} " +
                    //    $"- Work Amount For Each Resource For Today: {keyValuePair.Value.workAmount}");

                    if (keyValuePair.Value.workCost <= keyValuePair.Value.workAmount)
                    {
                        keyValuePair.Value.todaysGoodsAmountGenerated
                            = keyValuePair.Value.workAmount / keyValuePair.Value.workCost;
                        keyValuePair.Value.workAmount = keyValuePair.Value.workAmount % keyValuePair.Value.workCost;
                    }

                    GD.Print($"{countyData.countyName} {countyImprovementData.improvementName} todays goods " +
                        $"generated: {keyValuePair.Value.todaysGoodsAmountGenerated}");

                    Haulmaster.AdjustCountyGoodAmount(countyData, keyValuePair.Key.countyGoodType, keyValuePair.Value.todaysGoodsAmountGenerated);
                }
                // Reset all the county improvement work so that the next day it will generate with new skill checks.
                countyImprovementData.allDailyWorkAmountAtImprovementCompleted = 0;
            }
            else
            {
                countyImprovementData.allDailyWorkAmountAtImprovementCompleted = 0;
                GD.Print($"{countyImprovementData.improvementName} : There is either no output good, or max workers is zero.");
            }
        }
    }
}