using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class CountyImprovementData : Resource
    {
        public int location;
        public CountryImprovementDescriptionButton countyImprovementDescriptionButton;
        [Export] public Texture2D improvementTexture;
        [Export] public string improvementName;
        [Export] public string improvementDescription;
        [Export] public int numberBuilt;

        [Export] public AllEnums.Skills workSkill;
        [Export] public InterestData interest;
        // We will eventually be adding resource costs as well.
        [Export] public int influenceCost;
        private int currentAmountOfCounstruction;
        [Export] public int CurrentAmountOfConstruction
        {
            get { return currentAmountOfCounstruction; }
            set
            {
                currentAmountOfCounstruction = Math.Min(value, maxAmountOfConstruction);
            }
        }
        [Export] public int maxAmountOfConstruction;
        [Export] public int maxBuilders;
        [Export] public int maxWorkers;
        [Export] public AllEnums.CountyResourceType countyResourceType;
        [Export] public AllEnums.FactionResourceType factionResourceType;
        // If it needs two or more resources for input
        // We need to know the type and amount of each resource
        [Export] public Godot.Collections.Array<AllEnums.CountyResourceType> inputResources;
        [Export] public int dailyResourceGenerationAmount;
        [Export] public int dailyResourceGenerationBonus;

        [Export] public AllEnums.CountyImprovementStatus status;
        public List<CountyPopulation> countyPopulationAtImprovement = [];

        public bool CheckIfCountyInprovementDone()
        {
            if (CurrentAmountOfConstruction == maxAmountOfConstruction)
            {
                return true;
            }
            return false;
        }
        public void AddPopulationToCountyImprovementList(CountyPopulation countyPopulation)
        {

            GD.Print($"{countyPopulation.firstName} was added to {improvementName}'s list {countyPopulationAtImprovement.Count}.");
            countyPopulationAtImprovement.Add(countyPopulation);
        }

        public void RemovePopulationFromCountyImprovementList(CountyPopulation countyPopulation)
        {
            countyPopulationAtImprovement.Remove(countyPopulation);
        }

        public void SetCountyImprovementComplete()
        {
            status = AllEnums.CountyImprovementStatus.Complete;
        }

        public static CountyImprovementData NewCopy(CountyImprovementData countyImprovementData)
        {
            CountyImprovementData newCountyImprovementData = new()
            {
                location = countyImprovementData.location,
                countyImprovementDescriptionButton = countyImprovementData.countyImprovementDescriptionButton,
                improvementTexture = countyImprovementData.improvementTexture,
                improvementName = countyImprovementData.improvementName,
                improvementDescription = countyImprovementData.improvementDescription,
                numberBuilt = countyImprovementData.numberBuilt,
                workSkill = countyImprovementData.workSkill,
                interest = countyImprovementData.interest,
                influenceCost = countyImprovementData.influenceCost,
                currentAmountOfCounstruction = countyImprovementData.currentAmountOfCounstruction,
                CurrentAmountOfConstruction = countyImprovementData.CurrentAmountOfConstruction,
                maxAmountOfConstruction = countyImprovementData.maxAmountOfConstruction,
                maxBuilders = countyImprovementData.maxBuilders,
                maxWorkers = countyImprovementData.maxWorkers,
                countyResourceType = countyImprovementData.countyResourceType,
                factionResourceType = countyImprovementData.factionResourceType,
                dailyResourceGenerationAmount = countyImprovementData.dailyResourceGenerationAmount,
                dailyResourceGenerationBonus = countyImprovementData.dailyResourceGenerationBonus,
                status = countyImprovementData.status,
                countyPopulationAtImprovement = new List<CountyPopulation>(countyImprovementData.countyPopulationAtImprovement),
            };
            return newCountyImprovementData;
        }
    }
}