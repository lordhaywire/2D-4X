using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class FactionData : Resource
	{
        public event Action InfluenceChanged;

        [ExportGroup("Faction Info")]
        [Export] public bool isPlayer;
        [Export] public string factionName;
        [Export] public Color factionColor;
        [Export] public int factionCapitalCounty;
        public List<ResearchItemData> researchItems = new();
        public List<CountyData> countiesFactionOwns = new();
        public CountyPopulation factionLeader;

        public Diplomacy diplomacy = new();
        public TokenSpawner tokenSpawner = new(); 

        [ExportGroup("Expendables")]
        [Export] public int money;
        [Export] public int food;
        [Export] public int scrap;

        [ExportGroup("Getter Setter")]
        private int influence;
        [Export]
        public int Influence
        {
            get { return influence; }
            set
            {
                influence = value;
                InfluenceChanged?.Invoke();
            }
        }

        [ExportGroup("Diplomatic Incidences")]
        public List<War> wars = new();
    }
}