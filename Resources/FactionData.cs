using Godot;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;

[GlobalClass]
public partial class FactionData : Resource
{
    [ExportGroup("Faction Info")]
    [Export] public int factionId;
    [Export] public bool isPlayer;
    [Export] public string factionName;
    [Export] public Color factionColor;
    [Export] public int factionCapitalCounty;

    [Export] public Godot.Collections.Array<ResearchItemData> researchItems;// = [];
    [Export] public Godot.Collections.Array<ResearchItemData> researchableResearch;// = [];

    [Export] public Godot.Collections.Array<CountyData> countiesFactionOwns;// = [];
    [Export] public Godot.Collections.Array<PopulationData> allHeroesList;// = [];
    [Export] public PopulationData factionLeader;

    public readonly Diplomacy diplomacy = new();

    [Export] public Godot.Collections.Array<CountyImprovementData> allCountyImprovements;// = []; // This includes all county improvements, even possible ones.

    // All Faction Research Offices.
    [Export] public Godot.Collections.Array<CountyImprovementData> researchOffices;// = [];

    // Goods.
    [ExportGroup("Goods")]
    [Export] public Godot.Collections.Dictionary<AllEnums.FactionGoodType, GoodData> factionGoods;// = [];
    [Export] public Godot.Collections.Dictionary<AllEnums.FactionGoodType, GoodData> yesterdaysFactionGoods;// = [];
    [Export] public Godot.Collections.Dictionary<AllEnums.FactionGoodType, GoodData> amountUsedFactionGoods;// = [];

    public readonly List<War> wars = [];

    [ExportGroup("Diplomatic Matrix")]
    [Export] public Godot.Collections.Dictionary<string, bool> factionWarDictionary;// = [];

    public static FactionData GetFactionDataFromId(int id)
    {
        GD.Print("Faction ID that is trying to be used: " + id);
        Faction faction = (Faction)Globals.Instance.factionsParent.GetChild(id);
        return faction.factionData;
    }

    // We can error on the side of adding the hero to the All Heroes List because this checks
    // to see if the hero is already in the list.
    public void AddHeroToAllHeroesList(PopulationData populationData)
    {
        // We need to double-check that the hero isn't already in the list.
        if (!allHeroesList.Contains(populationData))
        {
            populationData.factionData.allHeroesList.Add(populationData);
            //GD.Print($"Add To {populationData.factionData.factionName} Hero List: " + populationData.lastName);
        }

        GD.Print($"{populationData.firstName} has been added to {factionName} all heroes list.");
    }

    // This isn't used yet, but when heroes die...Can heroes starve to death?
    public void RemoveHeroFromAllHeroesList(PopulationData populationData)
    {
        allHeroesList.Remove(populationData);
        GD.Print($"{populationData.firstName} has been removed from {factionName} all heroes list.");
    }

    public void CopyFactionResourcesToYesterday()
    {
        // Creating a deep copy of the dictionary
        yesterdaysFactionGoods = [];
        foreach (KeyValuePair<AllEnums.FactionGoodType, GoodData> keyValuePair in factionGoods)
        {
            yesterdaysFactionGoods.Add(keyValuePair.Key, new GoodData
            {
                goodName = keyValuePair.Value.goodName,
                description = keyValuePair.Value.description,
                factionGoodType = keyValuePair.Value.factionGoodType,
                Amount = keyValuePair.Value.Amount,
            });
        }
        if (isPlayer)
        {
            //GD.Print("Yesterday's Influence: " + yesterdaysFactionResources[AllEnums.FactionResourceType.Influence].amount);
            //GD.Print("This Influence should be the same as yesterdays: " + factionResources[AllEnums.FactionResourceType.Influence].amount);
        }
    }
    public void AddCountyImprovementToAllCountyImprovements(CountyImprovementData countyImprovementData)
    {
        // Generates the stockpile good dictionary.
        Haulmaster.GenerateStockpileGoodsDictionary(countyImprovementData);

        allCountyImprovements.Add(CountyImprovementData.NewCopy(countyImprovementData));
        //GD.PrintRich($"[rainbow][tornado]{factionName} {countyImprovementData.improvementName} has been added.");
        // Alphabetize the list by improvementName
        allCountyImprovements
            = [.. allCountyImprovements.OrderBy(improvement => Tr(improvement.improvementName))];

    }
    // Zero resources that are summed from each county.
    // Why not foreach this and skip the first two?
    private void ZeroFactionCountyResources()
    {
        factionGoods[AllEnums.FactionGoodType.Food].Amount = 0;
        factionGoods[AllEnums.FactionGoodType.Remnants].Amount = 0;
        factionGoods[AllEnums.FactionGoodType.BuildingMaterial].Amount = 0;
        factionGoods[AllEnums.FactionGoodType.Equipment].Amount = 0;
        factionGoods[AllEnums.FactionGoodType.RawMaterial].Amount = 0;
    }

    // This should be counting just the county resources of Faction Type, not the used.
    public void CountAllCountyFactionResources()
    {
        ZeroFactionCountyResources();
        foreach (CountyData countyData in countiesFactionOwns)
        {
            factionGoods[AllEnums.FactionGoodType.Food].Amount
                += countyData.CountFactionResourceOfType(AllEnums.FactionGoodType.Food);
            factionGoods[AllEnums.FactionGoodType.Remnants].Amount
                += countyData.CountFactionResourceOfType(AllEnums.FactionGoodType.Remnants);
            factionGoods[AllEnums.FactionGoodType.BuildingMaterial].Amount
                += countyData.CountFactionResourceOfType(AllEnums.FactionGoodType.BuildingMaterial);
            factionGoods[AllEnums.FactionGoodType.Equipment].Amount
                += countyData.CountFactionResourceOfType(AllEnums.FactionGoodType.Equipment);
            factionGoods[AllEnums.FactionGoodType.RawMaterial].Amount
                += countyData.CountFactionResourceOfType(AllEnums.FactionGoodType.RawMaterial);
        }
    }

    public void CountAllCountyFactionUsedResources()
    {
        ZeroFactionCountyActualUsedResources();
        foreach (CountyData countyData in countiesFactionOwns)
        {
            amountUsedFactionGoods[AllEnums.FactionGoodType.Food].Amount
                += countyData.CountUsedFactionResourceOfType(AllEnums.FactionGoodType.Food);
            amountUsedFactionGoods[AllEnums.FactionGoodType.Remnants].Amount
                += countyData.CountUsedFactionResourceOfType(AllEnums.FactionGoodType.Remnants);
            amountUsedFactionGoods[AllEnums.FactionGoodType.BuildingMaterial].Amount
                += countyData.CountUsedFactionResourceOfType(AllEnums.FactionGoodType.BuildingMaterial);
            amountUsedFactionGoods[AllEnums.FactionGoodType.Equipment].Amount
                += countyData.CountUsedFactionResourceOfType(AllEnums.FactionGoodType.Equipment);
            amountUsedFactionGoods[AllEnums.FactionGoodType.RawMaterial].Amount
                += countyData.CountUsedFactionResourceOfType(AllEnums.FactionGoodType.RawMaterial);
        }
    }

    // This is almost identical to the other zeroing out thing.
    // Why not a foreach loop and skip the first two?
    private void ZeroFactionCountyActualUsedResources()
    {
        amountUsedFactionGoods[AllEnums.FactionGoodType.Food].Amount = 0;
        amountUsedFactionGoods[AllEnums.FactionGoodType.Remnants].Amount = 0;
        amountUsedFactionGoods[AllEnums.FactionGoodType.BuildingMaterial].Amount = 0;
        amountUsedFactionGoods[AllEnums.FactionGoodType.Equipment].Amount = 0;
        amountUsedFactionGoods[AllEnums.FactionGoodType.RawMaterial].Amount = 0;
    }

    public void SubtractFactionResources()
    {
        // Do the math for amount used. Subtracted yesterday from today, and that is how much we have used.
        foreach (KeyValuePair<AllEnums.FactionGoodType, GoodData> keyValuePair in factionGoods)
        {
            amountUsedFactionGoods[keyValuePair.Key].Amount = factionGoods[keyValuePair.Key].Amount -
                yesterdaysFactionGoods[keyValuePair.Key].Amount;
        }
        if (isPlayer)
        {
            //GD.Print("After subtraction yesterday's influence is: " + yesterdaysFactionResources[AllEnums.FactionResourceType.Influence].amount);
        }
    }
}