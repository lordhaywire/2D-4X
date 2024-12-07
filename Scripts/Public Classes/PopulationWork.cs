using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    public class PopulationWork
    {
        public static void WorkDayOverForPopulation(CountyData countyData
            , Godot.Collections.Array<CountyPopulation> countyPopulationList)
        {
            foreach (CountyPopulation countyPopulation in countyPopulationList)
            {
                /*
                if (countyData.factionData.isPlayer == true)
                {
                    GD.PrintRich($"[color=blue]{Clock.Instance.GetDateAndTime()} {countyPopulation.firstName} {countyPopulation.activity}[/color]");
                }
                */
                switch (countyPopulation.activity)
                {
                    case AllEnums.Activities.Scavenge:
                        //GD.Print($"{countyPopulation.firstName} {countyPopulation.lastName} is generating scavenged resources.");
                        // Skill learning is done in the GenerateScavengedResources.
                        Banker.GenerateScavengedResources(countyData, countyPopulation);

                        // Learning skillcheck.
                        // Just for testing it is set to fast.  The bool doesn't matter for this skill.
                        SkillData.CheckLearning(countyPopulation, true);

                        countyPopulation.UpdateActivity(AllEnums.Activities.Idle);
                        break;
                    case AllEnums.Activities.Build:
                    case AllEnums.Activities.Work:
                        // Produce resources based on the countyimprovement
                        ApplyWorkPerPerson(countyData, countyPopulation);

                        // Check for Skill Learning.
                        // The bool doesn't matter for this skill.
                        SkillData.CheckLearning(countyPopulation, true);
                        // Check loyalty to see if they still want to work there and if they don't then they
                        // get set to idle.
                        PopulationAI.KeepWorkingAtCountyImprovement(countyPopulation);
                        break;
                    case AllEnums.Activities.Research:
                        // Person working at research office, or hero generates research.

                        //Banker.AddResearchForOfficeResearch(countyPopulation);

                        //PopulationAI.KeepResearching(countyPopulation);
                        break;
                    case AllEnums.Activities.Idle:
                        // Give idle people their bonus happiness.
                        countyPopulation.AddRandomHappiness(5);
                        break;
                }
            }
            //// GD.PrintRich($"[rainbow]{countyData.countyName}: Work Day Over For Population.");
        }

        /// <summary>
        /// Shouldn't check learning be in GenerateWorkAmoutWithSkillCheck?
        /// Applies work for each person and adds it to the county improvments All Daily Work Amount At Improvement Completed.
        /// </summary>
        /// <param name="countyData"></param>
        /// <param name="countyPopulation"></param>
        public static void ApplyWorkPerPerson(CountyData countyData, CountyPopulation countyPopulation)
        {
            if (countyPopulation.currentCountyImprovement == null)
            {
                return;
            }
            //GD.Print($"{countyData.countyName} Someone is working at {countyPopulation.currentCountyImprovement.improvementName}");
            countyPopulation.currentCountyImprovement.allDailyWorkAmountAtImprovementCompleted
                += Banker.GenerateWorkAmountWithSkillCheck(countyPopulation);
            /*
            GD.Print($"{countyPopulation.location}: " +
                $"{countyPopulation.currentCountyImprovement.improvementName}: " +
                $"All Daily {countyPopulation.activity} Amount At Improvement Completed: "
                + countyPopulation.currentCountyImprovement.allDailyWorkAmountAtImprovementCompleted);
            */
        }

        /// <summary>
        /// This should go through the list of completed county improvements and does the math
        /// to generate the goods produced.
        /// </summary>
        /// <param name="countyData"></param>
        public static void CalculateWorkToGoodsProduction(CountyData countyData)
        {
            foreach (CountyImprovementData underConstructionCountyImprovementData in
                countyData.underConstructionCountyImprovements)
            {
                underConstructionCountyImprovementData.CurrentAmountOfConstruction 
                    += underConstructionCountyImprovementData.allDailyWorkAmountAtImprovementCompleted;
            }
            foreach (CountyImprovementData countyImprovementData in countyData.completedCountyImprovements)
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

                        Banker.AddCountyResource(countyData, keyValuePair.Key.countyGoodType, keyValuePair.Value.todaysGoodsAmountGenerated);
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
}