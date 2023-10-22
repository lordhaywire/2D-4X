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
        //public GameObject gameObject; // This is the game object that is the county.
        [Export] public string countyName;
        [Export] public bool isPlayerCapital;
        [Export] public bool isAICapital;
        //public SpriteRenderer spriteRenderer;
        //public BuildImprovements buildImprovements;
        [Export] public FactionData faction;
        
        [Export] public AllEnums.Province province;
        [Export] public AllEnums.Terrain biomePrimary;
        [Export] public AllEnums.Terrain biomeSecondary;
        [Export] public AllEnums.Terrain biomeTertiary;
        [Export] public Godot.Collections.Dictionary<int, CountyPopulation> countyPopulation = new();
        public List<CountyPopulation> heroCountyPopulation = new();

        [ExportGroup("Getter Setters")]
        private int population;
        public int Population
        {
            get { return population; }
            set
            {
                population = value;
                CountyInfoControl.Instance.UpdateCountyPopulationLabel(population);
            }
        }
        private int idleWorkers;
        [Export]
        public int IdleWorkers
        {
            get { return idleWorkers; }
            set
            {
                idleWorkers = value;
                CountyInfoControl.Instance.UpdateIdleWorkersLabel(idleWorkers);
            }
        }
    }
}