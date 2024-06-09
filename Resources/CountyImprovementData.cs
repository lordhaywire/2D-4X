using Godot;
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
        [Export] public int currentAmountOfConstruction;
        [Export] public int maxAmountOfConstruction;
        [Export] public int currentBuilders;
        [Export] public int maxBuilders;
        [Export] public int currentWorkers;
        [Export] public int maxWorkers;
        [Export] public ResourceData resourceData;
        [Export] public int dailyResourceGenerationAmount;

        [Export] public AllEnums.CountyImprovementStatus status;
        public List<CountyPopulation> countyPopulationAtImprovement;
    }
}