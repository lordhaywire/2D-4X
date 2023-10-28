using Godot;
using Godot.Collections;
using System.Collections.Generic;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class CountyData : Resource
    {
        //public event Action IdleWorkersChanged;

        //public int countyID; // I believe this is being used now.
        [Export] public string countyName;
        [Export] public bool isPlayerCapital;
        [Export] public bool isAICapital;
        //public BuildImprovements buildImprovements;
        [Export] public FactionData faction;
        
        [Export] public AllEnums.Province province;
        [Export] public AllEnums.Terrain biomePrimary;
        [Export] public AllEnums.Terrain biomeSecondary;
        [Export] public AllEnums.Terrain biomeTertiary;
        public List<CountyPopulation> countyPopulation = new();
        public List<CountyPopulation> heroCountyPopulation = new();

        [Export] public int population;
        [Export] public int idleWorkers;
    }
}