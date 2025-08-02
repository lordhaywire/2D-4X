using System.Collections.Generic;
using PlayerSpace;

public class PopulationDto
{
    public int PopulationId { get; set; }
    public int? FactionId { get; set; }    // Store just ID, not whole FactionData
    public int Location { get; set; }
    public int LastLocation { get; set; }
    public int Destination { get; set; }

    // Info
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsMale { get; set; }
    public int Age { get; set; }

    // Personality / Hero Info
    public string Personality { get; set; }
    public bool IsHero { get; set; }
    public string HeroType { get; set; }
    public int NumberOfSubordinatesWanted { get; set; }
    public List<int> HeroSubordinates { get; set; } = []; // store subordinate IDs

    // Perks
    public Dictionary<string, PerkDto> Perks { get; set; } = new();

    // Expendables
    public int HitPoints { get; set; }
    public int MaxHitPoints { get; set; }
    public int MoraleExpendable { get; set; }
    public int LoyaltyBase { get; set; }
    public int LoyaltyAdjusted { get; set; }
    public int Happiness { get; set; }

    // Employment / time info
    public int DaysEmployed { get; set; }
    public int DaysEmployedButIdle { get; set; }
    public int DaysStarving { get; set; }
    public int DaysRecruited { get; set; }
    public int DaysUntilServiceStarts { get; set; }

    // Needs
    public Dictionary<string, int> Needs { get; set; } = new();

    // Attributes / Skills
    public Dictionary<string, AttributeDto> Attributes { get; set; } = new();
    public Dictionary<string, SkillDto> Skills { get; set; } = new();
    public string PreferredSkill { get; set; }        // reference by name/ID
    public string InterestData { get; set; }          // reference by name/ID

    // Work
    public string Activity { get; set; }

    // Inventory
    public bool UseNewestEquipment { get; set; }
    public Dictionary<string, GoodDto> Inventory { get; set; }

    // Current County Improvement & Research
    public string CurrentCountyImprovement { get; set; }
    public string PassiveResearchItem { get; set; }
    public string CurrentResearchItem { get; set; }
}
