using Godot;
using System;

namespace PlayerSpace
{
    public partial class AllResearch : Node
    {
        public static AllResearch Instance { get; private set; }

        [Export] public ResearchItemData[] allResearchItemDatas;

        public override void _Ready()
        {
            Instance = this;
        }
    }
}