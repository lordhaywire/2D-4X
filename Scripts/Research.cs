using Godot;
using System;

namespace PlayerSpace
{
	public partial class Research : Node
	{
		public static Research Instance { get; private set; }
		public override void _Ready()
		{
			Instance = this;
            Clock.Instance.WorkDayOver += ApplyHeroResearch;
		}

        private void ApplyHeroResearch()
        {
			foreach(FactionData factionData in Globals.Instance.factionDatas)
			{
				foreach(CountyPopulation countyPopulation in factionData.allHeroesList)
				{
					if(countyPopulation.currentActivity == AllText.Activities.RESEARCHING)
					{

					}
				}
			}
        }

        public void RemoveResearcher(CountyPopulation countyPopulation)
		{
            ResearchControl.Instance.assignedResearchers.Remove(countyPopulation);
            countyPopulation.CurrentResearchItemData = null;
        }


	}
}