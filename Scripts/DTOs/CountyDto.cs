using System.Collections.Generic;
using Godot;

namespace PlayerSpace;
public class CountyDto
{
    // Map Editor
    public int CountyId { get; set; }
    public string CountyName { get; set; }

    // Flags
    public bool IsPlayerCapital { get; set; }
    public bool IsAiCapital { get; set; }

    // Faction
    public int FactionId { get; set; }

    // Province
    public AllEnums.Province Province { get; set; }

    // Terrain and Exploration
    public AllEnums.Terrain PrimaryTerrain { get; set; }
    public AllEnums.Terrain SecondaryTerrain { get; set; }
    public AllEnums.Terrain TertiaryTerrain { get; set; }
    public List<AllEnums.Terrain> AllTerrains { get; set; } = [];
    public List<string> ExplorationEvents { get; set; } = []; // just storing event names/ids
    
    // Population Lists
    public List<PopulationDto> PopulationDataList { get; set; } = [];
    public List<PopulationDto> HeroesInCountyList { get; set; } = [];
    public List<PopulationDto> ArmiesInCountyList { get; set; } = [];
    public List<PopulationDto> VisitingHeroList { get; set; } = [];
    public List<PopulationDto> VisitingArmyList { get; set; } = [];

    // Construction and Work Lists
    public List<PopulationDto> HeroBuildersList { get; set; } = [];
    public List<PopulationDto> HeroWorkersList { get; set; } = [];
    public List<PopulationDto> WorkersList { get; set; } = [];

    public List<PopulationDto> PrioritizedHeroBuildersList { get; set; } = [];
    public List<PopulationDto> PrioritizedHeroWorkersList { get; set; } = [];
    public List<PopulationDto> PrioritizedBuildersList { get; set; } = [];
    public List<PopulationDto> PrioritizedWorkersList { get; set; } = [];
    public List<PopulationDto> WorkersToRemoveFromLists { get; set; } = [];

    public List<CountyImprovementDto> PrioritizedConstructionImprovementList { get; set; } = [];
    public List<CountyImprovementDto> PrioritizedWorkImprovementList { get; set; } = [];
    public List<CountyImprovementDto> UnderConstructionCountyImprovementList { get; set; } = [];
    public List<CountyImprovementDto> CompletedCountyImprovementList { get; set; } = [];

    // Storage & Resources
    public int PopulationCount { get; set; }
    public int PerishableStorage { get; set; }
    public int NonperishableStorage { get; set; }
    public int ScavengeableRemnants { get; set; }
    public int ScavengeableCannedFood { get; set; }

    public Dictionary<AllEnums.CountyGoodType, GoodDto> Goods { get; set; } = new();
    public Dictionary<AllEnums.CountyGoodType, GoodDto> YesterdaysGoods { get; set; } = new();
    public Dictionary<AllEnums.CountyGoodType, GoodDto> AmountOfGoodsUsed { get; set; } = new();

    // Map Visuals
    public string ColorHex { get; set; }
    public Vector2I StartMaskPosition { get; set; }
    public Vector2I CountyOverlayLocalPosition { get; set; }
    public string MaskTexturePath { get; set; }
    public string MapTexturePath { get; set; }

    // Workers / Misc
    public bool Selected { get; set; }
    public int IdleWorkers { get; set; }
}
