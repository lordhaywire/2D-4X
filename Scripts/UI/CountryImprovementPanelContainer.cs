using Godot;
using System;

namespace PlayerSpace
{
    public partial class CountryImprovementPanelContainer : PanelContainer
    {
        public CountyImprovementData countyImprovementData;

        [Export] public Label progressTitle;
        [Export] public ProgressBar progressBar;
        [Export] HBoxContainer prioritizeHBox;
        [Export] public CheckBox prioritizeCheckBox;
        [Export] public Label improvementNameLabel;
        [Export] Label improvementDescriptionLabel;
        [Export] TextureRect improvementTextureRect;
        [Export] PanelContainer workersPanelContainer; // This had the worker amounts and minus/plus buttons inside it.
        [Export] public CheckBox produceAsNeededCheckBox;
        [Export] public Label currentWorkersNumberLabel;
        [Export] public Label adjustedWorkersNumberLabel;
        [Export] public Label maxWorkersNumberLabel;
        [Export] public Button minusWorkerButton;
        [Export] public Button plusWorkerButton;
        [Export] public PackedScene outputPanelContainerPackedScene;
        [Export] public PackedScene inputPanelContainerPackedScene;
        [Export] PanelContainer constructionPanelContainer;
        [Export] public PackedScene constructionMaterialCostLabelPackedScene;
        [Export] public CheckBox remnantsForContructionCheckBox;
        [Export] public HBoxContainer adjustMaxBuildersHbox;
        [Export] public Label currentBuildersNumberLabel;
        [Export] public Label adjustedBuildersNumberLabel;
        [Export] public Label maxBuildersNumberLabel;
        [Export] public Button minusBuilderButton;
        [Export] public Button plusBuilderButton;
        [Export] private Button constructButton;
        [Export] public Label underContructionLabel;
        [Export] Button removeImprovementButton;

        public override void _Ready()
        {
            CallDeferred(nameof(UpdateImprovementLabels));
        }
        private void UpdateConstructionStatus()
        {
            GD.Print($"County Improvement Data: " + countyImprovementData);
            switch (countyImprovementData.status)
            {
                case AllEnums.CountyImprovementStatus.Complete:
                    progressTitle.Hide();
                    progressBar.Hide();
                    constructButton.Hide();
                    underContructionLabel.Hide();
                    prioritizeHBox.Show();
                    workersPanelContainer.Show();
                    constructionPanelContainer.Hide();
                    removeImprovementButton.Show();
                    break;
                case AllEnums.CountyImprovementStatus.UnderConstruction:
                    progressTitle.Show();
                    progressBar.Show();
                    constructButton.Hide();
                    prioritizeHBox.Show();
                    workersPanelContainer.Hide();
                    constructionPanelContainer.Show();
                    remnantsForContructionCheckBox.Show();
                    adjustMaxBuildersHbox.Show();
                    underContructionLabel.Show();
                    removeImprovementButton.Hide();
                    break;
                default:
                    progressTitle.Hide();
                    progressBar.Hide();
                    prioritizeHBox.Hide();
                    workersPanelContainer.Hide();
                    constructionPanelContainer.Show();
                    remnantsForContructionCheckBox.Hide();
                    adjustMaxBuildersHbox.Hide();
                    underContructionLabel.Hide();
                    removeImprovementButton.Hide();
                    CheckForConstructionResources();
                    break;
            }
        }

        private void CheckForConstructionResources()
        {
            Banker banker = new();
            if (banker.CheckBuildingCost(Globals.Instance.playerFactionData, countyImprovementData))
            {
                constructButton.Show();
            }
            else
            {
                constructButton.Hide();
            }
        }

        public void UpdateImprovementLabels()
        {
            UpdateConstructionStatus();

            GD.Print("Number of people working at county improvement: " + countyImprovementData.populationAtImprovement.Count);
            improvementTextureRect.Texture = countyImprovementData.improvementTexture;
            improvementNameLabel.Text = countyImprovementData.improvementName;
            improvementDescriptionLabel.Text = countyImprovementData.improvementDescription;

            UpdateBuilderNumberLabels();
            UpdateWorkerNumberLabels();
        }

        private void UpdateBuilderNumberLabels()
        {
            currentBuildersNumberLabel.Text = countyImprovementData.populationAtImprovement.Count.ToString();
            adjustedBuildersNumberLabel.Text = countyImprovementData.adjustedMaxBuilders.ToString();
            maxBuildersNumberLabel.Text = $"({countyImprovementData.maxBuilders})";
        }

        private void UpdateWorkerNumberLabels()
        {
            currentWorkersNumberLabel.Text = countyImprovementData.populationAtImprovement.Count.ToString();
            adjustedWorkersNumberLabel.Text = countyImprovementData.adjustedMaxWorkers.ToString();
            maxWorkersNumberLabel.Text = $"({countyImprovementData.maxWorkers})";
        }

        private void MinusMaxBuildersButtonPressed()
        {
            countyImprovementData.AdjustNumberOfBuilders(-1);
            UpdateAdjustedNumberLabels();
        }

        private void PlusMaxBuildersButtonPressed()
        {
            countyImprovementData.AdjustNumberOfBuilders(1);
            UpdateAdjustedNumberLabels();
        }

        private void MinusMaxWorkersButtonPressed()
        {
            countyImprovementData.AdjustNumberOfWorkers(-1);
            UpdateAdjustedNumberLabels();
        }

        private void PlusMaxWorkersButtonPressed()
        {
            countyImprovementData.AdjustNumberOfWorkers(1);
            UpdateAdjustedNumberLabels();
        }

        /// <summary>
        /// Updates both just because why not, instead of having it more complicated.
        /// </summary>
        private void UpdateAdjustedNumberLabels()
        {
            currentBuildersNumberLabel.Text = countyImprovementData.populationAtImprovement.Count.ToString();
            adjustedBuildersNumberLabel.Text = countyImprovementData.adjustedMaxBuilders.ToString();
            currentWorkersNumberLabel.Text = countyImprovementData.populationAtImprovement.Count.ToString();
            adjustedWorkersNumberLabel.Text = countyImprovementData.adjustedMaxWorkers.ToString();
        }

        private void ConstructButtonPressed()
        {
            GD.Print("You have pressed the county improvement button.");
            CountyImprovementsControl.Instance.buildConfirmationDialog.Visible = true;
            Globals.Instance.selectedPossibleBuildingControl = this;
        }
    }
}