using Godot;
using System;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class CountyResourceData : Resource
    {
        [Export] public string name;
        [Export] public string description;
        [Export] public AllEnums.CountyResourceType countyResourceType;
        [Export] public AllEnums.FactionResourceType factionResourceType;
        [Export] public bool perishable;

        public int amount; // The amount of resource.
        private int maxAmount; // This is the max amount that can be stored.

        public int MaxAmount
        {
            get { return maxAmount; }
            set
            {
                maxAmount = value;
                amount = Math.Min(amount, maxAmount);
                GD.Print($"Getter Setter: {name} {maxAmount}");
            }
        }
    }
}