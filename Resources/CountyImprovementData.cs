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
        //[Export] public float workAmountForEachResource;
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
        [Export] public Godot.Collections.Dictionary<CountyResourceData, ProductionData> countyOutputGoods = [];
        [Export] public int allDailyWorkAmountAtImprovementCompleted;

        //[Export] public int dailyResourceGenerationAmount; // I am pretty sure these are not used.
        //[Export] public int dailyResourceGenerationBonus; // I am pretty sure these are not used.

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

        public int CountNumberOfGoodsGettingProduced()
        {
            int numberOfGoodsGettingProduced = countyOutputGoods.Count + factionOutputGoods.Count;
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
                factionOutputGoods = countyImprovementData.CopyFactionOutputGoods(),
                countyOutputGoods = countyImprovementData.CopyCountyOutputGoods(),
                allDailyWorkAmountAtImprovementCompleted = countyImprovementData.allDailyWorkAmountAtImprovementCompleted,
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
                status = countyImprovementData.status,
                populationAtImprovement = new List<CountyPopulation>(countyImprovementData.populationAtImprovement),
            };
            return newCountyImprovementData;
        }

        // We have to do a copy of a copy to make a copy that is unique.
        public Godot.Collections.Dictionary<FactionResourceData, ProductionData> CopyFactionOutputGoods()
        {
            Godot.Collections.Dictionary<FactionResourceData, ProductionData> copiedDictionary = [];

            foreach (System.Collections.Generic.KeyValuePair<FactionResourceData, ProductionData> item in factionOutputGoods)
            {
                copiedDictionary[item.Key] = item.Value.NewCopy(item.Value);
            }

            return copiedDictionary;
        }

        // We have to do a copy of a copy to make a copy that is unique.
        public Godot.Collections.Dictionary<CountyResourceData, ProductionData> CopyCountyOutputGoods()
        {
            Godot.Collections.Dictionary<CountyResourceData, ProductionData> copiedDictionary = [];

            foreach (System.Collections.Generic.KeyValuePair<CountyResourceData, ProductionData> item in countyOutputGoods)
            {
                copiedDictionary[item.Key] = item.Value.NewCopy(item.Value);
            }

            return copiedDictionary;
        }
    }
}