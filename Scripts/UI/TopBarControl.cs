using Godot;
using System;
using System.Collections.Generic;
using System.Resources;

namespace PlayerSpace
{
    public partial class TopBarControl : Control
    {
        public static TopBarControl Instance { get; private set; }

        [Export] private Clock clock;
        [Export] private Label influenceLabel;
        [Export] private Label influenceAmountUsed;
        [Export] private Label moneyLabel;
        [Export] private Label moneyAmountUsed;
        [Export] private Label remnantsLabel;
        [Export] private Label remnantsAmountUsed;
        [Export] private Label buildingMaterialsLabel;
        [Export] private Label buildingMaterialsAmountUsed;
        [Export] private Label foodLabel;
        [Export] private Label foodAmountUsed;

        public override void _Ready()
        {
            Instance = this;
            Globals.Instance.playerFactionData.InfluenceChanged += UpdateFactionExpendables;
            Globals.Instance.playerFactionData.MoneyChanged += UpdateFactionExpendables;
            Globals.Instance.playerFactionData.ScrapChanged += UpdateFactionExpendables;
            Globals.Instance.playerFactionData.BuildingMaterialsChanged += UpdateFactionExpendables;
            Globals.Instance.playerFactionData.FoodChanged += UpdateFactionExpendables;
            UpdateFactionExpendables();
        }

        public void UpdateResourcesUsedYesterday()
        {
            if(Globals.Instance.SelectedLeftClickCounty != null)
            {
                
                CountyData countyData = Globals.Instance.SelectedLeftClickCounty.countyData;
                // Subtract yesterday's resources from today's.
                //int yesterdaysInfluence = countyData.resources[AllEnums.CountyResourceType.Influence].amount -
            }
        }

        public static void UpdateTopBarWithCountyResources()
        {
            if (Globals.Instance.SelectedLeftClickCounty != null)
            {
                County county = Globals.Instance.SelectedLeftClickCounty;
                CountFactionResources(county.countyData.resources);
            }
        }

        private static void CountFactionResources(Godot.Collections.Dictionary<AllEnums.CountyResourceType, CountyResourceData> resources)
        {
            foreach (KeyValuePair<AllEnums.CountyResourceType, CountyResourceData> keyValuePair in resources)
            {
                CountyResourceData countyResource = keyValuePair.Value;
                FactionData factionData = Globals.Instance.playerFactionData;
                switch (countyResource.factionResourceType)
                {
                    case AllEnums.FactionResourceType.Food:
                        factionData.factionResources[AllEnums.FactionResourceType.Food].amount 
                            += countyResource.amount;
                        break;
                    case AllEnums.FactionResourceType.Remnants:
                        factionData.factionResources[AllEnums.FactionResourceType.Remnants].amount 
                            += countyResource.amount;
                        break;
                    case AllEnums.FactionResourceType.BuildingMaterial:
                        factionData.factionResources[AllEnums.FactionResourceType.BuildingMaterial].amount 
                            += countyResource.amount;
                        break;
                }
            }
        }

        public void UpdateFactionExpendables()
        {
            GD.Print("Expendables have been updated, motherfucker!");
            FactionData factionData = Globals.Instance.playerFactionData;
            influenceLabel.Text = factionData.factionResources[AllEnums.FactionResourceType.Influence].amount.ToString();
            moneyLabel.Text = factionData.factionResources[AllEnums.FactionResourceType.Money].amount.ToString();
            foodLabel.Text = factionData.factionResources[AllEnums.FactionResourceType.Food].amount.ToString();
            remnantsLabel.Text = factionData.factionResources[AllEnums.FactionResourceType.Remnants].amount.ToString();
            buildingMaterialsLabel.Text = factionData.factionResources[AllEnums.FactionResourceType.BuildingMaterial].amount.ToString();

            // Do the math for the used resources from yesterday.
            influenceAmountUsed.Text 
                = $"({factionData.amountUsedFactionResources[AllEnums.FactionResourceType.Influence].amount})";
            moneyAmountUsed.Text 
                = $"({factionData.amountUsedFactionResources[AllEnums.FactionResourceType.Money].amount})";
            ;
        }

        public void ChangeSpeed(int speed)
        {
            clock.ChangeSpeed(speed);
        }
    }
}