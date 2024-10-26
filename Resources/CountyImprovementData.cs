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

        [ExportGroup("Improvement Info")]
        [Export] public AllEnums.CountyImprovementType countyImprovementType;
        //[Export] public int location;
        [Export] public bool prioritize;
        // This can't be exported so we need to change how it works. It doesn't even look like I am using this.
        //public CountryImprovementPanelContainer countyImprovementPanelContainer;
        [Export] public Texture2D improvementTexture;
        [Export] public string improvementName;
        [Export] public string improvementDescription;
        [Export] public int numberBuilt; // This is used to number the improvement name in the County Improvement Panel.

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
        /// <summary>
        /// If the improvement doesn't produce a good, but instead produces something like research, or
        /// warehouse space.  This string says what that is.  If it null then the normal production
        /// GridContainer is shown.
        /// </summary>
        //[Export] public string nonTangibleGoodProduced;
        //[Export] public string nonTangibleGoodNotBeingProduced;
        [ExportGroup("Outputs")]
        [Export] public Godot.Collections.Dictionary<FactionResourceData, int> factionOutputGoods = [];
        [Export] public Godot.Collections.Dictionary<CountyResourceData, int> countyOutputGoods = [];
        [Export] public Godot.Collections.Array<CountyResourceData> testCountyResourceList;
        [Export] public int dailyWorkAmountCompleted;




        [Export] public int dailyResourceGenerationAmount; // I am pretty sure these are done.
        [Export] public int dailyResourceGenerationBonus; // I am pretty sure these are done.
        [Export] public int workAmount;
        [Export] public int workAmountForEachResource;
        [Export] public int numberOfGoodsGenerated;

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

        public bool CheckIfStorageImprovement()
        {
            if(countyResourceType == AllEnums.CountyResourceType.StorageNonperishable
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
            GD.Print($"{countyPopulation.firstName} was added to {improvementName}'s list {populationAtImprovement.Count}.");
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
                //location = countyImprovementData.location,
                prioritize = countyImprovementData.prioritize,
                //countyImprovementPanelContainer = countyImprovementData.countyImprovementPanelContainer,
                improvementTexture = countyImprovementData.improvementTexture,
                improvementName = countyImprovementData.improvementName,
                improvementDescription = countyImprovementData.improvementDescription,
                numberBuilt = countyImprovementData.numberBuilt,
                workSkill = countyImprovementData.workSkill,
                interest = countyImprovementData.interest,
                factionOutputGoods = countyImprovementData.factionOutputGoods,
                countyOutputGoods = countyImprovementData.countyOutputGoods,
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
                //nonTangibleGoodProduced = countyImprovementData.nonTangibleGoodProduced,
                //nonTangibleGoodNotBeingProduced = countyImprovementData.nonTangibleGoodNotBeingProduced,
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