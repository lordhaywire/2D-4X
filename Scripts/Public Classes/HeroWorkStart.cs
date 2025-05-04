using Godot;
using System;

namespace PlayerSpace;
public abstract class HeroWorkStart
{
    /// <summary>
    /// Assigns Heroes to both the possibleWorkers list, and the prioritizedWorkers list.
    /// </summary>
    /// <param name="countyData"></param>
    public static void AssignWorkingHeroesToPrioritizedLists(CountyData countyData)
    {
        //GD.Print($"Assign Working Heroes To Prioritized Lists in {countyData.countyName} : HeroesInCountyList Count: {countyData.heroesInCountyList.Count}");
        foreach (PopulationData populationData in countyData.heroesInCountyList)
        {
            GD.Print($"{countyData.countyName}:{populationData.firstName} is {populationData.activity}");
            switch (populationData.activity)
            {
                case AllEnums.Activities.Build when populationData.currentCountyImprovement == null:
                    countyData.AddHeroToPrioritizedHeroBuildersList(populationData);
                    GD.Print($"{populationData.firstName} is a hero and has been added to the prioritized builders list.");
                    break;
                case AllEnums.Activities.Work when populationData.currentCountyImprovement == null:
                    countyData.AddHeroToPrioritizedHeroWorkersList(populationData);
                    GD.Print($"{populationData.firstName} is a hero and has been added to the prioritized workers list.");
                    break;
                case AllEnums.Activities.Combat:
                case AllEnums.Activities.Explore:
                case AllEnums.Activities.Idle:
                    GD.Print($"{populationData.firstName} is IDLE!!!!!!!!");
                    break;
                case AllEnums.Activities.Move:
                case AllEnums.Activities.Recruit:
                case AllEnums.Activities.Recruited:
                case AllEnums.Activities.Research:
                case AllEnums.Activities.Scavenge:
                case AllEnums.Activities.Service:
                default:
                    GD.Print($"{populationData.firstName} is {populationData.GetActivityName()}");
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
        foreach (PopulationData populationData in countyData.prioritizedHeroBuildersList[..availableBuilderSlots])
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

        foreach (PopulationData populationData in countyData.prioritizedHeroWorkersList[..availableWorkerSlots])
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
