using Godot;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class FactionData : Resource
	{
        //public event Action InfluenceChanged;

        [Export] public bool isPlayer;
        [Export] public string factionName;
        [Export] public Color factionColor;
        //public FactionNameAndColor factionNameAndColor;
        //public List<ResearchItem> researchItems;
        //public CountyPopulation factionLeader;

        [ExportGroup("Resources")]
        [Export] public int money;
        [Export] public int food;
        [Export] public int scrap;

        private int influence;
        [Export]
        public int Influence
        {
            get { return influence; }
            set
            {
                influence = value;
                //InfluenceChanged?.Invoke();
            }
        }
    }
}