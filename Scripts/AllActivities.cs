using Godot;

namespace PlayerSpace
{
    public partial class AllActivities : Node
    {
        public static AllActivities Instance { get; private set; }

        [Export] public ActivityData[] allActivityData;

        public override void _Ready()
        {
            Instance = this;
        }
    }
}