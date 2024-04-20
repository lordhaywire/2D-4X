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
            CheckForBuildingCountyImprovements();
            DecideHeroAction();
        }

        private void CheckForThoughts()
        {
            CheckForBuildingCountyImprovements();
        }

        private void DecideHeroAction()
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

        private void CheckForBuildingCountyImprovements()
        {
            foreach (FactionData factionData in Globals.Instance.factionDatas)
            {
                // Is there enough food? If not build a food building.
                if (factionData.isPlayer == false)
                {
                    if (Banker.Instance.CheckEnoughFood(factionData) == false)
                    {
                        Banker.Instance.FindFoodBuilding(factionData, out CountyImprovementData foodBuilding, out CountyData countyData);

                        if (foodBuilding != null && countyData != null)
                        {
                            GD.Print($"{factionData.factionName} wants to build a {foodBuilding.improvementName}");
                            if (Banker.Instance.CheckBuildingCost(factionData, foodBuilding) == true)
                            {
                                Banker.Instance.ChargeForBuilding(factionData, foodBuilding);
                                Banker.Instance.BuildImprovement(countyData, foodBuilding);
                            }
                            else
                            {
                                //GD.Print($"{factionData.factionName} doesn't have enough influence.");
                            }
                        }
                        else
                        {
                            //GD.Print("Food building is null");
                        }
                    }
                    else
                    {
                        //GD.Print(factionData.factionName + " has enough food.");
                    }
                }
            }
        }
    }
}