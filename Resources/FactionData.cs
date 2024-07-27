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