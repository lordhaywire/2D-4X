using Godot;

namespace PlayerSpace
{
    public class Research
    {
        public void PopulationResearch(County county)
        {
            foreach (CountyPopulation countyPopulation in county.countyData.countyPopulationList)
            {
                // Currently manually set to the second Research (Researching) to be updated later.
                county.countyData.factionData.researchItems[1].AmountOfResearchDone += Globals.Instance.populationResearchIncrease;
                /*
                GD.Print($"{county.countyData.countyName}: {countyPopulation.firstName} increased " +
                    $"{county.countyData.factionData.researchItems[1].researchName} to:" +
                    $"{county.countyData.factionData.researchItems[1].AmountOfResearchDone}");
                */
            }
        }

        public void RemoveResearcher(CountyPopulation countyPopulation)
        {
            ResearchControl.Instance.assignedResearchers.Remove(countyPopulation);
            countyPopulation.CurrentResearchItemData = null;
        }
    }
}