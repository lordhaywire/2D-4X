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

        private int amount; // The amount of resource.
        public int Amount
        {
            get { return amount; }
            set
            {
                // Make the amount never got above MaxAmount.
                amount = Math.Min(value, MaxAmount);
                //GD.Print($"Resource Amount: {name} has been set to {amount}");

            }
        }
        private int maxAmount; // This is the max amount that can be stored.

        public int MaxAmount
        {
            get { return maxAmount; }
            set
            {
                maxAmount = value;
                Amount = Math.Min(Amount, maxAmount);
                //GD.Print($"Resource Max Amount: {name} {maxAmount}");
            }
        }
    }
}