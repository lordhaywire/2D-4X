using Godot;
using System;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class ResourceData : Resource
    {
        [Export] public string resourceName;
        [Export] public AllEnums.CountyResourceType countyResourceType;
        [Export] public AllEnums.FactionResourceType factionResourceType;
        [Export] public bool perishable;

        public int amount;
        private int maxAmount;

        // Write a getter setter for amount never to be above maxAmount.
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