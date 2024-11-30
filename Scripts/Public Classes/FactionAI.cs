using Godot;

namespace PlayerSpace;

public class FactionAI
{
    // This is very primative logic.  It just goes down the list of idle heroes and assigns the first
    // research that isn't done to them.
    /*
    public void AssignResearch(FactionData factionData)
    {
        foreach (CountyPopulation countyPopulation in factionData.allHeroesList)
        {
            if (countyPopulation.activity != AllEnums.Activities.Idle)
            {
                return;
            }
            else
            {
                foreach (ResearchItemData researchItemData in factionData.researchItems)
                {
                    if (researchItemData.CheckIfResearchDone() == false)
                    {
                        countyPopulation.currentResearchItemData = researchItemData;
                        GD.Print($"{factionData.factionName}'s hero, {countyPopulation.firstName}, has been assigned" +
                            $" {researchItemData.researchName}.");
                        return;
                    }
                }
            }
        }
    }
    */
}