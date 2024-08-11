using Godot;

namespace PlayerSpace
{
    public class Research
    {
        // This needs to go to the ResearchItemData, maybe.
        public void RemoveResearcher(CountyPopulation countyPopulation)
        {
            ResearchControl.Instance.assignedResearchers.Remove(countyPopulation);
            countyPopulation.currentResearchItemData = null;
            // If the population isn't a hero then they must be working at a research office,
            // thus we need to make their activity be work.
            if (countyPopulation.isHero == false)
            {
                countyPopulation.UpdateActivity(AllEnums.Activities.Work);
            }
        }
    }
}