using Godot;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class CountyImprovementData : Resource
    {
        [Export] public string improvementName;
        [Export] public string improvementDescription;

        // We will eventually be adding resource costs as well.
        [Export] public int workCompleted;
        [Export] public int influenceCost;
        [Export] public int currentAmountOfWork;
        [Export] public int maxAmountOfWork;
        [Export] public int currentWorkers;
        [Export] public int maxWorkers;
        [Export] public ResourceData resourceData;
        [Export] public int dailyResourceGenerationAmount;

        [Export] public bool isBeingBuilt;
        [Export] public bool isBuilt;
    }
}