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
            Globals.Instance.playerFactionData.InfluenceChanged += UpdateExpendables;
            Globals.Instance.playerFactionData.MoneyChanged += UpdateExpendables;
            Globals.Instance.playerFactionData.ScrapChanged += UpdateExpendables;
            Globals.Instance.playerFactionData.BuildingMaterialsChanged += UpdateExpendables;
            Globals.Instance.playerFactionData.FoodChanged += UpdateExpendables;
            UpdateExpendables();
        }

        public void UpdateResourcesUsedYesterday()
        {
            if(Globals.Instance.SelectedLeftClickCounty != null)
            {
                CountyData countyData = Globals.Instance.SelectedLeftClickCounty.countyData;
                influenceAmountUsed.Text = countyData.resourcesUsedYesterday[AllEnums.FactionResourceType.Influence].ToString();
                moneyAmountUsed.Text = countyData.resourcesUsedYesterday[AllEnums.FactionResourceType.Money].ToString();
                remnantsAmountUsed.Text = countyData.resourcesUsedYesterday[AllEnums.FactionResourceType.Remnants].ToString();
                buildingMaterialsAmountUsed.Text = countyData.resourcesUsedYesterday[AllEnums.FactionResourceType.BuildingMaterial].ToString();
                foodAmountUsed.Text = countyData.resourcesUsedYesterday[AllEnums.FactionResourceType.Food].ToString();
            }
        }

        public void UpdateTopBarWithCountyResources()
        {
            Globals.Instance.playerFactionData.RemnantsFaction = 0;
            Globals.Instance.playerFactionData.BuildingMaterialsFaction = 0;
            Globals.Instance.playerFactionData.FoodFaction = 0;

            if (Globals.Instance.SelectedLeftClickCounty != null)
            {
                County county = Globals.Instance.SelectedLeftClickCounty;
                CountFactionResources(county.countyData.resources);
            }
        }

        private static void CountFactionResources(Godot.Collections.Dictionary<AllEnums.CountyResourceType, ResourceData> resources)
        {
            foreach (KeyValuePair<AllEnums.CountyResourceType, ResourceData> keyValuePair in resources)
            {
                ResourceData resource = keyValuePair.Value;
                switch (resource.factionResourceType)
                {
                    case AllEnums.FactionResourceType.Food:
                        Globals.Instance.playerFactionData.FoodFaction += resource.amount;
                        break;
                    case AllEnums.FactionResourceType.Remnants:
                        Globals.Instance.playerFactionData.RemnantsFaction += resource.amount;
                        break;
                    case AllEnums.FactionResourceType.BuildingMaterial:
                        Globals.Instance.playerFactionData.BuildingMaterialsFaction += resource.amount;
                        break;
                }
            }
        }

        public void UpdateExpendables()
        {
            //GD.Print("Expendables have been updated, motherfucker!");
            influenceLabel.Text = Globals.Instance.playerFactionData.Influence.ToString();
            moneyLabel.Text = Globals.Instance.playerFactionData.Money.ToString();
            foodLabel.Text = Globals.Instance.playerFactionData.FoodFaction.ToString();
            remnantsLabel.Text = Globals.Instance.playerFactionData.RemnantsFaction.ToString();
            buildingMaterialsLabel.Text = Globals.Instance.playerFactionData.BuildingMaterialsFaction.ToString();
        }

        public void ChangeSpeed(int speed)
        {
            clock.ChangeSpeed(speed);
        }
    }
}