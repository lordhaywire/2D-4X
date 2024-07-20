using Godot;
using System;
using System.Collections.Generic;

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

        FactionData factionData;

        public override void _Ready()
        {
            Instance = this;
            factionData = Globals.Instance.playerFactionData;
            UpdateResourceLabels();
        }

        public void UpdateResourceLabels()
        {
            GD.Print("Top Bar expendables have been updated, motherfucker!");
            UpdateInfluenceMoneyLabels();
            UpdateUsedInfluenceMoneyLabels();

            if (Globals.Instance.SelectedLeftClickCounty == null)
            {
                FactionLevelResources();
            }
            else
            {
                CountyLevelResources();
            }
        }

        private void CountyLevelResources()
        {
            UpdateLabelWithCountyAmount();
            UpdateLabelWithCountyUsedAmount();
        }

        private void UpdateLabelWithCountyAmount()
        {
            CountyData countyData = Globals.Instance.SelectedLeftClickCounty.countyData;

            foodLabel.Text = $"{countyData.CountFactionResourceOfType(AllEnums.FactionResourceType.Food)}";
            remnantsLabel.Text = $"{countyData.CountFactionResourceOfType(AllEnums.FactionResourceType.Remnants)}";
            buildingMaterialsLabel.Text = $"{countyData.CountFactionResourceOfType(AllEnums.FactionResourceType.BuildingMaterial)}";
        }
        private void UpdateLabelWithCountyUsedAmount()
        {
            CountyData countyData = Globals.Instance.SelectedLeftClickCounty.countyData;

            foodAmountUsed.Text = $"({countyData.CountUsedFactionResourceOfType(AllEnums.FactionResourceType.Food)})";
            remnantsAmountUsed.Text = $"({countyData.CountUsedFactionResourceOfType(AllEnums.FactionResourceType.Remnants)})";
            buildingMaterialsAmountUsed.Text = $"({countyData.CountUsedFactionResourceOfType(AllEnums.FactionResourceType.BuildingMaterial)})";
        }

        private void UpdateInfluenceMoneyLabels()
        {
            influenceLabel.Text = factionData.factionResources[AllEnums.FactionResourceType.Influence].amount.ToString();
            moneyLabel.Text = factionData.factionResources[AllEnums.FactionResourceType.Money].amount.ToString();
        }
        private void UpdateUsedInfluenceMoneyLabels()
        {
            influenceAmountUsed.Text
                = $"({factionData.amountUsedFactionResources[AllEnums.FactionResourceType.Influence].amount})";
            moneyAmountUsed.Text
                = $"({factionData.amountUsedFactionResources[AllEnums.FactionResourceType.Money].amount})";
        }

        private void FactionLevelResources()
        {
            // Count all the county resources and assign them to the faction resource dictionary.
            factionData.CountAllCountyFactionResources();
            factionData.CountAllCountyFactionUsedResources();

            // Update all of the faction labels
            UpdateLabelsWithFactionAmounts();

            // Update the used resource amount.
            UpdateLabelsWithFactionUsedAmounts();
        }

        private void UpdateLabelsWithFactionUsedAmounts()
        {
            foodAmountUsed.Text = $"({factionData.amountUsedFactionResources[AllEnums.FactionResourceType.Food].amount})";
            remnantsAmountUsed.Text = $"({factionData.amountUsedFactionResources[AllEnums.FactionResourceType.Remnants].amount})";
            buildingMaterialsAmountUsed.Text = $"({factionData.amountUsedFactionResources[AllEnums.FactionResourceType.BuildingMaterial].amount})";
        }

        private void UpdateLabelsWithFactionAmounts()
        {
            foodLabel.Text = factionData.factionResources[AllEnums.FactionResourceType.Food].amount.ToString();
            remnantsLabel.Text = factionData.factionResources[AllEnums.FactionResourceType.Remnants].amount.ToString();
            buildingMaterialsLabel.Text = factionData.factionResources[AllEnums.FactionResourceType.BuildingMaterial].amount.ToString();
        }

        public void ChangeSpeed(int speed)
        {
            clock.ChangeSpeed(speed);
        }
    }
}