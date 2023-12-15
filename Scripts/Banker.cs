using Godot;
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

        public void ChargeForHero()
        {
            GD.Print("Player Influence: " + Globals.Instance.playerFactionData.Influence);
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
            return faction.food >= Globals.Instance.minimumFood;
        }

        public void BuildImprovement(CountyData countyData, CountyImprovementData countyImprovementData)
        {
            GD.Print($"{countyData.factionData.factionName} is building {countyImprovementData.improvementName}.");
            countyImprovementData.isBeingBuilt = true;
            countyData.underConstructionCountyImprovements.Add(countyImprovementData);
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
                    if (improvementData.isBeingBuilt || improvementData.isBuilt)
                    {
                        GD.Print($"{improvementData.improvementName} is already being built.");
                        return;
                    }
                    if (improvementData.resourceData.resourceType == AllEnums.ResourceType.Food)
                    {
                        GD.Print($"{factionData.factionName} found {improvementData.improvementName} in {countyDataItem.countyName}.");
                        countyImprovementData = improvementData;
                        countyData = countyDataItem;
                        return;
                    }
                }
            }
        }

        /*
        public CountyImprovementData FindFoodBuilding(FactionData factionData)
        {
            List<CountyData> countiesData = factionData.countiesFactionOwns;

            foreach (CountyData countyData in countiesData)
            {
                foreach (CountyImprovementData countyImprovementData in countyData.allCountyImprovements)
                {
                    if (countyImprovementData.isBeingBuilt == true || countyImprovementData.isBuilt == true)
                    {
                        GD.Print($"{countyImprovementData.improvementName} is already being built.");
                        return null;
                    }
                    if (countyImprovementData.resourceData.resourceType == AllEnums.ResourceType.Food)
                    {
                        GD.Print($"{factionData.factionName} found {countyImprovementData.improvementName} in {countyData.countyName}.");
                        return countyImprovementData;
                    }
                }
            }
            GD.Print("No food building found.");
            return null;
        }
        */


        /*
        public void CountIdleWorkers(County county)
        {
            // I don't think this is very efficient.
            int idleWorkers = 0;

            for (int i = 0; i < county.countyPopulation.Count; i++)
            {
                if (county.countyPopulation[i].nextBuilding == null)
                {
                    idleWorkers++;
                }
            }

            county.IdleWorkers = idleWorkers;
        }
        */

    }

}
