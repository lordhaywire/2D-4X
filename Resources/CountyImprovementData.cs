using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class CountyImprovementData : Resource
    {
        [ExportGroup("Not For Inspector")]
        [Export] private int currentAmountOfCounstruction;
        [Export] public float workAmountForEachResource;
        [Export] public int numberBuilt; // This is used to number the improvement name in the County Improvement Panel.

        [ExportGroup("Improvement Info")]
        [Export] public AllEnums.CountyImprovementType countyImprovementType;
        //[Export] public int location;
        [Export] public bool prioritize;
        // This can't be exported so we need to change how it works. It doesn't even look like I am using this.
        //public CountryImprovementPanelContainer countyImprovementPanelContainer;
        [Export] public Texture2D improvementTexture;
        [Export] public string improvementName;
        [Export] public string improvementDescription;

        [ExportGroup("Skill and Interest")]
        [Export] public AllEnums.Skills workSkill;
        [Export] public InterestData interest;

        [ExportGroup("Construction Costs")]
        [Export] public Godot.Collections.Dictionary<FactionResourceData, int> factionResourceConstructionCost;
        [Export] public Godot.Collections.Dictionary<CountyResourceData, int> countyResourceConstructionCost;


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
        [Export] public AllEnums.CountyResourceType countyResourceType;
        [Export] public AllEnums.FactionResourceType factionResourceType;

        [ExportGroup("Outputs")]
        [Export] public Godot.Collections.Dictionary<FactionResourceData, ProductionData> factionOutputGoods = [];
        // Resource and work amount cost.
        [Export] public Godot.Collections.Dictionary<CountyResourceData, ProductionData> countyOutputGoods = [];
        [Export] public int allDailyWorkAmountAtImprovementCompleted;

        [Export] public int dailyResourceGenerationAmount; // I am pretty sure these are not used.
        [Export] public int dailyResourceGenerationBonus; // I am pretty sure these are not used.

        // All input goods that are need to create the finished good.
        // For some reason this one needs to be initialized, but the faction and county construction costs don't.
        [ExportGroup("Inputs")]
        [Export] public Godot.Collections.Dictionary<FactionResourceData, int> factionInputGoods = [];
        [Export] public Godot.Collections.Dictionary<CountyResourceData, int> countyInputGoods = [];

        [Export] public AllEnums.CountyImprovementStatus status;
        public List<CountyPopulation> populationAtImprovement = [];

        public void AdjustNumberOfBuilders(int adjustment)
        {
            adjustedMaxBuilders += adjustment;
            adjustedMaxBuilders = Math.Clamp(adjustedMaxBuilders, 0, maxBuilders);
            if (adjustedMaxBuilders < populationAtImprovement.Count)
            {
                // Remove lowest skilled worker.
                CountyPopulation lowestSkilledPopulation = GetLowestSkilledPopulation(true);
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
        private CountyPopulation GetLowestSkilledPopulation(bool constructing)
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
            List<CountyPopulation> sortedLowestSkillLevelPopulation
                = [.. populationAtImprovement.OrderBy(pop => pop.skills[skill].skillLevel)];
            CountyPopulation lowestSkilledPopulation = sortedLowestSkillLevelPopulation.FirstOrDefault();
            return lowestSkilledPopulation;

        }

        /// <summary>
        /// MaxWorkers * Global Amount of Daily Work without bonus / work cost = average daily amount generated.
        /// </summary>
        /// <param name="productionData"></param>
        public void GenerateAverageDailyAmountGenerated(ProductionData productionData)
        {
            if (countyImprovementType != AllEnums.CountyImprovementType.Storage)
            {
                productionData.AverageDailyAmountGenerated = maxWorkers * Globals.Instance.dailyWorkAmount
                    / (float)productionData.workCost;
            }
            else
            {
                productionData.AverageDailyAmountGenerated = productionData.storageAmount;
            }
        }
        public bool CheckIfStorageImprovement()
        {
            if (countyResourceType == AllEnums.CountyResourceType.StorageNonperishable
                || countyResourceType == AllEnums.CountyResourceType.StoragePerishable)
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
                CountyPopulation lowestSkilledPopulation = GetLowestSkilledPopulation(false);
                lowestSkilledPopulation.RemoveFromCountyImprovement();
            }
        }

        public bool CheckIfCountyImprovementDone()
        {
            if (CurrentAmountOfConstruction == maxAmountOfConstruction)
            {
                return true;
            }
            return false;
        }
        public void AddPopulationToCountyImprovementList(CountyPopulation countyPopulation)
        {
            // GD.Print($"{countyPopulation.firstName} was added to {improvementName}'s list {populationAtImprovement.Count}.");
            populationAtImprovement.Add(countyPopulation);
        }

        public void RemovePopulationFromCountyImprovementList(CountyPopulation countyPopulation)
        {
            populationAtImprovement.Remove(countyPopulation);
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
                    break;
                // Currently this isn't really used, so it just sets the status to producing.
                case AllEnums.CountyImprovementType.Storage:
                    Banker banker = new();
                    banker.AddStorageToCounty(countyData, this);
                    status = AllEnums.CountyImprovementStatus.Producing;
                    break;
                case AllEnums.CountyImprovementType.Standard:
                    status = AllEnums.CountyImprovementStatus.Producing;
                    break;
            }
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
                interest = countyImprovementData.interest,
                factionOutputGoods = countyImprovementData.factionOutputGoods,
                countyOutputGoods = countyImprovementData.countyOutputGoods,
                allDailyWorkAmountAtImprovementCompleted = countyImprovementData.allDailyWorkAmountAtImprovementCompleted,
                workAmountForEachResource = countyImprovementData.workAmountForEachResource,
                factionResourceConstructionCost = countyImprovementData.factionResourceConstructionCost,
                countyResourceConstructionCost = countyImprovementData.countyResourceConstructionCost,
                currentAmountOfCounstruction = countyImprovementData.currentAmountOfCounstruction,
                CurrentAmountOfConstruction = countyImprovementData.CurrentAmountOfConstruction,
                maxAmountOfConstruction = countyImprovementData.maxAmountOfConstruction,
                maxBuilders = countyImprovementData.maxBuilders,
                adjustedMaxBuilders = countyImprovementData.adjustedMaxBuilders,
                maxWorkers = countyImprovementData.maxWorkers,
                adjustedMaxWorkers = countyImprovementData.adjustedMaxWorkers,
                countyResourceType = countyImprovementData.countyResourceType,
                factionResourceType = countyImprovementData.factionResourceType,
                factionInputGoods = countyImprovementData.factionInputGoods,
                countyInputGoods = countyImprovementData.countyInputGoods,
                dailyResourceGenerationAmount = countyImprovementData.dailyResourceGenerationAmount,
                dailyResourceGenerationBonus = countyImprovementData.dailyResourceGenerationBonus,
                status = countyImprovementData.status,
                populationAtImprovement = new List<CountyPopulation>(countyImprovementData.populationAtImprovement),
            };
            return newCountyImprovementData;
        }
    }
}