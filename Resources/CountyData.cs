using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class CountyData : Resource
    {
        public event Action IdleWorkersChanged;

        [Export] public int countyID;
        [Export] public string countyName;
        [Export] public bool isPlayerCapital; // We need to differentiate between player choosen capitals and AI capitals for generation after player creation.
        [Export] public bool isAICapital;
        [Export] public FactionData factionData;
        
        [Export] public AllEnums.Province province;
        [Export] public AllEnums.Terrain biomePrimary;
        [Export] public AllEnums.Terrain biomeSecondary;
        [Export] public AllEnums.Terrain biomeTertiary;
        public List<CountyPopulation> countyPopulation = new();
        public List<CountyPopulation> heroCountyPopulation = new();
        public List<CountyImprovementData> allCountyImprovements = new();
        public List<CountyImprovementData> underConstructionCountyImprovements = new();

        [Export] public int population;
        private int idleWorkers;

        public int IdleWorkers
        {
            get { return idleWorkers; }
            set 
            { 
                idleWorkers = value;
                IdleWorkersChanged?.Invoke();
            }
        }


    }
}