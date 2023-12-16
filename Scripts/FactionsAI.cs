using Godot;

namespace PlayerSpace
{

    public partial class FactionsAI : Node
    {
        public FactionData factionData;

        public override void _Ready()
        {
            Clock.Instance.FirstRun += CheckForBuildingBuildings;
            Clock.Instance.HourZero += CheckForBuildingBuildings;
        }

        private void CheckForBuildingBuildings()
        {
            foreach (FactionData factionData in Globals.Instance.factionDatas)
            {
                // Is there enough food? If not build a food building.
                if(factionData.isPlayer == false)
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
                                GD.Print($"{factionData.factionName} doesn't have enough influence.");
                            }
                        }
                        else
                        {
                            GD.Print("Food building is null");
                        }
                    }
                    else
                    {
                        GD.Print(factionData.factionName + " has enough food.");
                    }
                }  
            }
        }
    }
}