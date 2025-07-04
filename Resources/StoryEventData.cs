using Godot;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class StoryEventData : Resource
    {
        [Export] public string storyEventTitle;
        [Export] public string storyEventDescription;
        [Export] public AllEnums.Terrain storyEventTerrainType;
        [Export] public int amountExplored;

        [ExportGroup("Event Rewards")]
        [Export] public AllEnums.StoryEventRewardType storyEventRewardType;
        [Export] public GoodData rewardGood;
        [Export] public int rewardAmount;
        [Export] public CountyImprovementData rewardCountyImprovement;

        [Export] public string[] choices;

        public County eventCounty;
    }
}