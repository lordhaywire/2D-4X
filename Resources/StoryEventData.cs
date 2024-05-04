using Godot;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class StoryEventData : Resource
    {
        [Export] public string storyEventTitle;
        [Export] public string storyEventDescription;

        [Export] public ResourceData resource;
        [Export] public int resourceAmount;

        [Export] public string[] choices;

        public County eventCounty;
    }
}