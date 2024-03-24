using Godot;

namespace PlayerSpace
{
    public partial class StoryEventList : Node
    {
        public static StoryEventList Instance { get; private set; }

        [Export] public StoryEventData[] storyEventDatas;

        public override void _Ready()
        {
            Instance = this;
        }
    }
}