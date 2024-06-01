using Godot;
using System;

namespace PlayerSpace
{

    public partial class FactionsAI : Node
    {
        public FactionData factionData;
        public override void _Ready()
        {
            // The hero will check every time is becomes idle after this.

            Clock.Instance.FirstRun += CheckForFirstThoughts;
            // I am not sure if we want this every hour.  Instead we could check every time a county improvement is done
            // being built.
            Clock.Instance.HourZero += CheckForThoughts;
        }

        private void CheckForFirstThoughts()
        {
            //CheckForBuildingCountyImprovements();
            DecideHeroAction();
        }

        private void CheckForThoughts()
        {
            //CheckForBuildingCountyImprovements();
        }

        private static void DecideHeroAction()
        {
            foreach (FactionData factionData in Globals.Instance.factionDatas)
            {
                foreach (CountyPopulation countyPopulation in factionData.allHeroesList)
                {
                    //Temporarily assign all heroes to research Researching.
                    if (countyPopulation.factionData != Globals.Instance.playerFactionData)
                    {
                        countyPopulation.CurrentResearchItemData = factionData.researchItems[1];
                    }
                }
            }
        }

        
    }
}