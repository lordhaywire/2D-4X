using Godot;
using Godot.Collections;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class CountyData : Resource
    {
        //public event Action IdleWorkersChanged;

        //public int countyID; // I believe this is being used now.
        //public GameObject gameObject; // This is the game object that is the county.
        [Export] public bool isPlayerCapital;
        [Export] public bool isAICapital;
        //public SpriteRenderer spriteRenderer;
        //public BuildImprovements buildImprovements;
        [Export] public FactionData faction;
        [Export] public AllEnums.Province province;
        [Export] public AllEnums.Terrain biomePrimary;
        [Export] public AllEnums.Terrain biomeSecondary;
        [Export] public AllEnums.Terrain biomeTertiary;
        [Export] public Dictionary<int, CountyPopulation> countyPopulation = new();
        //[Export] public List<CountyPopulation> countyPopulation;
    }
}