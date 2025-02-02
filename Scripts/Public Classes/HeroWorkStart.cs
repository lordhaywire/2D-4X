using Godot;

namespace PlayerSpace;
public class HeroWorkStart
{
    /// <summary>
    /// Assigns Heroes to both the possibleWorkers list, and the prioritizedWorkers list.
    /// </summary>
    /// <param name="countyData"></param>
    public static void AssignWorkingHeroesToLists(CountyData countyData)
    {
        foreach (PopulationData populationData in countyData.heroesInCountyList)
        {
            switch (populationData.activity)
            {
                case AllEnums.Activities.Build when populationData.currentCountyImprovement == null:
                    countyData.AddHeroToPrioritizedHeroBuildersList(populationData);
                    GD.Print($"{populationData.firstName} has been added to the possible & prioritized builders list.");
                    break;
                case AllEnums.Activities.Work when populationData.currentCountyImprovement == null:
                    countyData.AddHeroToPrioritizedHeroWorkersList(populationData);
                    GD.Print($"{populationData.firstName} has been added to the possible & prioritized workers list.");
                    break;
                case AllEnums.Activities.Scavenge:
                    GD.Print($"{populationData.firstName} is scavenging.");
                    break;
            }
        }
    }
}
