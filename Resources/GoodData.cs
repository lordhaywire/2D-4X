using Godot;
using System;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class GoodData : Resource
    {
        [Export] public string goodName;
        [Export] public string description;
        [Export] public AllEnums.GoodType goodType;
        [Export] public AllEnums.CountyGoodType countyGoodType;
        [Export] public AllEnums.FactionGoodType factionGoodType;
        [Export] public AllEnums.Perishable perishable;
        [Export] public int failureRate; // Daily chance of the item to fail. This isn't used yet.
        [Export] public bool remnantSubstitutable;
        [Export] private int amount; // The amount of resource.

        // I don't think we need to export the Getter Setter because the private int amount
        // is exported.
        public int Amount
        {
            get { return amount; }
            set
            {
                // Make the amount never got above MaxAmount.
                amount = Math.Min(value, MaxAmount);
                //// GD.Print($"Resource Amount: {name} has been set to {amount}");

            }
        }

        [Export] private int maxAmount = int.MaxValue; // This is the max amount that can be stored.

        public int MaxAmount
        {
            get { return maxAmount; }
            set
            {
                if (goodType == AllEnums.GoodType.FactionGood)
                {
                    maxAmount = int.MaxValue;
                    GD.PrintRich($"[rainbow]Good Max Amount: {goodName} {maxAmount}");
                    return;
                }

                maxAmount = value;
                Amount = Math.Min(Amount, maxAmount);
                GD.Print($"Good Max Amount: {goodName} {maxAmount}");
            }
        }
    }
}