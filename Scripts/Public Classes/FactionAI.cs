namespace PlayerSpace;

public class FactionAI
{
    /// <summary>
    /// This takes the faction leaders personality and assign each heroes equipment depending on that.
    /// </summary>
    /// <param name="faction"></param>
    public static void DecideIfHeroUsesNewestEquipment(Faction faction)
    {
        foreach (PopulationData populationData in faction.factionData.allHeroesList)
        {
            faction.factionData.factionLeader.iPersonality.EquipmentAssignment(populationData);
        }
    }
    // This is very primative logic.  It just goes down the list of idle heroes and assigns the first
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