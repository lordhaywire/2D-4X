using Godot;
using System.Linq;

namespace PlayerSpace
{
    public class Work
    {

        // End work for all of the world population!        
        // This is what we are probably getting rid of, sort of.
        public static void PopulationDailyResearch(CountyPopulation countPopulation, AllEnums.Skills skill)
        {
            GD.Print("Population Daily Research?  Are we using this?");
        }

        // Go through everyone in this county again and clear out their job if their building is done.
        // This is now in CountyAI called CheckIfCountyImprovementsAreDone().
        /*
        private void CheckConstructionComplete()
        {
            foreach (CountyPopulation countyPopulation in county.countyData.countyPopulationList)
            {
                // ? is null checking currentImprovement.
                if (countyPopulation.CurrentCountyImprovment?.status == AllEnums.CountyImprovementStatus.Complete)
                {
                    countyPopulation.CurrentCountyImprovment = null;
                    countyPopulation.UpdateActivity(AllEnums.Activities.Idle);
                }
            }
            county.countyData.CountIdleWorkers();
        }
        */
    }
}