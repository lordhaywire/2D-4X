using Godot;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class ProductionData : Resource
    {
        [Export] public int workAmountAfterSkillCheck; // This should be 
        [Export] public float workCost; // The amount of work it takes to generate 1 of this good, that has
        // to be set in the inspector by someone.
        [Export] public float workAmountForEachResourceForToday = 0;
        [Export] public float workAmountLeftOver = 0;
        [Export] public int todaysGoodsAmountGenerated;
        [Export] private float averageDailyAmountGenerated;
        [Export] public int storageAmount;

        [Export]
        public float AverageDailyGoodsAmountGenerated
        {
            get { return averageDailyAmountGenerated; }
            set
            {
                averageDailyAmountGenerated = value;
                if (averageDailyAmountGenerated >= 1)
                {
                    averageDailyAmountGenerated = Mathf.Ceil(averageDailyAmountGenerated);
                }
            }
        }

    }
}