using Godot;

namespace PlayerSpace;

[GlobalClass]
public partial class PerkData : Resource
{
    [Export] public string perkName;
    [Export] public string perkDescription;
    [Export] public int perkBonus;

    public static int GetPerkBonus(PopulationData populationData, AllEnums.Perks perk)
    {
        return populationData.perks.TryGetValue(perk, out PerkData dataPerk) ? dataPerk.perkBonus : 0;
    }
    
    public static PerkDto ToDto(PerkData perk)
    {
        if (perk == null) return null;

        return new PerkDto
        {
            PerkName = perk.perkName,
            PerkDescription = perk.perkDescription,
            PerkBonus = perk.perkBonus
        };
    }
    
    public static PerkData FromDto(PerkDto dto)
    {
        if (dto == null) return null;

        PerkData perk = new PerkData
        {
            perkName = dto.PerkName,
            perkDescription = dto.PerkDescription,
            perkBonus = dto.PerkBonus
        };

        return perk;
    }
}