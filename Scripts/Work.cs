using Godot;
using System.Linq;

namespace PlayerSpace
{
    public class Work
    {
        private readonly County county;

        // End work for all of the world population!        
        // This is what we are probably getting rid of, sort of.
        public static void PopulationDailyResearch(CountyPopulation countPopulation, AllEnums.Skills skill)
        {
            GD.Print("Population Daily Research?  Are we using this?");
        }

        // Go through everyone in this county again and clear out their job if their building is done.
        private void CheckConstructionComplete()
        {
            Banker banker = new();

            foreach (CountyPopulation countyPopulation in county.countyData.countyPopulationList)
            {
                // ? is null checking currentImprovement.
                if (countyPopulation.CurrentConstruction?.status == AllEnums.CountyImprovementStatus.Completed)
                {
                    Activities activities = new();
                    countyPopulation.CurrentConstruction = null;
                    countyPopulation.NextConstruction = null;
                    activities.UpdateNext(countyPopulation, AllEnums.Activities.Idle);
                }
            }
            banker.CountIdleWorkers(county);
        }
    }
}