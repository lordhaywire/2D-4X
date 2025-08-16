using System.Collections.Generic;

namespace PlayerSpace;

public class FactionAI
{
    /// <summary>
    /// This takes the faction leaders personality and assign each heroes equipment depending on that.
    /// </summary>
    public static void DecideIfHeroUsesNewestEquipment(FactionData factionData)
    {
        foreach (KeyValuePair<int, int> keyValuePair in factionData.allHeroesDictionary)
        {
            County county = (County)Globals.Instance.countiesParent.GetChild(keyValuePair.Value);
            PopulationData populationData = PopulationData.ReturnPopulationDataFromPopulationId(county.countyData.heroesInCountyList, keyValuePair.Key);
            factionData.factionLeader.iPersonality.EquipmentAssignment(populationData);
        }
    }
    // This is very primitive logic.  It just goes down the list of idle heroes and assigns the first
    // research that isn't done to them.
    /*
    public void AssignResearch(FactionData factionData)
    {
        foreach (CountyPopulation populationData in factionData.allHeroesList)
        {
            if (populationData.activity != AllEnums.Activities.Idle)
            {
                return;
            }
            else
            {
                foreach (ResearchItemData researchItemData in factionData.researchItems)
                {
                    if (researchItemData.CheckIfResearchDone() == false)
                    {
                        populationData.currentResearchItemData = researchItemData;
                        GD.Print($"{factionData.factionName}'s hero, {populationData.firstName}, has been assigned" +
                            $" {researchItemData.researchName}.");
                        return;
                    }
                }
            }
        }
    }
    */
}