using System.Collections.Generic;

namespace PlayerSpace;

public class FactionDto
{
    // Faction Info
    public int FactionId { get; set; }
    public bool IsPlayer { get; set; }
    public string FactionName { get; set; }
    public string FactionColor { get; set; }   // Stored as HEX string, e.g., "#FFAA00"
    public string FactionStatus { get; set; }
    public int FactionCapitalCounty { get; set; }

    // Collections
    public List<ResearchItemDto> ResearchItems { get; set; } = [];
    public List<string> ResearchableResearch { get; set; } = [];
    public List<string> CountiesFactionOwns { get; set; } = [];
    public Dictionary<int, int> AllHeroesDictionary { get; set; } = [];
    
    public (int HeroId, int CountyId) FactionLeader { get; set; }

    public List<string> AllCountyImprovements { get; set; } = [];
    public List<CountyImprovementDto> ResearchOffices { get; set; } = [];

    // Goods
    public Dictionary<string, GoodDto> FactionGoods { get; set; } = new();
    public Dictionary<string, GoodDto> YesterdaysFactionGoods { get; set; } = new();
    public Dictionary<string, GoodDto> AmountUsedFactionGoods { get; set; } = new();

    // Diplomacy & War
    public List<WarDto> Wars { get; set; } = [];
    public Dictionary<string, bool> FactionWarDictionary { get; set; } = new();
}