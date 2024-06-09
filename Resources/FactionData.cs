using Godot;
using System;
using System.Collections.Generic;

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

        private int influence;
        private int money;

        private int scrapFaction;
        private int buildingMaterialsFaction;
        private int foodFaction;

        [ExportGroup("Getter Setter")]
        [Export]
        public int Influence
        {
            get { return influence; }
            set
            {
                influence = value;
                if (isPlayer == true)
                {
                    InfluenceChanged?.Invoke();
                }
            }
        }
        [Export]
        public int Money
        {
            get { return money; }
            set
            {
                money = value;
                if (isPlayer == true)
                {
                    MoneyChanged?.Invoke();
                }
            }
        }

        [Export]
        public int RemnantsFaction
        {
            get { return scrapFaction; }
            set
            {
                scrapFaction = value;
                if(isPlayer == true)
                {
                    ScrapChanged?.Invoke();
                }
            }
        }

        [Export]
        public int BuildingMaterialsFaction
        {
            get { return buildingMaterialsFaction; }
            set
            {
                buildingMaterialsFaction = value;
                if (isPlayer == true)
                {
                    BuildingMaterialsChanged?.Invoke();
                }
            }
        }


        [Export]
        public int FoodFaction
        {
            get { return foodFaction; }
            set
            {
                foodFaction = value;
                if (isPlayer == true)
                {
                    FoodChanged?.Invoke();
                }
            }
        }

        [ExportGroup("Diplomatic Incidences")]
        public List<War> wars = [];

        [ExportGroup("Diplomatic Matrix")]
        [Export] public Godot.Collections.Dictionary<string, bool> factionWarDictionary = [];
    }
}