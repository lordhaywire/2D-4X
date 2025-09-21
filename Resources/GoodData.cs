using Godot;
using System;
using System.Linq;
using System.Reflection;
using AutoloadSpace;

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

    
    public int MaxAmount
    {
        get => maxAmount;
        set
        {
            if (goodType == AllEnums.GoodType.FactionGood)
            {
                maxAmount = int.MaxValue;
                GD.PrintRich($"Faction Good Max Amount: {goodName} {maxAmount}");
                return;
            }

            maxAmount = value;
            Amount = Math.Min(Amount, maxAmount);
            GD.Print($"Good Max Amount: {goodName} {maxAmount}");
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
            Amount = amount,
            MaxAmount = maxAmount,
            // if there is an EquipmentData resource, store its EquipmentType as string
            EquipmentData = equipmentData?.inventorySlot.ToString()
        };
    }

    public static GoodData FromDto(GoodDto goodDto)
    {
        GoodData goodData = new GoodData
        {
            goodName = goodDto.GoodName,
            description = goodDto.Description,
            goodType = Enum.Parse<AllEnums.GoodType>(goodDto.GoodType),
            countyGoodType = Enum.Parse<AllEnums.CountyGoodType>(goodDto.CountyGoodType),
            factionGoodType = Enum.Parse<AllEnums.FactionGoodType>(goodDto.FactionGoodType),
            perishable = Enum.Parse<AllEnums.Perishable>(goodDto.Perishable),
            failureRate = goodDto.FailureRate,
            remnantSubstitutable = goodDto.RemnantSubstitutable,
            useRemnants = goodDto.UseRemnants,
            amount = goodDto.Amount,
            maxAmount = goodDto.MaxAmount
        };
        
        // If EquipmentType exists, you can later decide how to load/create the correct EquipmentData
        if (!string.IsNullOrEmpty(goodDto.EquipmentData))
        {
            // Either just set type if you're creating EquipmentData later
            goodData.equipmentData = new EquipmentData
            {
                inventorySlot = Enum.Parse<AllEnums.InventorySlot>(goodDto.EquipmentData)
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

    public static GoodData GetGoodFromAutoloadList(AllEnums.GoodType goodType)
    {
        return Autoload.Instance.allGoodData.FirstOrDefault(goodData => goodData.goodType == goodType);
    }
    public static class ReflectionCopy
    {
        public static T NewCopy<T>(T original) where T : new()
        {
            if (original == null)
                throw new ArgumentNullException(nameof(original));

            T copy = new T();

            // Copy public properties
            foreach (PropertyInfo property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanRead && property.CanWrite)
                {
                    object value = property.GetValue(original);
                    property.SetValue(copy, value);
                }
            }

            // Copy public fields
            foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                object value = field.GetValue(original);
                field.SetValue(copy, value);
            }

            return copy;
        }
    }

}