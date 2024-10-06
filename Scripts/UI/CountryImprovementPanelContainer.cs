using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace;

public partial class CountryImprovementPanelContainer : PanelContainer
{
    [Export] public CountyImprovementData countyImprovementData;

    [Export] PackedScene goodPanelContainerPackedScene;
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
    [Export] GridContainer outputsGridContainer;
    [Export] GridContainer inputsGridContainer;
    [Export] public Label nontangibleProductionLabel;
    [Export] PanelContainer constructionPanelContainer;
    [Export] Label constructionTitleLabel;
    [Export] GridContainer constructionMaterialCostGridContainer;
    [Export] public CheckBox remnantsForContructionCheckBox;
    [Export] public HBoxContainer adjustMaxBuildersHbox;
    [Export] public Label currentBuildersNumberLabel;
    [Export] public Label adjustedBuildersNumberLabel;
    [Export] public Label maxBuildersNumberLabel;
    [Export] public Button minusBuilderButton;
    [Export] public Button plusBuilderButton;
    [Export] private Button constructButton;
    [Export] private Label hiringLabel;
    [Export] private Label assignResearcherInPanelLabel;
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
            case AllEnums.CountyImprovementStatus.Producing:
                progressTitle.Hide();
                progressBar.Hide();
                constructButton.Hide();
                GenerateOutputGoods();
                GenerateInputGoods();
                prioritizeHBox.Show();
                workersPanelContainer.Show();
                constructionPanelContainer.Hide();
                underContructionLabel.Hide();
                assignResearcherInPanelLabel.Hide();
                removeImprovementButton.Show();
                break;
            case AllEnums.CountyImprovementStatus.ProducingWithoutWorkers:
                progressTitle.Show(); // This needs to be changed to production progress.
                progressBar.Show();
                prioritizeHBox.Show();
                workersPanelContainer.Hide();
                UpdateOutputGoodsProducing();
                constructionPanelContainer.Hide();
                remnantsForContructionCheckBox.Hide();
                adjustMaxBuildersHbox.Hide();
                underContructionLabel.Hide();
                hiringLabel.Hide();
                assignResearcherInPanelLabel.Hide();
                removeImprovementButton.Show();
                break;
            case AllEnums.CountyImprovementStatus.Researching:
                break;
            case AllEnums.CountyImprovementStatus.AwaitingPlayerAssignment:
                progressTitle.Hide();
                progressBar.Hide();
                constructButton.Hide();
                underContructionLabel.Hide();
                prioritizeHBox.Show();
                workersPanelContainer.Show();
                constructionPanelContainer.Hide();
                removeImprovementButton.Show();
                assignResearcherInPanelLabel.Show();
                underContructionLabel.Hide();
                break;
            case AllEnums.CountyImprovementStatus.UnderConstruction:
                progressTitle.Show();
                progressBar.Show();
                prioritizeHBox.Show();
                workersPanelContainer.Hide();
                UpdateOutputGoodsNotProducing(); // What is this?
                GenerateOutputGoods();
                GenerateInputGoods();
                constructionPanelContainer.Show();
                GenerateConstructionCosts();
                remnantsForContructionCheckBox.Show();
                adjustMaxBuildersHbox.Show();
                hiringLabel.Hide(); // I think we are going to get rid of this.
                assignResearcherInPanelLabel.Hide();
                underContructionLabel.Show();
                constructButton.Hide();
                removeImprovementButton.Hide();
                break;
            default:
                progressTitle.Hide();
                progressBar.Hide();
                prioritizeHBox.Hide();
                workersPanelContainer.Hide();
                UpdateOutputGoodsNotProducing(); // We need to change.
                GenerateOutputGoods();
                GenerateInputGoods();
                constructionPanelContainer.Show();
                GenerateConstructionCosts();
                CheckForConstructionResources();
                remnantsForContructionCheckBox.Hide();
                adjustMaxBuildersHbox.Hide();
                underContructionLabel.Hide();
                removeImprovementButton.Hide();
                assignResearcherInPanelLabel.Hide();
                break;
        }
    }

    private void GenerateOutputGoods()
    {
        foreach (KeyValuePair<FactionResourceData, int> keyValuePair in countyImprovementData.factionOutputGoods)
        {
            GoodPanelContainer goodPanelContainer = (GoodPanelContainer)goodPanelContainerPackedScene.Instantiate();
            outputsGridContainer.AddChild(goodPanelContainer);
            goodPanelContainer.goodLabel.Text = $"{Tr(keyValuePair.Key.name)} : {keyValuePair.Value}";
            goodPanelContainer.useRemnantsCheckBox.Hide();
            if (countyImprovementData.status == AllEnums.CountyImprovementStatus.Producing)
            {
                goodPanelContainer.onlyProduceCheckBox.Show();
            }
            else
            {
                goodPanelContainer.onlyProduceCheckBox.Hide();
            }
        }
        foreach (KeyValuePair<CountyResourceData, int> keyValuePair in countyImprovementData.countyOutputGoods)
        {
            GoodPanelContainer goodPanelContainer = (GoodPanelContainer)goodPanelContainerPackedScene.Instantiate();
            outputsGridContainer.AddChild(goodPanelContainer);
            goodPanelContainer.goodLabel.Text = $"{Tr(keyValuePair.Key.name)} : {keyValuePair.Value}";
            goodPanelContainer.useRemnantsCheckBox.Hide();
            if (countyImprovementData.status == AllEnums.CountyImprovementStatus.Producing)
            {
                goodPanelContainer.onlyProduceCheckBox.Show();
            }
            else
            {
                goodPanelContainer.onlyProduceCheckBox.Hide();
            }
        }
    }
    private void GenerateInputGoods()
    {
        foreach (KeyValuePair<CountyResourceData, int> keyValuePair in countyImprovementData.inputGoods)
        {
            GoodPanelContainer goodPanelContainer = (GoodPanelContainer)goodPanelContainerPackedScene.Instantiate();
            inputsGridContainer.AddChild(goodPanelContainer);
            goodPanelContainer.goodLabel.Text = $"{Tr(keyValuePair.Key.name)} : {keyValuePair.Value}";
            if (countyImprovementData.status == AllEnums.CountyImprovementStatus.Producing)
            {
                goodPanelContainer.useRemnantsCheckBox.Show();
            }
            else
            {
                goodPanelContainer.useRemnantsCheckBox.Hide();
            }
        }
    }

    private void GenerateConstructionCosts()
    {
        int totalGoods = countyImprovementData.factionResourceConstructionCost.Count
            + countyImprovementData.countyResourceConstructionCost.Count;
        if(totalGoods == 1)
        {
            constructionMaterialCostGridContainer.Columns = 1;
        }
        if (totalGoods == 0)
        {
            GoodPanelContainer goodPanelContainer = (GoodPanelContainer)goodPanelContainerPackedScene.Instantiate();
            constructionMaterialCostGridContainer.AddChild(goodPanelContainer);
            constructionMaterialCostGridContainer.Columns = 1;
            goodPanelContainer.goodLabel.Text = $"{Tr("WORD_NONE")}";
            return;
        }
        else 
        { 

            foreach (KeyValuePair<FactionResourceData, int> keyValuePair in countyImprovementData.factionResourceConstructionCost)
            {
                GoodPanelContainer goodPanelContainer = (GoodPanelContainer)goodPanelContainerPackedScene.Instantiate();
                constructionMaterialCostGridContainer.AddChild(goodPanelContainer);
                goodPanelContainer.goodLabel.Text = $"{Tr(keyValuePair.Key.name)} : {keyValuePair.Value}";
                // We can probably get rid of this, as long as the goodPanelContainerPackedScene has the use Remnants box hidden.
                if (countyImprovementData.status == AllEnums.CountyImprovementStatus.None)
                {
                    goodPanelContainer.useRemnantsCheckBox.Hide();
                }
            }

            foreach (KeyValuePair<CountyResourceData, int> keyValuePair in countyImprovementData.countyResourceConstructionCost)
            {
                GoodPanelContainer goodPanelContainer = (GoodPanelContainer)goodPanelContainerPackedScene.Instantiate();
                constructionMaterialCostGridContainer.AddChild(goodPanelContainer);
                goodPanelContainer.goodLabel.Text = $"{Tr(keyValuePair.Key.name)} : {keyValuePair.Value}";
                if (countyImprovementData.status == AllEnums.CountyImprovementStatus.None)
                {
                    goodPanelContainer.useRemnantsCheckBox.Hide();
                }
            }
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

    private void PrioritizeCheckbox()
    {
        countyImprovementData.prioritize = !countyImprovementData.prioritize;
        GD.Print("Prioritized? " + countyImprovementData.prioritize);
    }
    public void UpdateImprovementLabels()
    {
        UpdatePrioritizeCheckbox();
        UpdateConstructionStatus();
        UpdateInformationLabels();
        UpdateBuilderNumberLabels();
        UpdateWorkerNumberLabels();
    }

    private void UpdatePrioritizeCheckbox()
    {
        prioritizeCheckBox.ButtonPressed = countyImprovementData.prioritize;
    }

    private void UpdateOutputGoodsProducing()
    {
        GD.Print("County Improvement Nontangible: " + countyImprovementData.nonTangibleGoodProduced);
        if (string.IsNullOrEmpty(countyImprovementData.nonTangibleGoodProduced))
        {
            outputsGridContainer.Show();
            nontangibleProductionLabel.Hide();
        }
        else
        {
            outputsGridContainer.Hide();
            nontangibleProductionLabel.Text = countyImprovementData.nonTangibleGoodProduced;
            nontangibleProductionLabel.Show();
        }
    }
    private void UpdateOutputGoodsNotProducing()
    {
        GD.Print("County Improvement Nontangible: " + countyImprovementData.nonTangibleGoodProduced);
        if(string.IsNullOrEmpty(countyImprovementData.nonTangibleGoodProduced))
        {
            outputsGridContainer.Show();
            nontangibleProductionLabel.Hide();
        }
        else
        {
            outputsGridContainer.Hide();
            nontangibleProductionLabel.Text = countyImprovementData.nonTangibleGoodNotBeingProduced;
            nontangibleProductionLabel.Show();
        }
    }

    private void UpdateInformationLabels()
    {
        GD.Print("Number of people working at county improvement: " + countyImprovementData.populationAtImprovement.Count);
        improvementTextureRect.Texture = countyImprovementData.improvementTexture;
        improvementNameLabel.Text = countyImprovementData.improvementName;
        improvementDescriptionLabel.Text = countyImprovementData.improvementDescription;
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