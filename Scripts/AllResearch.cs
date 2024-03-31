using Godot;

namespace PlayerSpace
{
    public partial class AllResearch : Node
    {
        public static AllResearch Instance { get; private set; }

        [Export] public ResearchItemData[] allTierOneResearchData;

        public override void _Ready()
        {
            Instance = this;
        }
    }
}