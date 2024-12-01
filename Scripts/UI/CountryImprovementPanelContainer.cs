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
    [Export] public Label constructionCostLabel;
    [Export] public Label maxWorkersLabel;

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
    [Export] public Label goodsProducedPerDayTitleLabel;
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
        //GD.Print($"County Improvement Data: " + countyImprovementData);
        HideEverything();
        switch (countyImprovementData.status)
        {
            // This is currently working in the research description panel.
            case AllEnums.CountyImprovementStatus.InResearchPanel:
                UpdateConstructionCost();
                UpdateMaxWorkers();
                GenerateOutputGoods();
                GenerateInputGoods();
                constructionPanelContainer.Show();
                GenerateConstructionGoodsCosts();
                break;
            // This one seems to be working for normal improvements and storage as well.
            case AllEnums.CountyImprovementStatus.Producing:
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
                prioritizeHBox.Show();
                UpdateConstructionProgress();
                UpdateMaxWorkers();
                GenerateOutputGoods();
                GenerateInputGoods();
                constructionPanelContainer.Show();
                GenerateConstructionGoodsCosts();
                adjustMaxBuildersHbox.Show();
                underContructionLabel.Show();
                removeImprovementButton.Show();
                break;
            // Default is status none, so when it is in the possible list.
            default:
                UpdateConstructionCost();
                UpdateMaxWorkers();
                GenerateOutputGoods();
                GenerateInputGoods();
                constructionPanelContainer.Show();
                GenerateConstructionGoodsCosts();
                CheckForConstructionResources(Globals.Instance.SelectedLeftClickCounty.countyData);
                break;
        }
    }



    void UpdateConstructionCost()
    {
        progressPanelContainer.Show();
        progressBar.Hide();
        progressTitle.Hide();
        constructionCostLabel.Text = $"{Tr("PHRASE_CONSTRUCTION_COST")} : " +
            $"{countyImprovementData.maxAmountOfConstruction}";

    }

    void UpdateMaxWorkers()
    {
        maxWorkersLabel.Show();
        maxWorkersLabel.Text = $"{Tr("PHRASE_MAX_WORKERS")} : " +
            $"{countyImprovementData.maxWorkers}";
    }
    private void UpdateConstructionProgress()
    {
        progressPanelContainer.Show();
        progressBar.MaxValue = countyImprovementData.maxAmountOfConstruction;
        progressBar.Value = countyImprovementData.CurrentAmountOfConstruction;
        constructionCostLabel.Text = $"{Tr("PHRASE_CONSTRUCTION_COST")} : " +
            $"{countyImprovementData.CurrentAmountOfConstruction} / {countyImprovementData.maxAmountOfConstruction}";
    }

    private void HideEverything()
    {
        prioritizeHBox.Hide();
        maxWorkersLabel.Hide();
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
        if (countyImprovementData.populationAtImprovement.Count == 0)
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
           , countyImprovementData.outputGoods.Count);

        foreach (KeyValuePair<GoodData, ProductionData> keyValuePair in countyImprovementData.outputGoods)
        {
            GoodPanelContainer goodPanelContainer
                = AddOutputGoodsPanel(keyValuePair.Key, keyValuePair.Value, outputsGridContainer);

            goodPanelContainer.useRemnantsCheckBox.Hide();

            // We aren't using this right now, but it is quite possible we will use it in the future.
            goodPanelContainer.onlyProduceCheckBox.Hide();
        }
    }

    // ChatGPT wrote most of this.
    GoodPanelContainer AddOutputGoodsPanel(GoodData goodData, ProductionData productionData
        , GridContainer goodsParentGridContainer)
    {
        // Generate the goods produced without bonuses.
        // This is going to happen every time the player opens the county improvement panel, or research panel.
        countyImprovementData.GenerateGoodsProducedWithoutBonusesForUI(productionData);

        GoodPanelContainer goodPanelContainer = (GoodPanelContainer)goodPanelContainerPackedScene.Instantiate();
        goodsParentGridContainer.AddChild(goodPanelContainer);

        //GD.Print($"Average Daily Amount Generated: {productionData.AverageDailyAmountGenerated}");
        if (countyImprovementData.status != AllEnums.CountyImprovementStatus.Producing)
        {
            goodPanelContainer.goodLabel.Text = $"{Tr(goodData.goodName)} "
                + $": {productionData.AverageDailyGoodsAmountGenerated}";
        }
        else
        {
            goodsProducedPerDayTitleLabel.Text = "PHRASE_GOODS_PRODUCED_YESTERDAY";
            if (productionData.todaysGoodsAmountGenerated >= 1)
            {
                goodPanelContainer.goodLabel.Text = $"{Tr(goodData.goodName)} : {productionData.todaysGoodsAmountGenerated}";
            }
            else
            {
                goodPanelContainer.goodLabel.Text = $"{Tr(goodData.goodName)} "
                    + $": {productionData.workAmount} / {productionData.workCost}";
            }
        }
        return goodPanelContainer;
    }

    private void GenerateInputGoods()
    {
        CheckForGoodsForColumns(inputsGridContainer
            , countyImprovementData.inputGoods.Count);

        foreach (KeyValuePair<GoodData, int> keyValuePair in countyImprovementData.inputGoods)
        {
            GoodPanelContainer goodPanelContainer = AddInputGoodsPanel(keyValuePair, keyValuePair.Key.goodName, inputsGridContainer);

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

    // ChatGPT wrote part of this.
    GoodPanelContainer AddInputGoodsPanel<T>(KeyValuePair<T, int> keyValuePair, string name, GridContainer goodsParentGridContainer)
    {
        GoodPanelContainer goodPanelContainer = (GoodPanelContainer)goodPanelContainerPackedScene.Instantiate();
        goodsParentGridContainer.AddChild(goodPanelContainer);
        goodPanelContainer.goodLabel.Text = $"{Tr(name)} : {keyValuePair.Value}";

        return goodPanelContainer;
    }



    void GenerateConstructionGoodsCosts()
    {
        CheckForGoodsForColumns(constructionMaterialCostGridContainer
            , countyImprovementData.goodsConstructionCost.Count);

        foreach (KeyValuePair<GoodData, int> keyValuePair in countyImprovementData.goodsConstructionCost)
        {
            GoodPanelContainer goodPanelContainer = AddInputGoodsPanel(keyValuePair, keyValuePair.Key.goodName, constructionMaterialCostGridContainer);

            CheckForHideUseRemnants(keyValuePair, goodPanelContainer);
        }
    }

    private void CheckForHideUseRemnants(KeyValuePair<GoodData, int> keyValuePair
        , GoodPanelContainer goodPanelContainer)
    {
        bool shouldHideCheckBox =
            countyImprovementData.status == AllEnums.CountyImprovementStatus.None
            || countyImprovementData.status == AllEnums.CountyImprovementStatus.InResearchPanel
            || keyValuePair.Key.countyGoodType == AllEnums.CountyGoodType.Remnants
            || keyValuePair.Key.factionGoodType == AllEnums.FactionGoodType.Food;

        if (shouldHideCheckBox)
        {
            goodPanelContainer.useRemnantsCheckBox.Hide();
        }
        else
        {
            goodPanelContainer.useRemnantsCheckBox.Show();
        }
    }

    /// <summary>
    /// Changes the number of columns in the grid so that it looks good.
    /// </summary>
    /// <param name="gridContainer"></param>
    /// <param name="goodsCount"></param>
    private void CheckForGoodsForColumns(GridContainer gridContainer
        , int goodsCount)
    {
        int totalGoods = goodsCount;
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
    private void CheckForConstructionResources(CountyData countyData)
    {
        Banker banker = new();
        if (banker.CheckBuildingCost(countyData, countyImprovementData))
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
        //GD.Print("Prioritized? " + countyImprovementData.prioritize);
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

    private void UpdateInformationLabels()
    {
        //GD.Print("Number of people working at county improvement: " + countyImprovementData.populationAtImprovement.Count);
        improvementTextureRect.Texture = countyImprovementData.improvementTexture;
        improvementNameLabel.Text = countyImprovementData.GetCountyImprovementName();
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
        //GD.Print("You have pressed the county improvement button.");
        CountyImprovementsControl.Instance.buildConfirmationDialog.Visible = true;
        Globals.Instance.selectedPossibleBuildingControl = this;
    }
}