using Godot;
using System;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class FactionData : Resource
	{
        public event Action InfluenceChanged;

        [Export] public bool isPlayer;
        [Export] public string factionName;
        [Export] public Color factionColor;
        [Export] public int factionCapitalCounty;
        //public List<ResearchItem> researchItems;
        public CountyPopulation factionLeader;

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
    }
}