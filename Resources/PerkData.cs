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
}