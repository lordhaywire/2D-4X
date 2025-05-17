using Godot;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class StoryEventData : Resource
    {
        [Export] public string storyEventTitle;
        [Export] public string storyEventDescription;
        [Export] public AllEnums.Terrain storyEventTerrainType;

        [ExportGroup("Event Rewards")]
        [Export] public AllEnums.CountyGoodType rewardCountyGoodType;
        [Export] public AllEnums.FactionGoodType rewardFactionGoodType;
        [Export] public int rewardAmount;

        [Export] public string[] choices;

        public County eventCounty;


    }
}