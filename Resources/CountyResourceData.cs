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

        // Write a getter setter for amount never to be above maxAmount - That getter setter looks wrong to me.
        // Shouldn't the amount be zero?
        public int MaxAmount
        {
            get { return maxAmount; }
            set
            {
                maxAmount = value;
                amount = Math.Min(amount, maxAmount);
                //GD.Print($"Getter Setter: {resourceName} {maxAmount}");
            }
        }
    }
}