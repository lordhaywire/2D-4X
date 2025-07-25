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
    public List<string> ResearchItems { get; set; } = [];
    public List<string> ResearchableResearch { get; set; } = [];
    public List<string> CountiesFactionOwns { get; set; } = [];
    public List<int> AllHeroesList { get; set; } = [];
    public int FactionLeader { get; set; }

    public List<string> AllCountyImprovements { get; set; } = [];
    public List<string> ResearchOffices { get; set; } = [];

    // Goods
    public Dictionary<string, GoodDto> FactionGoods { get; set; } = new();
    public Dictionary<string, GoodDto> YesterdaysFactionGoods { get; set; } = new();
    public Dictionary<string, GoodDto> AmountUsedFactionGoods { get; set; } = new();

    // Diplomacy & War
    public List<WarDto> Wars { get; set; } = new();
    public Dictionary<string, bool> FactionWarDictionary { get; set; } = new();
    
    
}