using Godot;
using System;

namespace PlayerSpace
{
    public partial class AllInterests : Node
    {
        public static AllInterests Instance { get; private set; }

        [Export] public Godot.Collections.Array<InterestData> interests;

        public override void _Ready()
        {
            Instance = this;
        }

        public InterestData GetRandomInterest()
        {
            Random random = new();
            if (interests.Count == 0)
            {
                return null; // Or handle this case as appropriate
            }

            int randomIndex = random.Next(0, interests.Count);
            return interests[randomIndex];
        }
    }
}