using System.Collections.Generic;

namespace PlayerSpace;
public class CountyImprovementDto
{
    public string ImprovementName { get; set; }
    public string ImprovementDescription { get; set; }
    public string ImprovementTexturePath { get; set; }

    public AllEnums.CountyImprovementType CountyImprovementType { get; set; }
    public bool Prioritize { get; set; }

    public AllEnums.Skills WorkSkill { get; set; }
    public string InterestDataName { get; set; }   // store name/id instead of full resource

    // Construction
    public int CurrentAmountOfConstruction { get; set; }
    public int MaxAmountOfConstruction { get; set; }
    public int NumberBuilt { get; set; }
    public int MaxBuilders { get; set; }
    public int AdjustedMaxBuilders { get; set; }
    public int MaxWorkers { get; set; }
    public int AdjustedMaxWorkers { get; set; }

    // Goods
    public Dictionary<string, int> GoodsConstructionCost { get; set; } = new();
    public Dictionary<string, ProductionDto> OutputGoods { get; set; } = new();
    public Dictionary<string, int> UniqueInputGoods { get; set; } = new();
    public Dictionary<string, int> InputGoods { get; set; } = new();
    public Dictionary<AllEnums.CountyGoodType, int> CountyStockpiledGoods { get; set; } = new();

    // Misc
    public AllEnums.CountyGoodType CountyResourceType { get; set; }
    public AllEnums.FactionGoodType FactionResourceType { get; set; }
    public AllEnums.CountyImprovementStatus Status { get; set; }
    public int AllDailyWorkAmountAtImprovementCompleted { get; set; }

    // We will store **only IDs** for workers, not full population data
    public List<int> PopulationIdsAtImprovement { get; set; } = new();
}