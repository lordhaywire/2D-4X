using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;

public abstract class PopulationWorkStart
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
            if (populationData.CheckWillWorkLoyalty()
                && populationData.CheckForPerk(AllEnums.Perks.Unhelpful) == false
                && populationData.activity != AllEnums.Activities.Recruited)
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
            if (populationData.CheckWillWorkLoyalty()
                && populationData.CheckForPerk(AllEnums.Perks.Unhelpful) == false
                && populationData.activity != AllEnums.Activities.Recruited)
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
            if (countyImprovementData.prioritize)
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
            if (countyImprovementData.prioritize &&
                countyImprovementData.CheckIfStatusLowStockpiledGoods() == false)
            {
                countyData.AddImprovementToPrioritizedWorkImprovementList(countyImprovementData);
            }
        }
    }

    public static void AssignBuildersToImprovement(CountyData countyData)
    {
        foreach (CountyImprovementData countyImprovementData in countyData.prioritizedConstructionImprovementList)
        {
            if (countyData.prioritizedHeroBuildersList.Count > 0)
            {
                HeroWorkStart.AssignHeroesToBuildImprovement(countyData, countyImprovementData);
            }

            AssignPopulationToBuildImprovement(countyData, countyImprovementData, countyData.prioritizedBuildersList);
        }
    }

    /// <summary>
    /// This list is getting sorted because the populations skill does matter, and the heroes
    /// should have been removed from the list at this point.
    /// </summary>
    /// <param name="countyData"></param>
    /// <param name="countyImprovementData"></param>
    /// <param name="improvementBuildersList"></param>
    public static void AssignPopulationToBuildImprovement(CountyData countyData
        , CountyImprovementData countyImprovementData, Godot.Collections.Array<PopulationData> improvementBuildersList)
    {
        // We are not sorting the list till here, because the heroes are at the top of the list to start with.
        List<PopulationData> sortedList = [.. improvementBuildersList];

        // We should probably test that this is sorting correctly.
        // Sort population list by highest construction skill.
        sortedList.Sort((a, b) => b.skills[AllEnums.Skills.Construction].skillLevel
            .CompareTo(a.skills[AllEnums.Skills.Construction].skillLevel));

        // Update the remaining workers slots number.
        int remainingWorkerSlots = countyImprovementData.adjustedMaxBuilders -
                                   countyImprovementData.populationAtImprovement.Count;
        int availableBuilderSlots = Math.Min(improvementBuildersList.Count
            , remainingWorkerSlots);

        // Adds normal population to improvement.
        foreach (PopulationData populationData in sortedList.Take(availableBuilderSlots))
        {
            populationData.RemoveFromCountyImprovement();
            populationData.UpdateActivity(AllEnums.Activities.Build);
            populationData.UpdateCurrentCountyImprovement(countyImprovementData);
            countyImprovementData.AddPopulationToPopulationAtImprovementList(populationData);
            countyData.workersToRemoveFromLists.Add(populationData);
        }

        countyData.RemovePopulationFromLists(improvementBuildersList);
    }

    /// <summary>
    /// This list is getting sorted because the populations skill does matter, and the heroes
    /// should have been removed from the list at this point.
    /// </summary>
    /// <param name="countyData"></param>
    /// <param name="countyImprovementData"></param>
    private static void AssignPopulationToWorkImprovement(CountyData countyData,
        CountyImprovementData countyImprovementData
    )
    {
        // We are not sorting the list till here, because the heroes are at the top of the list to start with.
        List<PopulationData> sortedList = [.. countyData.prioritizedWorkersList];

        // We should probably test that this is sorting correctly.
        // Sort population list by highest construction skill.
        sortedList.Sort((a, b) => b.skills[countyImprovementData.workSkill].skillLevel
            .CompareTo(a.skills[countyImprovementData.workSkill].skillLevel));

        // Update the remaining workers slots number.
        int remainingWorkerSlots = countyImprovementData.adjustedMaxWorkers
                                   - countyImprovementData.populationAtImprovement.Count;
        int availableWorkersSlots = Math.Min(countyData.prioritizedWorkersList.Count
            , remainingWorkerSlots);

        // Adds normal population to improvement.
        foreach (PopulationData populationData in sortedList.Take(availableWorkersSlots))
        {
            populationData.RemoveFromCountyImprovement();
            populationData.UpdateActivity(AllEnums.Activities.Work);
            populationData.UpdateCurrentCountyImprovement(countyImprovementData);
            countyImprovementData.AddPopulationToPopulationAtImprovementList(populationData);
            countyData.workersToRemoveFromLists.Add(populationData);
        }

        countyData.RemovePopulationFromLists(countyData.prioritizedWorkersList);
    }

    /// <summary>
    /// This has not been changed at all from AssignBuildersToImprovement.
    /// </summary>
    /// <param name="countyData"></param>
    public static void AssignWorkersToImprovement(CountyData countyData)
    {
        foreach (CountyImprovementData countyImprovementData
                 in countyData.prioritizedWorkImprovementList)
        {
            if (countyData.prioritizedHeroWorkersList.Count > 0)
            {
                HeroWorkStart.AssignHeroesToWorkImprovement(countyData, countyImprovementData);
            }

            AssignPopulationToWorkImprovement(countyData, countyImprovementData);
        }
    }

    public static void ClearPrioritizedBuildersList(CountyData countyData)
    {
        countyData.prioritizedBuildersList.Clear();
    }

    public static void ClearPrioritizedWorkersList(CountyData countyData)
    {
        countyData.prioritizedWorkersList.Clear();
    }
}