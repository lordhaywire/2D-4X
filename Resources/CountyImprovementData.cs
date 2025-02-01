using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;

[GlobalClass]
public partial class CountyImprovementData : Resource
{
    [ExportGroup("Not For Inspector")]
    [Export] private int currentAmountOfCounstruction;
    [Export] public int numberBuilt; // This is used to number the improvement name in the County Improvement Panel.

    [ExportGroup("Improvement Info")]
    [Export] public AllEnums.CountyImprovementType countyImprovementType;
    [Export] public bool prioritize;

    [Export] public Texture2D improvementTexture;
    [Export] public string improvementName;
    [Export] public string improvementDescription;

    [ExportGroup("Skill and Interest")]
    [Export] public AllEnums.Skills workSkill;
    [Export] public InterestData interestData;

    [ExportGroup("Construction Costs")]
    [Export] public Godot.Collections.Dictionary<GoodData, int> goodsConstructionCost = [];

    [Export]
    public int CurrentAmountOfConstruction
    {
        get { return currentAmountOfCounstruction; }
        set
        {
            currentAmountOfCounstruction = Math.Min(value, maxAmountOfConstruction);
        }
    }
    [Export] public int maxAmountOfConstruction;
    [Export] public int maxBuilders;
    [Export] public int adjustedMaxBuilders;
    [Export] public int maxWorkers;
    [Export] public int adjustedMaxWorkers;

    [ExportGroup("Resource Types")]
    [Export] public AllEnums.CountyGoodType countyResourceType;
    [Export] public AllEnums.FactionGoodType factionResourceType;

    [ExportGroup("Outputs")]
    [Export] public Godot.Collections.Dictionary<GoodData, ProductionData> outputGoods = [];
    [Export] public int allDailyWorkAmountAtImprovementCompleted;

    // All input goods that are need to create the finished good.
    [ExportGroup("Inputs")]
    // Unqiue input goods are needed for the county improvement container so that the GoodData can remember
    // changes such as Use Remnants that the player or AI does.
    [Export] public Godot.Collections.Dictionary<GoodData, int> uniqueInputGoods = [];
    [Export] public Godot.Collections.Dictionary<GoodData, int> inputGoods = [];
    [Export] public Godot.Collections.Dictionary<AllEnums.CountyGoodType, int> countyStockpiledGoods = [];

    [Export] public AllEnums.CountyImprovementStatus status;
    [Export] public Godot.Collections.Array<PopulationData> populationAtImprovement = [];

    public void AdjustNumberOfBuilders(int adjustment)
    {
        adjustedMaxBuilders += adjustment;
        adjustedMaxBuilders = Math.Clamp(adjustedMaxBuilders, 0, maxBuilders);
        if (adjustedMaxBuilders < populationAtImprovement.Count)
        {
            // Remove lowest skilled worker.
            PopulationData lowestSkilledPopulation = GetLowestSkilledPopulation(true);
            lowestSkilledPopulation.RemoveFromCountyImprovement();
        }
    }

    public string GetCountyImprovementName()
    {
        string name;
        if (numberBuilt == 0)
        {
            name = $"{Tr(improvementName)}";
        }
        else
        {
            name = $"{Tr(improvementName)} {numberBuilt}";
        }
        return name;
    }
    private PopulationData GetLowestSkilledPopulation(bool constructing)
    {
        AllEnums.Skills skill;
        if (constructing)
        {
            skill = AllEnums.Skills.Construction;
        }
        else
        {
            skill = workSkill;
        }
        // Remove the lowest skilled worker.
        List<PopulationData> sortedLowestSkillLevelPopulation
            = [.. populationAtImprovement.OrderBy(pop => pop.skills[skill].skillLevel)];
        PopulationData lowestSkilledPopulation = sortedLowestSkillLevelPopulation.FirstOrDefault();
        return lowestSkilledPopulation;

    }

    public int CountNumberOfGoodsGettingProduced()
    {
        int numberOfGoodsGettingProduced = outputGoods.Count;
        return numberOfGoodsGettingProduced;
    }

    /// <summary>
    /// MaxWorkers * Global Amount of Daily Work without bonus / work cost = average daily amount generated.
    /// </summary>
    /// <param name="productionData"></param>
    public void GenerateGoodsProducedWithoutBonusesForUI(ProductionData productionData)
    {
        if (countyImprovementType != AllEnums.CountyImprovementType.Storage)
        {
            // Get all of the work and then divide it by the number of resources.
            int workAmount = maxWorkers * Globals.Instance.dailyWorkAmount
                / productionData.workCost;
            
            productionData.AverageDailyGoodsAmountGenerated = workAmount;
        }
        else
        {
            productionData.AverageDailyGoodsAmountGenerated = productionData.storageAmount;
        }
    }
    public bool CheckIfWorkersFull()
    {
        if(populationAtImprovement.Count >= adjustedMaxWorkers)
        {
            return true;
        }
        return false;
    }
    public bool CheckIfResearchImprovement()
    {
        if (factionResourceType == AllEnums.FactionGoodType.Research)
        {
            return true;
        }
        return false;
    }
    public bool CheckIfStorageImprovement()
    {
        if (countyResourceType == AllEnums.CountyGoodType.StorageNonperishable
            || countyResourceType == AllEnums.CountyGoodType.StoragePerishable)
        {
            return true;
        }
        return false;
    }
    public void AdjustNumberOfWorkers(int adjustment)
    {
        adjustedMaxWorkers += adjustment;
        adjustedMaxWorkers = Math.Clamp(adjustedMaxWorkers, 0, maxWorkers);
        if (adjustedMaxWorkers < populationAtImprovement.Count)
        {
            // Remove lowest skilled worker.
            PopulationData lowestSkilledPopulation = GetLowestSkilledPopulation(false);
            lowestSkilledPopulation.RemoveFromCountyImprovement();
        }
    }

    public bool CheckIfCountyImprovementDone()
    {
        if (CurrentAmountOfConstruction >= maxAmountOfConstruction)
        {
            return true;
        }
        return false;
    }

    public bool CheckIfStatusLowStockpiledGoods()
    {
        if (status == AllEnums.CountyImprovementStatus.LowStockpiledGoods)
        {
            return true;
        }
        return false;
    }
    public void AddPopulationToPopulationAtImprovementList(PopulationData populationData)
    {
        // GD.Print($"{populationData.firstName} was added to {improvementName}'s list {populationAtImprovement.Count}.");
        populationAtImprovement.Add(populationData);
    }

    public void RemovePopulationFromPopulationAtImprovementList(PopulationData populationData)
    {
        populationAtImprovement.Remove(populationData);
    }

    /// <summary>
    /// This could have been an if else but I think we will add more types.
    /// </summary>
    public void SetCountyImprovementComplete(CountyData countyData)
    {
        switch (countyImprovementType)
        {
            case AllEnums.CountyImprovementType.Research:
                status = AllEnums.CountyImprovementStatus.Researching;
                AddResearchOfficeToFactionResearchOfficeList(countyData.factionData);
                break;
            case AllEnums.CountyImprovementType.Storage:
                Haulmaster.AddImprovementStorageToCounty(countyData, this);
                status = AllEnums.CountyImprovementStatus.Producing;
                break;
            case AllEnums.CountyImprovementType.Standard:
                status = AllEnums.CountyImprovementStatus.Producing;
                allDailyWorkAmountAtImprovementCompleted = 0;
                break;
        }
    }

    private void AddResearchOfficeToFactionResearchOfficeList(FactionData factionData)
    {
        factionData.researchOffices.Add(this);
    }
    public void SetCountyImprovementStatus(AllEnums.CountyImprovementStatus newStatus)
    {
        status = newStatus;
    }

    public static CountyImprovementData NewCopy(CountyImprovementData countyImprovementData)
    {
        CountyImprovementData newCountyImprovementData = new()
        {
            countyImprovementType = countyImprovementData.countyImprovementType,
            prioritize = countyImprovementData.prioritize,
            improvementTexture = countyImprovementData.improvementTexture,
            improvementName = countyImprovementData.improvementName,
            improvementDescription = countyImprovementData.improvementDescription,
            numberBuilt = countyImprovementData.numberBuilt,
            workSkill = countyImprovementData.workSkill,
            interestData = countyImprovementData.interestData,
            outputGoods = countyImprovementData.CopyOutputGoods(),
            allDailyWorkAmountAtImprovementCompleted = countyImprovementData.allDailyWorkAmountAtImprovementCompleted,
            goodsConstructionCost = countyImprovementData.goodsConstructionCost,
            currentAmountOfCounstruction = countyImprovementData.currentAmountOfCounstruction,
            CurrentAmountOfConstruction = countyImprovementData.CurrentAmountOfConstruction,
            maxAmountOfConstruction = countyImprovementData.maxAmountOfConstruction,
            maxBuilders = countyImprovementData.maxBuilders,
            adjustedMaxBuilders = countyImprovementData.adjustedMaxBuilders,
            maxWorkers = countyImprovementData.maxWorkers,
            adjustedMaxWorkers = countyImprovementData.adjustedMaxWorkers,
            countyResourceType = countyImprovementData.countyResourceType,
            factionResourceType = countyImprovementData.factionResourceType,
            uniqueInputGoods = countyImprovementData.CopyUniqueInputGoods(),
            inputGoods = countyImprovementData.inputGoods,
            countyStockpiledGoods = countyImprovementData.CopyStockpiledGoods(),
            status = countyImprovementData.status,
            populationAtImprovement = new Godot.Collections.Array<PopulationData>(countyImprovementData.populationAtImprovement),
        };
        return newCountyImprovementData;
    }

    public Godot.Collections.Dictionary<GoodData, int> CopyUniqueInputGoods()
    {
        Godot.Collections.Dictionary<GoodData, int> copiedDictionary = [];

        foreach (KeyValuePair<GoodData, int> keyValuePair in inputGoods) 
        {
            copiedDictionary.Add(keyValuePair.Key.NewCopy(keyValuePair.Key), keyValuePair.Value);
        }
        return copiedDictionary;
    }

    // We have to do a copy of a copy to make a copy that is unique.
    public Godot.Collections.Dictionary<AllEnums.CountyGoodType, int> CopyStockpiledGoods()
    {
        Godot.Collections.Dictionary<AllEnums.CountyGoodType, int> copiedDictionary = [];

        foreach (KeyValuePair<AllEnums.CountyGoodType, int> keyValuePair in countyStockpiledGoods)
        {
            copiedDictionary.Add(keyValuePair.Key, keyValuePair.Value);
        }
        return copiedDictionary;
    }
    
    // We have to do a copy of a copy to make a copy that is unique.
    // I bet this isn't really unique.
    public Godot.Collections.Dictionary<GoodData, ProductionData> CopyOutputGoods()
    {
        Godot.Collections.Dictionary<GoodData, ProductionData> copiedDictionary = [];

        foreach (KeyValuePair<GoodData, ProductionData> item in outputGoods)
        {
            copiedDictionary[item.Key] = item.Value.NewCopy(item.Value);
        }

        return copiedDictionary;
    }
}