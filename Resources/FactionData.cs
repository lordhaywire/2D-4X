using Godot;
using System;
using System.Collections.Generic;
using System.Resources;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class FactionData : Resource
    {
        public event Action InfluenceChanged;
        public event Action MoneyChanged;
        public event Action ScrapChanged;
        public event Action BuildingMaterialsChanged;
        public event Action FoodChanged;

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
            // Do the math for amount used. Subtract yesterdays from todays and that is how much we have used.
            foreach (KeyValuePair<AllEnums.FactionResourceType, FactionResourceData> keyValuePair in factionResources)
            {
                amountUsedFactionResources[keyValuePair.Key].amount = factionResources[keyValuePair.Key].amount -
                    yesterdaysFactionResources[keyValuePair.Key].amount;
                GD.Print("Amount Used: " + amountUsedFactionResources[keyValuePair.Key].amount);
            }

            // This is a "deep" copy.
            yesterdaysFactionResources = factionResources.Duplicate(true);
            TopBarControl.Instance.UpdateFactionExpendables();
        }
    }
}