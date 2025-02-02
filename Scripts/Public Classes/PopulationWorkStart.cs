using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PlayerSpace;
public class PopulationWorkStart
{

    /// <summary>
    /// Get all the people who are helpful and loyal for prioritized construction and work.
    /// The heroes should already be assigned to this list.
    /// </summary>
    public static void GeneratePrioritizedBuildersList(CountyData countyData)
    {
        // Go through each person in the county.
        foreach (PopulationData populationData in countyData.populationDataList)
        {
            // Go through everyone and if they are helpful and loyal add them to the prioritizedWorkers list.
            if (populationData.CheckWillWorkLoyalty() == true
                && populationData.CheckForPerk(AllEnums.Perks.Unhelpful) == false)
            {
                //GD.Print($"Prioritized: {countyData.countyName}: {populationData.firstName} is loyal and is helpful.");
                countyData.AddPopulationDataToPrioritizedBuildersList(populationData);

                GD.Print($"{populationData.firstName} is a normy and in the prioritize workers list.");
            }
        }
    }


    /// <summary>
    /// Get all the people who are helpful and loyal for prioritized construction and work.
    /// </summary>
    public static void GeneratePrioritizedWorkersList(CountyData countyData)
    {
        // Go through each person in the county.
        foreach (PopulationData populationData in countyData.populationDataList)
        {
            // Go through everyone and if they are helpful and loyal add them to the prioritizedWorkers list.
            if (populationData.CheckWillWorkLoyalty() == true
                && populationData.CheckForPerk(AllEnums.Perks.Unhelpful) == false)
            {
                countyData.AddPopulationDataToPrioritizedWorkersList(populationData);

                GD.Print($"{populationData.firstName} is a normy and in the prioritize workers list.");
            }
        }
    }

    /// <summary>
    /// There is no stockpile for building under construction so it doesn't check that.
    /// </summary>
    /// <param name="countyData"></param>
    public static void GeneratePrioritizedConstructionImprovementList(CountyData countyData)
    {
        foreach (CountyImprovementData countyImprovementData in countyData.underConstructionCountyImprovementList)
        {
            if (countyImprovementData.prioritize == true)
            {
                countyData.AddImprovementToPrioritizedConstructionImprovementList(countyImprovementData);
            }
        }
    }

    /// <summary>
    /// Work improvements require stockpiles so it must check.
    /// </summary>
    /// <param name="countyData"></param>
    public static void GeneratePrioritizedWorkImprovementList(CountyData countyData)
    {
        foreach (CountyImprovementData countyImprovementData in countyData.completedCountyImprovementList)
        {
            // If there are low goods stockpiled then don't assign workers.
            if (countyImprovementData.prioritize == true &&
            countyImprovementData.CheckIfStatusLowStockpiledGoods() == false)
            {
                countyData.AddImprovementToPrioritizedWorkImprovementList(countyImprovementData);
            }
        }
    }

    public static void AssignBuildersToImprovement(CountyData countyData)
    {
        foreach(CountyImprovementData countyImprovementData in countyData.prioritizedConstructionImprovementList)
        {
            int remainingWorkerSlots = countyImprovementData.adjustedMaxBuilders - countyImprovementData.populationAtImprovement.Count;
            // Assign Heroes to improvement.
            int availableBuilderSlots = Math.Min(countyData.prioritizedHeroBuildersList.Count
                , remainingWorkerSlots);

            foreach (PopulationData populationData in countyData.prioritizedHeroBuildersList.Slice(0, availableBuilderSlots))
            {
                populationData.RemoveFromCountyImprovement(); // This is just in case they are already at a county improvement.
                populationData.UpdateActivity(AllEnums.Activities.Build);
                populationData.UpdateCurrentCountyImprovement(countyImprovementData);
                countyImprovementData.AddPopulationToPopulationAtImprovementList(populationData);
                countyData.workersToRemoveFromLists.Add(populationData);
            }
            
            countyData.RemovePopulationFromLists(countyData.prioritizedHeroBuildersList);

            List<PopulationData> sortedList = countyData.prioritizedBuildersList.ToList();

            // We should probably test that this is sorting correctly.
            // Sort population list by highest construction skill.
            sortedList.Sort((a, b) => b.skills[AllEnums.Skills.Construction].skillLevel
                .CompareTo(a.skills[AllEnums.Skills.Construction].skillLevel));

            // Update the remaining workers slots number.
            remainingWorkerSlots = countyImprovementData.adjustedMaxBuilders - countyImprovementData.populationAtImprovement.Count;
            availableBuilderSlots = Math.Min(countyData.prioritizedBuildersList.Count
                , remainingWorkerSlots);
            foreach (PopulationData populationData in sortedList.Take(availableBuilderSlots))
            {
                populationData.RemoveFromCountyImprovement();
                populationData.UpdateActivity(AllEnums.Activities.Build);
                populationData.UpdateCurrentCountyImprovement(countyImprovementData);
                countyImprovementData.AddPopulationToPopulationAtImprovementList(populationData);
                countyData.workersToRemoveFromLists.Add(populationData);
            }

            countyData.RemovePopulationFromLists(countyData.prioritizedBuildersList);
        }
    }

    public static void ClearPrioritizedWorkersList(CountyData countyData)
    {
        countyData.prioritizedWorkersList.Clear();
    }
}
