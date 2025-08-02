using Godot;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class ProductionData : Resource
    {
        [Export] public int workAmountAfterSkillCheck; // This should be 
        [Export] public int workCost; // The amount of work it takes to generate 1 of this good, that has
        // to be set in the inspector by someone.
        [Export] public int workAmount;
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

        public ProductionDto ToDto()
        {
            return new ProductionDto
            {
                WorkAmountAfterSkillCheck = workAmountAfterSkillCheck,
                WorkCost = workCost,
                WorkAmount = workAmount,
                TodaysGoodsAmountGenerated = todaysGoodsAmountGenerated,
                AverageDailyGoodsAmountGenerated = AverageDailyGoodsAmountGenerated,
                StorageAmount = storageAmount
            };
        }

        public static ProductionData FromDto(ProductionDto dto)
        {
            return new ProductionData
            {
                workAmountAfterSkillCheck = dto.WorkAmountAfterSkillCheck,
                workCost = dto.WorkCost,
                workAmount = dto.WorkAmount,
                todaysGoodsAmountGenerated = dto.TodaysGoodsAmountGenerated,
                AverageDailyGoodsAmountGenerated = dto.AverageDailyGoodsAmountGenerated,
                storageAmount = dto.StorageAmount
            };
        }

        public ProductionData NewCopy(ProductionData productionData)
        {
            ProductionData newProductionData = new()
            {
                workAmountAfterSkillCheck = productionData.workAmountAfterSkillCheck,
                workCost = productionData.workCost,
                workAmount = productionData.workAmount,
                todaysGoodsAmountGenerated = productionData.todaysGoodsAmountGenerated,
                averageDailyAmountGenerated = productionData.averageDailyAmountGenerated,
                storageAmount = productionData.storageAmount,
            };
            return newProductionData;
        }
    }
}