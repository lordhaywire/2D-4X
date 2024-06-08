using Godot;

namespace PlayerSpace
{
    public class Research
    {
        // This needs to go to the ResearchItemData.
        public void RemoveResearcher(CountyPopulation countyPopulation)
        {
            ResearchControl.Instance.assignedResearchers.Remove(countyPopulation);
            countyPopulation.CurrentResearchItemData = null;
        }
    }
}