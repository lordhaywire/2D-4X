using Godot;
using System;

namespace PlayerSpace;

[GlobalClass]
public partial class GoodData : Resource
{
    [Export] public string goodName;
    [Export] public string description;
    [Export] public AllEnums.GoodType goodType;
    [Export] public AllEnums.CountyGoodType countyGoodType;
    [Export] public AllEnums.FactionGoodType factionGoodType;
    [Export] public AllEnums.Perishable perishable;
    [Export] public EquipmentData equipmentData;
    [Export] public int failureRate; // A daily chance of the item to fail. This isn't used yet.
    [Export] public bool remnantSubstitutable;
    [Export] public bool useRemnants;
    [Export] private int amount; // The amount of good.

    [Export]
    public int Amount
    {
        get => amount;
        set =>
            // Make the amount never got above MaxAmount.
            amount = Math.Min(value, MaxAmount);
        //GD.Print($"Resource Amount: {goodName} has been set to {amount}");
    }

    [Export] private int maxAmount; // This is the max amount that can be stored.

    [Export]
    public int MaxAmount
    {
        get => maxAmount;
        set
        {
            if (goodType == AllEnums.GoodType.FactionGood)
            {
                maxAmount = int.MaxValue;
                //GD.PrintRich($"Faction Good Max Amount: {goodName} {maxAmount}");
                return;
            }

            maxAmount = value;
            Amount = Math.Min(Amount, maxAmount);
            //GD.Print($"Good Max Amount: {goodName} {maxAmount}");
        }
    }

    public GoodDto ToDto()
    {
        return new GoodDto
        {
            GoodName = goodName,
            Description = description,
            GoodType = goodType.ToString(),
            CountyGoodType = countyGoodType.ToString(),
            FactionGoodType = factionGoodType.ToString(),
            Perishable = perishable.ToString(),
            FailureRate = failureRate,
            RemnantSubstitutable = remnantSubstitutable,
            UseRemnants = useRemnants,
            Amount = Amount,
            MaxAmount = MaxAmount,
            // ✅ if there is an EquipmentData resource, store its EquipmentType as string
            EquipmentData = equipmentData?.equipmentType.ToString()
        };
    }

    public static GoodData FromDto(GoodDto dto)
    {
        GoodData goodData = new GoodData
        {
            goodName = dto.GoodName,
            description = dto.Description,
            goodType = Enum.Parse<AllEnums.GoodType>(dto.GoodType),
            countyGoodType = Enum.Parse<AllEnums.CountyGoodType>(dto.CountyGoodType),
            factionGoodType = Enum.Parse<AllEnums.FactionGoodType>(dto.FactionGoodType),
            perishable = Enum.Parse<AllEnums.Perishable>(dto.Perishable),
            failureRate = dto.FailureRate,
            remnantSubstitutable = dto.RemnantSubstitutable,
            useRemnants = dto.UseRemnants,
            Amount = dto.Amount,
            MaxAmount = dto.MaxAmount
        };
        
        // If EquipmentType exists, you can later decide how to load/create the correct EquipmentData
        if (!string.IsNullOrEmpty(dto.EquipmentData))
        {
            // Either just set type if you're creating EquipmentData later
            goodData.equipmentData = new EquipmentData
            {
                equipmentType = Enum.Parse<AllEnums.EquipmentType>(dto.EquipmentData)
            };
        }

        return goodData;
    }

    public static GoodData NewCopy(GoodData goodData)
    {
        GoodData newGoodData = new()
        {
            goodName = goodData.goodName,
            description = goodData.description,
            goodType = goodData.goodType,
            countyGoodType = goodData.countyGoodType,
            factionGoodType = goodData.factionGoodType,
            perishable = goodData.perishable,
            failureRate = goodData.failureRate,
            remnantSubstitutable = goodData.remnantSubstitutable,
            useRemnants = goodData.useRemnants,
            amount = goodData.amount,
            Amount = goodData.Amount,
            maxAmount = goodData.maxAmount,
            MaxAmount = goodData.MaxAmount,
        };
        return newGoodData;
    }
}