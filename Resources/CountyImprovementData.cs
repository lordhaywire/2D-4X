using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class CountyImprovementData : Resource
    {
        public int location;
        public bool prioritize;
        public CountryImprovementPanelContainer countyImprovementDescriptionButton;
        [Export] public Texture2D improvementTexture;
        [Export] public string improvementName;
        [Export] public string improvementDescription;
        [Export] public int numberBuilt;

        [Export] public AllEnums.Skills workSkill;
        [Export] public InterestData interest;


        [Export] public Godot.Collections.Dictionary<FactionResourceData, int> factionResourceConstructionCost;
        [Export] public Godot.Collections.Dictionary<CountyResourceData, int> countyResourceConstructionCost;

        private int currentAmountOfCounstruction;
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
        public int adjustedMaxBuilders;
        [Export] public int maxWorkers;
        public int adjustedMaxWorkers;
        [Export] public AllEnums.CountyResourceType countyResourceType;
        [Export] public AllEnums.FactionResourceType factionResourceType;
        /// <summary>
        /// If the improvement doesn't produce a good, but instead produces something like research, or
        /// warehouse space.  This string says what that is.  If it null then the normal production
        /// GridContainer is shown.
        /// </summary>
        [Export] public string nonTangibleGoodProduced;
        [Export] public string nonTangibleGoodNotBeingProduced;
        [Export] public Godot.Collections.Dictionary<FactionResourceData, int> factionOutputGoods = [];
        [Export] public Godot.Collections.Dictionary<CountyResourceData, int> countyOutputGoods = [];

        // All input goods that are need to create the finished good.
        // For some reason this one needs to be initialized, but the faction and county construction costs don't.
        [Export] public Godot.Collections.Dictionary<CountyResourceData, int> inputGoods = [];
        [Export] public int dailyResourceGenerationAmount;
        [Export] public int dailyResourceGenerationBonus;

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

        public void SetCountyImprovementComplete()
        {
            if (maxWorkers > 0)
            {
                status = AllEnums.CountyImprovementStatus.Producing;
            }
            else
            {
                status = AllEnums.CountyImprovementStatus.ProducingWithoutWorkers;
            }
        }

        public static CountyImprovementData NewCopy(CountyImprovementData countyImprovementData)
        {
            CountyImprovementData newCountyImprovementData = new()
            {
                location = countyImprovementData.location,
                prioritize = countyImprovementData.prioritize,
                countyImprovementDescriptionButton = countyImprovementData.countyImprovementDescriptionButton,
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
                nonTangibleGoodProduced = countyImprovementData.nonTangibleGoodProduced,
                nonTangibleGoodNotBeingProduced = countyImprovementData.nonTangibleGoodNotBeingProduced,
                inputGoods = countyImprovementData.inputGoods,
                dailyResourceGenerationAmount = countyImprovementData.dailyResourceGenerationAmount,
                dailyResourceGenerationBonus = countyImprovementData.dailyResourceGenerationBonus,
                status = countyImprovementData.status,
                populationAtImprovement = new List<CountyPopulation>(countyImprovementData.populationAtImprovement),
            };
            return newCountyImprovementData;
        }
    }
}