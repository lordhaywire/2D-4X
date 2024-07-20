using Godot;
using System;

namespace PlayerSpace
{
    public partial class CountryImprovementDescriptionButton : Control
    {
        public CountyImprovementData countyImprovementData;

        [Export] private TextureRect improvementTexture;
        [Export] public Label improvementNameLabel;
        [Export] private Label improvementDescriptionLabel;
        [Export] private Label improvementInfluenceCostLabel;
        [Export] private Label improvementAmountOfConstructionLabel;
        [Export] private Label improvementMaxBuildersLabel;

        [Export] public Label underContructionLabel;

        [Export] private Button buildingButton;

        public override void _Ready()
        {
            CallDeferred("UpdatePossibleBuildingLabels");
        }

        public void UpdatePossibleBuildingLabels()
        {
            Banker banker = new();
            improvementTexture.Texture = countyImprovementData.improvementTexture;
            improvementNameLabel.Text = countyImprovementData.improvementName;
            improvementDescriptionLabel.Text = countyImprovementData.improvementDescription;
            improvementInfluenceCostLabel.Text = $"Influence Cost: {countyImprovementData.influenceCost}";
            if (ResearchDescriptionPanel.Instance.Visible == true)
            {
                buildingButton.Disabled = true;
                improvementMaxBuildersLabel.Text = $"{countyImprovementData.maxBuilders} Builders";
                improvementAmountOfConstructionLabel.Text = $"{countyImprovementData.maxAmountOfConstruction} Amount of Contruction";
                return;
            }
            else if (CountyImprovementsControl.Instance.Visible == true)
            {
                // Something is going to get fucked up here.
                if (banker.CheckBuildingCost(Globals.Instance.SelectedLeftClickCounty.countyData.factionData, countyImprovementData) == false)
                {
                    buildingButton.Disabled = true;
                }
                // We can get rid of this since we can have multiple of the same building.
                if (countyImprovementData.status == AllEnums.CountyImprovementStatus.UnderConstruction
                    || countyImprovementData.status == AllEnums.CountyImprovementStatus.Complete)
                {
                    buildingButton.Disabled = true;
                }

                if (countyImprovementData.status == AllEnums.CountyImprovementStatus.None)
                {
                    improvementAmountOfConstructionLabel.Text = $"Amount of construction: {countyImprovementData.maxAmountOfConstruction}";
                    improvementMaxBuildersLabel.Text = $"Max builders: {countyImprovementData.maxBuilders}";
                }
                else
                {
                    improvementInfluenceCostLabel.Hide();
                    improvementAmountOfConstructionLabel.Text = $"{countyImprovementData.CurrentAmountOfConstruction}/{countyImprovementData.maxAmountOfConstruction} Amount of Contruction";
                    improvementMaxBuildersLabel.Text = $"{countyImprovementData.currentBuilders}/{countyImprovementData.maxBuilders} Builders";
                }
                CheckForUnderConstructionLabel();
                if (countyImprovementData.status == AllEnums.CountyImprovementStatus.Complete)
                {
                    improvementMaxBuildersLabel.Text = $"{countyImprovementData.currentWorkers}/{countyImprovementData.maxWorkers} Workers";
                    improvementAmountOfConstructionLabel.Hide();
                    improvementInfluenceCostLabel.Hide();
                }
            }
        }

        private void CheckForUnderConstructionLabel()
        {
            if (countyImprovementData.status == AllEnums.CountyImprovementStatus.UnderConstruction)
            {
                underContructionLabel.Show();
            }
            else
            {
                underContructionLabel.Hide();
            }
        }

        private void BuildingButton()
        {
            //GD.Print("You have pressed the building button.");
            CountyImprovementsControl.Instance.buildConfirmationDialog.Visible = true;
            Globals.Instance.selectedPossibleBuildingControl = this;
        }
    }
}