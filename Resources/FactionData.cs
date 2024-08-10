using Godot;
using System;
using System.Collections.Generic;
using System.Resources;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class FactionData : Resource
    {
        [ExportGroup("Faction Info")]
        [Export] public int factionID;
        [Export] public bool isPlayer;
        [Export] public string factionName;
        [Export] public Color factionColor;
        [Export] public int factionCapitalCounty;

        public List<ResearchItemData> researchItems = [];
        readonly List<ResearchItemData> researchableResearch = [];

        public List<CountyData> countiesFactionOwns = [];
        public List<CountyPopulation> allHeroesList = [];
        public CountyPopulation factionLeader;

        public Diplomacy diplomacy = new();
        public TokenSpawner tokenSpawner = new();

        // Resources.
        public Godot.Collections.Dictionary<AllEnums.FactionResourceType, FactionResourceData> factionResources = [];
        public Godot.Collections.Dictionary<AllEnums.FactionResourceType, FactionResourceData> yesterdaysFactionResources = [];
        public Godot.Collections.Dictionary<AllEnums.FactionResourceType, FactionResourceData> amountUsedFactionResources = [];
        /// <summary>
        /// I am not sure we need this.
        /// </summary>
        [Obsolete("What the flying fuck is this?")] public Godot.Collections.Dictionary<AllEnums.FactionResourceType, FactionResourceData> actualUsedFactionResources = [];

        [ExportGroup("Diplomatic Incidences")]
        public List<War> wars = [];

        [ExportGroup("Diplomatic Matrix")]
        [Export] public Godot.Collections.Dictionary<string, bool> factionWarDictionary = [];

        // Goes through all the population and adds a set number to research.
        // It should check what they are doing and try to add that research then if they aren't doing anything
        // it should add to a random research that isn't done yet.
        // Don't forget about idle heroes researching other things.
        public void PopulationResearch(CountyData countyData)
        {
            // Get a list of all the research that isn't done.
            CreateResearchableResearchList();
            
            // Have the population research by their interests.
            CheckEachPeopleForResearch(countyData);
        }

        private void CreateResearchableResearchList()
        {
            researchableResearch.Clear();
            foreach (ResearchItemData researchItemData in researchItems)
            {
                if (researchItemData.isResearchDone == false)
                {
                    researchableResearch.Add(researchItemData);
                }
            }
        }

        private void CheckEachPeopleForResearch(CountyData countyData)
        {
            foreach (CountyPopulation countyPopulation in countyData.countyPopulationList)
            {
                ResearchByInterest(countyPopulation);
                ResearchByJob(countyPopulation);
            }
        }

        private void ResearchByJob(CountyPopulation countyPopulation)
        {
            ResearchItemData whatPopulationIsResearching = null;

            foreach (ResearchItemData researchItemData in researchableResearch)
            {
                // If the county improvement isn't null then see if the interest matches.
                if (countyPopulation.currentCountyImprovement?.interest == researchItemData.interest)
                {
                    whatPopulationIsResearching = researchItemData;
                    //GD.Print($"{countyPopulation.firstName} {countyPopulation.interest} is having them research {researchItemData.researchName}");
                    break;
                }
            }

            if (whatPopulationIsResearching != null)
            {
                // After a skill check, have the banker add the research to the research with a possible bonus.
                Banker.IncreaseResearchAmountBonus(countyPopulation, whatPopulationIsResearching);
            }
        }

        private void ResearchByInterest(CountyPopulation countyPopulation)
        {
            ResearchItemData whatPopulationIsResearching = null;

            foreach (ResearchItemData researchItemData in researchableResearch)
            {
                if (countyPopulation.interest.interestType == researchItemData.interest.interestType)
                {
                    whatPopulationIsResearching = researchItemData;
                    GD.Print($"{countyPopulation.firstName} {countyPopulation.interest} is having them research {researchItemData.researchName}");
                    break;
                }
            }

            if (whatPopulationIsResearching != null)
            {
                // After a skill check, have the banker add the research to the research with a possible bonus.
                Banker.IncreaseResearchAmountBonus(countyPopulation, whatPopulationIsResearching);
            }
        }

        public void CopyFactionResourcesToYesterday()
        {
            // Creating a deep copy of the dictionary
            yesterdaysFactionResources = [];
            foreach (KeyValuePair<AllEnums.FactionResourceType, FactionResourceData> keyValuePair in factionResources)
            {
                yesterdaysFactionResources.Add(keyValuePair.Key, new FactionResourceData
                {
                    name = keyValuePair.Value.name,
                    description = keyValuePair.Value.description,
                    resourceType = keyValuePair.Value.resourceType,
                    amount = keyValuePair.Value.amount,
                });
            }
            if (isPlayer)
            {
                GD.Print("Yesterday's Influence: " + yesterdaysFactionResources[AllEnums.FactionResourceType.Influence].amount);
                GD.Print("This Influence should be the same as yesterdays: " + factionResources[AllEnums.FactionResourceType.Influence].amount);
            }
        }

        // Zero resources that are summed from each county.
        public void ZeroFactionCountyResources()
        {
            factionResources[AllEnums.FactionResourceType.Food].amount = 0;
            factionResources[AllEnums.FactionResourceType.Remnants].amount = 0;
            factionResources[AllEnums.FactionResourceType.BuildingMaterial].amount = 0;
        }

        // This should be counting just the county resources of Faction Type, not the used.
        public void CountAllCountyFactionResources()
        {
            ZeroFactionCountyResources();
            foreach (CountyData countyData in countiesFactionOwns)
            {
                factionResources[AllEnums.FactionResourceType.Food].amount 
                    += countyData.CountFactionResourceOfType(AllEnums.FactionResourceType.Food);
                factionResources[AllEnums.FactionResourceType.BuildingMaterial].amount 
                    += countyData.CountFactionResourceOfType(AllEnums.FactionResourceType.BuildingMaterial);
                factionResources[AllEnums.FactionResourceType.Remnants].amount 
                    += countyData.CountFactionResourceOfType(AllEnums.FactionResourceType.Remnants);
            }
        }

        public void CountAllCountyFactionUsedResources()
        { 
            ZeroFactionCountyActualUsedResources();
            foreach (CountyData countyData in countiesFactionOwns)
            {
                amountUsedFactionResources[AllEnums.FactionResourceType.Food].amount
                    += countyData.CountUsedFactionResourceOfType(AllEnums.FactionResourceType.Food);
                amountUsedFactionResources[AllEnums.FactionResourceType.BuildingMaterial].amount
                    += countyData.CountUsedFactionResourceOfType(AllEnums.FactionResourceType.BuildingMaterial);
                amountUsedFactionResources[AllEnums.FactionResourceType.Remnants].amount
                    += countyData.CountUsedFactionResourceOfType(AllEnums.FactionResourceType.Remnants);
            }
        }

        private void ZeroFactionCountyActualUsedResources()
        {
            amountUsedFactionResources[AllEnums.FactionResourceType.Food].amount = 0;
            amountUsedFactionResources[AllEnums.FactionResourceType.Remnants].amount = 0;
            amountUsedFactionResources[AllEnums.FactionResourceType.BuildingMaterial].amount = 0;
        }

        public void SubtractFactionResources()
        {
            // Do the math for amount used. Subtract yesterdays from todays and that is how much we have used.
            foreach (KeyValuePair<AllEnums.FactionResourceType, FactionResourceData> keyValuePair in factionResources)
            {
                amountUsedFactionResources[keyValuePair.Key].amount = factionResources[keyValuePair.Key].amount -
                    yesterdaysFactionResources[keyValuePair.Key].amount;
            }
            if (isPlayer)
            {
                GD.Print("After subtraction yesterdays influence is: " + yesterdaysFactionResources[AllEnums.FactionResourceType.Influence].amount);
            }
        }
    }
}