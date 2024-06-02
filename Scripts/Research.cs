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
                GD.Print($"{county.countyData.countyName}: {countyPopulation.firstName} increased " +
                    $"{county.countyData.factionData.researchItems[1].researchName} to:" +
                    $"{county.countyData.factionData.researchItems[1].AmountOfResearchDone}");
            }
        }

        public void ApplyHeroResearch(FactionData factionData)
        {
            foreach (CountyPopulation countyPopulation in factionData.allHeroesList)
            {
                if (countyPopulation.currentActivity == AllEnums.Activities.Research)
                {
                    bool passedCheck = factionData.skillHandling.Check(countyPopulation.skills[AllEnums.Skills.Research].skillLevel);
                    GD.PrintRich($"[rainbow]{countyPopulation.firstName} skill check: {passedCheck}");
                    IncreaseResearcherResearch(countyPopulation, passedCheck);

                    // Only the researchers learn research skill.  Normal population who is just adding a tiny bit of research
                    // does not get a learning check.
                    countyPopulation.factionData.skillHandling.CheckLearning(countyPopulation, countyPopulation.skills[AllEnums.Skills.Research]);
                }
            }
        }

        private static void IncreaseResearcherResearch(CountyPopulation countyPopulation, bool passedCheck)
        {
            int bonusResearchIncrease = 0;
            if (passedCheck == true)
            {
                bonusResearchIncrease = Globals.Instance.random.Next(1, Globals.Instance.researchIncreaseBonus);
            }
            countyPopulation.CurrentResearchItemData.AmountOfResearchDone
                += Globals.Instance.researcherResearchIncrease + bonusResearchIncrease;

            GD.Print($"Amount of Research Done: {countyPopulation.CurrentResearchItemData.AmountOfResearchDone}");
        }

        public void RemoveResearcher(CountyPopulation countyPopulation)
        {
            ResearchControl.Instance.assignedResearchers.Remove(countyPopulation);
            countyPopulation.CurrentResearchItemData = null;
        }
    }
}