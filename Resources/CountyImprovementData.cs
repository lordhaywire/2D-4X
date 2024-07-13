using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class CountyImprovementData : Resource
    {
        public CountryImprovementDescriptionButton countyImprovementDescriptionButton;
        [Export] public Texture2D improvementTexture;
        [Export] public string improvementName;
        [Export] public string improvementDescription;
        [Export] public int numberBuilt;

        [Export] public AllEnums.Skills workSkill;
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
        [Export] public int currentBuilders;
        [Export] public int maxBuilders;
        [Export] public int currentWorkers;
        [Export] public int maxWorkers;
        [Export] public CountyResourceData resourceData;
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

        public void RemovePopulationFromCountyImprovementList(CountyPopulation countyPopulation)
        {
            countyPopulationAtImprovement.Remove(countyPopulation);
        }

        public void SetCountyImprovementComplete()
        {
            status = AllEnums.CountyImprovementStatus.Complete;
        }
    }
}