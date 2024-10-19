using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace;

public partial class CountryImprovementPanelContainer : PanelContainer
{
    [Export] public CountyImprovementData countyImprovementData;

    [Export] PackedScene goodPanelContainerPackedScene;

    [Export] Label researchAssignedLabel;
    [ExportGroup("Progress")]
    [Export] public PanelContainer progressPanelContainer;
    [Export] public Label progressTitle;
    [Export] public ProgressBar progressBar;
    [Export] public Label constructionCost;

    [ExportGroup("Prioritize")]
    [Export] HBoxContainer prioritizeHBox;
    [Export] public CheckBox prioritizeCheckBox;

    [ExportGroup("Improvement Info")]
    [Export] public Label improvementNameLabel;
    [Export] Label improvementDescriptionLabel;
    [Export] TextureRect improvementTextureRect;

    [ExportGroup("Workers")]
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

    [ExportGroup("Construction")]
    [Export] PanelContainer constructionPanelContainer;
    [Export] Label constructionTitleLabel;
    [Export] GridContainer constructionMaterialCostGridContainer;
    //[Export] public CheckBox remnantsForContructionCheckBox;
    [Export] public HBoxContainer adjustMaxBuildersHbox;
    [Export] public Label currentBuildersNumberLabel;
    [Export] public Label adjustedBuildersNumberLabel;
    [Export] public Label maxBuildersNumberLabel;
    [Export] public Button minusBuilderButton;
    [Export] public Button plusBuilderButton;
    [Export] Button constructButton;
    //[Export] private Label hiringLabel;
    [ExportGroup("Research")]
    [Export] Label assignResearcherInPanelLabel;
    [Export] public Label underContructionLabel;
    [Export] Button removeImprovementButton;

    public override void _Ready()
    {
        CallDeferred(nameof(UpdateImprovementLabels));
    }

    private void UpdateConstructionStatus()
    {
        GD.Print($"County Improvement Data: " + countyImprovementData);
        HideEverything();
        switch (countyImprovementData.status)
        {
            // This is currently working in the research description panel.
            case AllEnums.CountyImprovementStatus.InResearchPanel:
                GenerateOutputGoods();
                GenerateInputGoods();
                constructionPanelContainer.Show();
                GenerateConstructionCosts();
                break;
            // This one seems to be working for normal improvements and storage as well.
            case AllEnums.CountyImprovementStatus.Producing:
                progressPanelContainer.Show();
                GenerateOutputGoods();
                GenerateInputGoods();
                prioritizeHBox.Show();
                CheckForMaxWorkers();
                removeImprovementButton.Show();
                break;
            case AllEnums.CountyImprovementStatus.Researching:
                GenerateOutputGoods();
                GenerateInputGoods();
                prioritizeHBox.Show();
                workersPanelContainer.Show();
                CheckIfResearchIsAssigned(); // Progress PanelContainer is controlled in here.
                removeImprovementButton.Show();
                break;
            case AllEnums.CountyImprovementStatus.UnderConstruction:
                progressPanelContainer.Show();
                prioritizeHBox.Show();
                GenerateOutputGoods();
                GenerateInputGoods();
                constructionPanelContainer.Show();
                GenerateConstructionCosts();
                adjustMaxBuildersHbox.Show();
                underContructionLabel.Show();
                UpdateConstructionProgress();
                removeImprovementButton.Show();
                break;
            // Default is status none, so when it is in the possible list.
            default:
                GenerateOutputGoods();
                GenerateInputGoods();
                constructionPanelContainer.Show();
                GenerateConstructionCosts();
                CheckForConstructionResources();
                break;
        }
    }

    private void UpdateConstructionProgress()
    {
        progressBar.MaxValue = countyImprovementData.maxAmountOfConstruction;
        progressBar.Value = countyImprovementData.CurrentAmountOfConstruction;
        constructionCost.Text = $"{Tr("PHRASE_CONSTRUCTION_COST")} : " +
            $"{countyImprovementData.CurrentAmountOfConstruction} / {countyImprovementData.maxAmountOfConstruction}";
    }

    private void HideEverything()
    {
        prioritizeHBox.Hide();
        progressPanelContainer.Hide();
        workersPanelContainer.Hide();
        constructButton.Hide();
        constructionPanelContainer.Hide();
        adjustMaxBuildersHbox.Hide();
        underContructionLabel.Hide();
        removeImprovementButton.Hide();
        assignResearcherInPanelLabel.Hide();
        researchAssignedLabel.Hide();
    }

    private void CheckForMaxWorkers()
    {
        if (countyImprovementData.maxWorkers > 0)
        {
            prioritizeHBox.Show();
            workersPanelContainer.Show();
        }
        else
        {
            prioritizeHBox.Hide();
            workersPanelContainer.Hide();
        }
    }

    /// <summary>
    /// There can only be one researcher assigned per Research Office, or Lab.
    /// </summary>
    private void CheckIfResearchIsAssigned()
    {
        if(countyImprovementData.populationAtImprovement.Count == 0)
        {
            return;
        }
        CountyPopulation countyPopulation = countyImprovementData.populationAtImprovement[0];
        if (countyPopulation.currentResearchItemData == null)
        {
            assignResearcherInPanelLabel.Show();
            return;
        }
        researchAssignedLabel.Text = $"{countyPopulation.firstName} {countyPopulation.lastName}" +
            $" {Tr("PHRASE_IS_RESEARCHING")} {Tr(countyPopulation.currentResearchItemData.researchName)}";
        researchAssignedLabel.Show();
        progressPanelContainer.Show();
        progressTitle.Text = "PHRASE_RESEARCH_PROGRESS";
        progressBar.MaxValue = countyPopulation.currentResearchItemData.costOfResearch;
        progressBar.Value = countyPopulation.currentResearchItemData.AmountOfResearchDone;
        assignResearcherInPanelLabel.Hide();
    }

    private void GenerateOutputGoods()
    {
        CheckForGoodsForColumns(outputsGridContainer
            , countyImprovementData.factionOutputGoods, countyImprovementData.countyOutputGoods);

        foreach (KeyValuePair<FactionResourceData, int> keyValuePair in countyImprovementData.factionOutputGoods)
        {
            GoodPanelContainer goodPanelContainer = AddGoodsPanel(keyValuePair, keyValuePair.Key.name, outputsGridContainer);

            // You can't use remnants to replace faction resources.
            goodPanelContainer.useRemnantsCheckBox.Hide();

            // We aren't using this right now, but it is quite possible we will use it in the future.
            goodPanelContainer.onlyProduceCheckBox.Hide();

            if (keyValuePair.Key.resourceType == AllEnums.FactionResourceType.Research)
            {
                produceAsNeededCheckBox.Hide();
            }
        }

        foreach (KeyValuePair<CountyResourceData, int> keyValuePair in countyImprovementData.countyOutputGoods)
        {
            GoodPanelContainer goodPanelContainer = AddGoodsPanel(keyValuePair, keyValuePair.Key.name, outputsGridContainer);

            goodPanelContainer.useRemnantsCheckBox.Hide();

            // We aren't using this right now, but it is quite possible we will use it in the future.
            goodPanelContainer.onlyProduceCheckBox.Hide();
        }
    }
    private void GenerateInputGoods()
    {
        CheckForGoodsForColumns(inputsGridContainer
            , countyImprovementData.factionInputGoods
            , countyImprovementData.countyInputGoods);

        foreach (KeyValuePair<FactionResourceData, int> keyValuePair in countyImprovementData.factionInputGoods)
        {
            GoodPanelContainer goodPanelContainer = AddGoodsPanel(keyValuePair, keyValuePair.Key.name, inputsGridContainer);

            // You can't use remnants to replace faction resources.
            goodPanelContainer.useRemnantsCheckBox.Hide();
        }

        foreach (KeyValuePair<CountyResourceData, int> keyValuePair in countyImprovementData.countyInputGoods)
        {
            GoodPanelContainer goodPanelContainer = AddGoodsPanel(keyValuePair, keyValuePair.Key.name, inputsGridContainer);

            if (countyImprovementData.status == AllEnums.CountyImprovementStatus.UnderConstruction)
            {
                goodPanelContainer.useRemnantsCheckBox.Hide();
            }
            else
            {
                CheckForHideUseRemnants(keyValuePair, goodPanelContainer);
            }
        }
    }

    GoodPanelContainer AddGoodsPanel<T>(KeyValuePair<T, int> keyValuePair, string name, GridContainer goodsParentGridContainer)
    {
        GoodPanelContainer goodPanelContainer = (GoodPanelContainer)goodPanelContainerPackedScene.Instantiate();
        goodsParentGridContainer.AddChild(goodPanelContainer);
        goodPanelContainer.goodLabel.Text = $"{Tr(name)} : {keyValuePair.Value}";

        return goodPanelContainer;
    }

    void GenerateConstructionCosts()
    {
        CheckForGoodsForColumns(constructionMaterialCostGridContainer
            , countyImprovementData.factionResourceConstructionCost
            , countyImprovementData.countyResourceConstructionCost);

        foreach (KeyValuePair<FactionResourceData, int> keyValuePair in countyImprovementData.factionResourceConstructionCost)
        {
            GoodPanelContainer goodPanelContainer = AddGoodsPanel(keyValuePair, keyValuePair.Key.name, constructionMaterialCostGridContainer);

            // You can't use remnants to replace faction resources.
            goodPanelContainer.useRemnantsCheckBox.Hide();
        }

        foreach (KeyValuePair<CountyResourceData, int> keyValuePair in countyImprovementData.countyResourceConstructionCost)
        {
            GoodPanelContainer goodPanelContainer = AddGoodsPanel(keyValuePair, keyValuePair.Key.name, constructionMaterialCostGridContainer);

            CheckForHideUseRemnants(keyValuePair, goodPanelContainer);
        }
    }

    private void CheckForHideUseRemnants(KeyValuePair<CountyResourceData, int> keyValuePair
        , GoodPanelContainer goodPanelContainer)
    {
        bool shouldHideCheckBox =
            countyImprovementData.status == AllEnums.CountyImprovementStatus.None
            || countyImprovementData.status == AllEnums.CountyImprovementStatus.InResearchPanel
            || keyValuePair.Key.countyResourceType == AllEnums.CountyResourceType.Remnants
            || keyValuePair.Key.factionResourceType == AllEnums.FactionResourceType.Food;

        if (shouldHideCheckBox)
        {
            goodPanelContainer.useRemnantsCheckBox.Hide();
        }
        else
        {
            goodPanelContainer.useRemnantsCheckBox.Show();
        }
    }

    private void CheckForGoodsForColumns(GridContainer gridContainer
        , Godot.Collections.Dictionary<FactionResourceData, int> factionResources
        , Godot.Collections.Dictionary<CountyResourceData, int> countyResources)
    {
        int totalGoods = factionResources.Count
            + countyResources.Count;
        if (totalGoods == 1)
        {
            gridContainer.Columns = 1;
        }
        if (totalGoods == 0)
        {
            GoodPanelContainer goodPanelContainer = (GoodPanelContainer)goodPanelContainerPackedScene.Instantiate();
            gridContainer.AddChild(goodPanelContainer);
            gridContainer.Columns = 1;
            goodPanelContainer.goodLabel.Text = $"{Tr("WORD_NONE")}";
            return;
        }
    }

    /// <summary>
    /// Currently passing in the player faction data, if the AI needs to use this then we need to have
    /// it pass in a different factionData.
    /// </summary>
    private void CheckForConstructionResources()
    {
        Banker banker = new();
        if (banker.CheckBuildingCost(Globals.Instance.playerFactionData
            , Globals.Instance.SelectedLeftClickCounty.countyData, countyImprovementData))
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
        if (string.IsNullOrEmpty(countyImprovementData.nonTangibleGoodProduced))
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
        if (countyImprovementData.numberBuilt == 0)
        {
            improvementNameLabel.Text = $"{Tr(countyImprovementData.improvementName)}";
        }
        else
        {
            improvementNameLabel.Text = $"{Tr(countyImprovementData.improvementName)} " +
                $"{countyImprovementData.numberBuilt}";
        }
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