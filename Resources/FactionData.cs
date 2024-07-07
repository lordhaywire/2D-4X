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