using Godot;
using System;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class PerkData : Resource
    {
        [Export] public string perkName;
        [Export] public string perkDescription;

        public bool CheckForPerk(CountyPopulation countyPopulation, AllEnums.Perks perk)
        {
            if (countyPopulation.perks.ContainsKey(perk))
            {
                return true;
            }
            return false;
        }
    }
}