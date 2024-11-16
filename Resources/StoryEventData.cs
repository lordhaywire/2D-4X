using Godot;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class StoryEventData : Resource
    {
        [Export] public string storyEventTitle;
        [Export] public string storyEventDescription;

        [ExportGroup("Event Rewards")]
        [Export] public GoodData good;
        [Export] public AllEnums.CountyGoodType resourceType;
        [Export] public int resourceAmount;

        [Export] public string[] choices;

        public County eventCounty;
    }
}