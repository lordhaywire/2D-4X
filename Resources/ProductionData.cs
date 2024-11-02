using Godot;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class ProductionData : Resource
    {
        [Export] public int workAmountAfterSkillCheck; // This should be 
        [Export] public float workCost; // The amount of work it takes to generate 1 of this good, that has
        // to be set in the inspector by someone.
        [Export] public int workAmountLeftOver;
        [Export] public int todaysAmountGenerated;
        [Export] public float averageDailyAmountGenerated;
        [Export] public int storageAmount;
    }
}