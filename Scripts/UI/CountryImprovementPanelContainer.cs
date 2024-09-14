using Godot;

namespace PlayerSpace
{
    public partial class CountryImprovementPanelContainer : PanelContainer
    {
        public CountyImprovementData countyImprovementData;

        [Export] public Label progressTitle;
        [Export] public ProgressBar progressBar;
        [Export] public CheckBox prioritizeCheckBox;
        [Export] public Label improvementNameLabel;
        [Export] private Label improvementDescriptionLabel;
        [Export] private TextureRect improvementTextureRect;
        [Export] public CheckBox produceAsNeededCheckBox;
        [Export] public Label currentWorkersNumberLabel;
        [Export] public Label maxWorkersNumberLabel;
        [Export] public Button minusButton;
        [Export] public Button plusButton;
        [Export] public PackedScene outputPanelContainerPackedScene;
        [Export] public PackedScene inputPanelContainerPackedScene;
        [Export] public PackedScene constructionMaterialCostLabelPackedScene;
        [Export] public CheckBox remnantsForContructionCheckBox;
        [Export] public Label currentBuildersNumberLabel;
        [Export] public Label maxBuildersNumberLabel;
        [Export] private Button constructButton;
        [Export] public Label underContructionLabel;

        public override void _Ready()
        {
            CallDeferred(nameof(UpdatePossibleBuildingLabels));
        }

        public void UpdatePossibleBuildingLabels()
        {
            GD.Print("Number of people working at county improvement: " + countyImprovementData.countyPopulationAtImprovement.Count);
            Banker banker = new();
            //improvementTexture.Texture = countyImprovementData.improvementTexture;
            //improvementNameLabel.Text = countyImprovementData.improvementName;
            //improvementDescriptionLabel.Text = countyImprovementData.improvementDescription;
            //improvementInfluenceCostLabel.Text = $"Influence Cost: {countyImprovementData.influenceCost}";
            /*
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
                    // We could probably return out of here at this point.
                }

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
                    improvementMaxBuildersLabel.Text = $"{countyImprovementData.countyPopulationAtImprovement.Count}/{countyImprovementData.maxBuilders} Builders";
                }

                CheckForUnderConstructionLabel();

                if (countyImprovementData.status == AllEnums.CountyImprovementStatus.Complete)
                {
                    improvementMaxBuildersLabel.Text = $"{countyImprovementData.countyPopulationAtImprovement.Count}/{countyImprovementData.maxWorkers} Workers";
                    improvementAmountOfConstructionLabel.Hide();
                    improvementInfluenceCostLabel.Hide();
                }
            }
            */
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
            GD.Print("You have pressed the county improvement button.");
            CountyImprovementsControl.Instance.buildConfirmationDialog.Visible = true;
            Globals.Instance.selectedPossibleBuildingControl = this;
        }
    }
}