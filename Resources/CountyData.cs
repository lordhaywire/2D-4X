using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class CountyData : Resource
    {
        //public event Action IdleWorkersChanged;

        [Export] public int countyID;
        [Export] public string countyName;
        [Export] public bool isPlayerCapital; // We need to differentiate between player choosen capitals and AI capitals for generation after player creation.
        [Export] public bool isAICapital;
        //public BuildImprovements buildImprovements;
        [Export] public FactionData faction;
        
        [Export] public AllEnums.Province province;
        [Export] public AllEnums.Terrain biomePrimary;
        [Export] public AllEnums.Terrain biomeSecondary;
        [Export] public AllEnums.Terrain biomeTertiary;
        public List<CountyPopulation> countyPopulation = new();
        public List<CountyPopulation> heroCountyPopulation = new();
        public List<CountyImprovementData> countyImprovements = new();

        [Export] public int population;
        [Export] public int idleWorkers;
    }
}