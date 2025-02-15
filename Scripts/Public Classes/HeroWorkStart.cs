using Godot;
using System;

namespace PlayerSpace;
public class HeroWorkStart
{
    /// <summary>
    /// Assigns Heroes to both the possibleWorkers list, and the prioritizedWorkers list.
    /// </summary>
    /// <param name="countyData"></param>
    public static void AssignWorkingHeroesToPrioritizedLists(CountyData countyData)
    {
        foreach (PopulationData populationData in countyData.heroesInCountyList)
        {
            switch (populationData.activity)
            {
                case AllEnums.Activities.Build when populationData.currentCountyImprovement == null:
                    countyData.AddHeroToPrioritizedHeroBuildersList(populationData);
                    GD.Print($"{populationData.firstName} is a hero and has been added to the possible & prioritized builders list.");
                    break;
                case AllEnums.Activities.Work when populationData.currentCountyImprovement == null:
                    countyData.AddHeroToPrioritizedHeroWorkersList(populationData);
                    GD.Print($"{populationData.firstName} is a hero and has been added to the possible & prioritized workers list.");
                    break;
                case AllEnums.Activities.Scavenge:
                    GD.Print($"{populationData.firstName} is scavenging.");
                    break;
            }
        }
    }

    /// <summary>
    /// My fear is that the hero will be removed from a county improvement and moved to a new one whenever things change.
    /// Heroes who are already working on prioritized buildings should have been removed from the prioritized
    /// hero builders list at this point.
    /// We do not sort this list because at this point the heroes should be at the top of the list,
    /// and their skill doesn't matter.
    /// </summary>
    /// <param name="countyData"></param>
    /// <param name="countyImprovementData"></param>
    public static void AssignHeroesToBuildImprovement(CountyData countyData
        , CountyImprovementData countyImprovementData)
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
    }


    /// <summary>
    /// We do not sort this list because at this point the heroes should be at the top of the list,
    /// and their skill doesn't matter.
    /// </summary>
    /// <param name="countyData"></param>
    /// <param name="countyImprovementData"></param>
    public static void AssignHeroesToWorkImprovement(CountyData countyData, CountyImprovementData countyImprovementData)
    {
        int remainingWorkerSlots = countyImprovementData.adjustedMaxWorkers 
            - countyImprovementData.populationAtImprovement.Count;
        // Assign Heroes to improvement.
        int availableWorkerSlots = Math.Min(countyData.prioritizedHeroWorkersList.Count
            , remainingWorkerSlots);

        foreach (PopulationData populationData in countyData.prioritizedHeroWorkersList.Slice(0, availableWorkerSlots))
        {
            populationData.RemoveFromCountyImprovement(); // This is just in case they are already at a county improvement.
            populationData.UpdateActivity(AllEnums.Activities.Work);
            populationData.UpdateCurrentCountyImprovement(countyImprovementData);
            countyImprovementData.AddPopulationToPopulationAtImprovementList(populationData);
            countyData.workersToRemoveFromLists.Add(populationData);
        }

        countyData.RemovePopulationFromLists(countyData.prioritizedHeroWorkersList);
    }


    public static void ClearPrioritizedHeroWorkersList(CountyData countyData)
    {
        countyData.prioritizedHeroWorkersList.Clear();
    }

    public static void ClearPrioritizedHeroBuildersList(CountyData countyData)
    {
        countyData.prioritizedHeroBuildersList.Clear();
    }
}
