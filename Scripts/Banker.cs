using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class Banker : Node
    {
        public static Banker Instance { get; private set; }

        public override void _Ready()
        {
            Instance = this;
        }

        public void AddCountyResource(StoryEventData storyEventData)
        {
            GD.Print($"Faction: {storyEventData.eventCounty.countyData.factionData.factionName} is adding " +
                $"{storyEventData.resourceAmount} {storyEventData.resource.resourceName}");
            if (storyEventData.resource.perishable)
            {
                storyEventData.eventCounty.countyData.perishableResources[storyEventData.resource.countyResourceType].amount
                    += storyEventData.resourceAmount;
            }
            TopBarControl.Instance.UpdateTopBarWithCountyResources();
        }

        private void AddResourceToCounty()
        {
            throw new NotImplementedException();
        }

        public void ChargeForHero()
        {
            //GD.Print("Player Influence: " + Globals.Instance.playerFactionData.Influence);
            Globals.Instance.playerFactionData.Influence -= Globals.Instance.costOfHero;
        }

        public bool CheckBuildingCost(FactionData factionData, CountyImprovementData countyImprovementData)
        {
            return factionData.Influence >= countyImprovementData.influenceCost;
        }

        // Charge for building and also assign it to the underConstructionList.
        public void ChargeForBuilding(FactionData factionData, CountyImprovementData countyImprovementData)
        {
            factionData.Influence -= countyImprovementData.influenceCost;
        }

        public bool CheckEnoughFood(FactionData faction)
        {
            return faction.FoodFaction >= Globals.Instance.minimumFood;
        }

        public void BuildImprovement(CountyData countyData, CountyImprovementData countyImprovementData)
        {
            countyImprovementData.underConstruction = true;
            countyData.underConstructionCountyImprovements.Add(countyImprovementData);
            GD.Print($"{countyData.factionData.factionName} is building {countyImprovementData.improvementName}.");
        }

        public void FindFoodBuilding(FactionData factionData, out CountyImprovementData countyImprovementData, out CountyData countyData)
        {
            countyImprovementData = null;
            countyData = null;

            List<CountyData> countiesData = factionData.countiesFactionOwns;

            foreach (CountyData countyDataItem in countiesData)
            {
                foreach (CountyImprovementData improvementData in countyDataItem.allCountyImprovements)
                {
                    if (improvementData.underConstruction || improvementData.isBuilt)
                    {
                        //GD.Print($"{improvementData.improvementName} is already being built.");
                        return;
                    }
                    if (improvementData.resourceData.factionResourceType == AllEnums.FactionResourceType.Food)
                    {
                        //GD.Print($"{factionData.factionName} found {improvementData.improvementName} in {countyDataItem.countyName}.");
                        countyImprovementData = improvementData;
                        countyData = countyDataItem;
                        return;
                    }
                }
            }
        }
    }
}
