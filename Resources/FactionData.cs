using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using AutoloadSpace;

namespace PlayerSpace;

[GlobalClass]
public partial class FactionData : Resource
{
    [ExportGroup("Faction Info")] 
    [Export] public int factionId;
    [Export] public bool isPlayer;
    [Export] public string factionName;
    [Export] public Color factionColor;
    [Export] public AllEnums.FactionStatus factionStatus;
    [Export] public int factionCapitalCounty;

    [Export] public Godot.Collections.Array<ResearchItemData> researchItems;
    // Why are we saving this?  It happens every day at Day Start.  It could be a temporary list just used when needed.
    [Export] public Godot.Collections.Array<ResearchItemData> researchableResearch; 

    [Export] public Godot.Collections.Array<CountyData> countiesFactionOwns;
    public Dictionary<int,int> allHeroesDictionary; // Hero Id, hero location
    [Export] public PopulationData factionLeader;

    public readonly Diplomacy diplomacy = new();

    // This includes all county improvements, even possible ones.
    [Export] public Godot.Collections.Array<CountyImprovementData> allFactionKnownCountyImprovements; 

    // All Faction Research Offices.
    [Export] public Godot.Collections.Array<CountyImprovementData> researchOffices;

    // Goods.
    [ExportGroup("Goods")] 
    [Export] public Godot.Collections.Dictionary<AllEnums.FactionGoodType, GoodData> factionGoods;
    [Export] public Godot.Collections.Dictionary<AllEnums.FactionGoodType, GoodData> yesterdaysFactionGoods;
    [Export] public Godot.Collections.Dictionary<AllEnums.FactionGoodType, GoodData> amountUsedFactionGoods;

    public readonly List<War> wars = [];

    [ExportGroup("Diplomatic Matrix")] 
    [Export] public Godot.Collections.Dictionary<string, bool> factionWarDictionary;

    public FactionDto ToDto()
    {
        FactionDto factionDto = new FactionDto
        {
            FactionId = factionId,
            IsPlayer = isPlayer,
            FactionName = factionName,
            FactionColor = factionColor.ToHtml(), // Convert Color → Hex string
            FactionStatus = factionStatus.ToString(),
            FactionCapitalCounty = factionCapitalCounty
        };

        foreach (ResearchItemData research in researchItems)
            factionDto.ResearchItems.Add(research.ToDto());

        foreach (ResearchItemData research in researchableResearch)
            factionDto.ResearchableResearch.Add(research.researchName);

        foreach (CountyData county in countiesFactionOwns)
            factionDto.CountiesFactionOwns.Add(county.countyName);

        foreach (PopulationData hero in allHeroesDictionary)
            factionDto.AllHeroesList.Add(hero.populationId);

        if (factionLeader != null)
            factionDto.FactionLeader = factionLeader.populationId;

        foreach (CountyImprovementData improvement in allFactionKnownCountyImprovements)
            factionDto.AllCountyImprovements.Add(improvement.improvementName);

        factionDto.ResearchOffices = new List<CountyImprovementDto>();

        foreach (CountyImprovementData office in researchOffices)
        {
            factionDto.ResearchOffices.Add(office.ToDto());
        }

        // Goods (convert to GoodDto)
        foreach (KeyValuePair<AllEnums.FactionGoodType, GoodData> keyValuePair in factionGoods)
            factionDto.FactionGoods[keyValuePair.Key.ToString()] = keyValuePair.Value.ToDto();

        foreach (KeyValuePair<AllEnums.FactionGoodType, GoodData> keyValuePair in yesterdaysFactionGoods)
            factionDto.YesterdaysFactionGoods[keyValuePair.Key.ToString()] = keyValuePair.Value.ToDto();

        foreach (KeyValuePair<AllEnums.FactionGoodType, GoodData> keyValuePair in amountUsedFactionGoods)
            factionDto.AmountUsedFactionGoods[keyValuePair.Key.ToString()] = keyValuePair.Value.ToDto();

        // Wars
        foreach (War war in wars)
            factionDto.Wars.Add(war.ToDto());


        foreach (KeyValuePair<string, bool> keyValuePair in factionWarDictionary)
            factionDto.FactionWarDictionary[keyValuePair.Key] = keyValuePair.Value;

        return factionDto;
    }

    public static FactionData FromDto(FactionDto factionDto)
    {
        FactionData factionData = new FactionData
        {
            factionId = factionDto.FactionId,
            isPlayer = factionDto.IsPlayer,
            factionName = factionDto.FactionName,
            factionColor = new Color(factionDto.FactionColor), // Convert hex back to Godot Color
            factionStatus = Enum.Parse<AllEnums.FactionStatus>(factionDto.FactionStatus),
            factionCapitalCounty = factionDto.FactionCapitalCounty
        };

        factionData.researchItems = [];
        foreach (ResearchItemData loadedItem in factionDto.ResearchItems.Select(ResearchItemData.FromDto))
        {
            factionData.researchItems.Add(loadedItem);
        }

        factionData.researchableResearch = [];
        foreach (string researchName in factionDto.ResearchableResearch)
        {
            ResearchItemData found = Autoload.Instance.allResearchItemData
                .FirstOrDefault(r => r.researchName == researchName);

            if (found != null)
            {
                factionData.researchableResearch.Add(found);
            }
            else
            {
                GD.PrintErr($"[FactionData.FromDto] Could not find ResearchItemData for {researchName}");
            }
        }

        // TODO: This is going to break when we move County Generation from Main to Game Generation.
        factionData.countiesFactionOwns = [];
        foreach (string countyName in factionDto.CountiesFactionOwns)
        {
            // Go through every County node under countiesParent
            foreach (Node child in Globals.Instance.countiesParent.GetChildren())
            {
                if (child is County county)
                {
                    // Match the name (or some other unique property)
                    if (county.countyData.countyName == countyName)
                    {
                        factionData.countiesFactionOwns.Add(county.countyData);
                        break; // stop looping once we find it
                    }
                }
            }
        }

        factionData.allHeroesDictionary = [];
        foreach (int heroId in factionDto.AllHeroesList)
        {
            
            PopulationData hero = SaveManager.Instance.saveGameData.allCountyDataList[h]
                .FirstOrDefault(p => p.populationId == heroId);

            if (hero != null)
            {
                factionData.allHeroesDictionary.Add(hero);
            }
        }

        if (factionDto.FactionLeader > -1)
        {
            PopulationData leader = SaveManager.Instance.saveGameData.allPopulationDataList
                .FirstOrDefault(p => p.populationId == factionDto.FactionLeader);

            if (leader != null)
            {
                factionData.factionLeader = leader;
            }
            else
            {
                GD.PrintErr($"Faction leader with ID {factionDto.FactionLeader} not found in save data.");
            }
        }


        // List of all the possible county improvements a faction can build.
        factionData.allFactionKnownCountyImprovements = [];
        foreach (string improvementName in factionDto.AllCountyImprovements)
        {
            CountyImprovementData improvement = Autoload.Instance.allCountyImprovementData
                .FirstOrDefault(c => c.improvementName == improvementName);

            if (improvement != null)
            {
                factionData.allFactionKnownCountyImprovements.Add(improvement);
            }
            else
            {
                GD.PrintErr($"CountyImprovement '{improvementName}' not found in allCountyImprovementData.");
            }
        }

        factionData.researchOffices = [];
        foreach (CountyImprovementData countyImprovement in factionDto.ResearchOffices.Select(CountyImprovementData.FromDto))
        {
            factionData.researchOffices.Add(countyImprovement);
        }

        // Convert faction goods dictionaries
        factionData.factionGoods = new Godot.Collections.Dictionary<AllEnums.FactionGoodType, GoodData>();
        foreach (KeyValuePair<string, GoodDto> keyValuePair in factionDto.FactionGoods)
        {
            factionData.factionGoods[Enum.Parse<AllEnums.FactionGoodType>(keyValuePair.Key)] = GoodData.FromDto(keyValuePair.Value);
        }

        factionData.yesterdaysFactionGoods = new Godot.Collections.Dictionary<AllEnums.FactionGoodType, GoodData>();
        foreach (KeyValuePair<string, GoodDto> keyValuePair in factionDto.YesterdaysFactionGoods)
        {
            factionData.yesterdaysFactionGoods[Enum.Parse<AllEnums.FactionGoodType>(keyValuePair.Key)] =
                GoodData.FromDto(keyValuePair.Value);
        }

        factionData.amountUsedFactionGoods = new Godot.Collections.Dictionary<AllEnums.FactionGoodType, GoodData>();
        foreach (KeyValuePair<string, GoodDto> keyValuePair in factionDto.AmountUsedFactionGoods)
        {
            factionData.amountUsedFactionGoods[Enum.Parse<AllEnums.FactionGoodType>(keyValuePair.Key)] =
                GoodData.FromDto(keyValuePair.Value);
        }

        //TODO: This needs to be loaded after the all faction data list is loaded.
        // Wars – rebuild as War objects
        factionData.wars.Clear();
        foreach (WarDto warDto in factionDto.Wars)
        {
           //War war = War.FromDto(warDto, SaveManager.Instance.saveGameData.allFactionDataList);
            //factionData.wars.Add(war);
        }

        // Diplomatic matrix
        factionData.factionWarDictionary = new Godot.Collections.Dictionary<string, bool>();
        foreach (KeyValuePair<string, bool> keyValuePair in factionDto.FactionWarDictionary)
        {
            factionData.factionWarDictionary[keyValuePair.Key] = keyValuePair.Value;
        }
        return factionData;
    }


    public static FactionData GetFactionDataFromId(int id)
    {
        GD.Print("Faction ID that is trying to be used: " + id);
        FactionData factionData = SaveManager.Instance.saveGameData.allFactionDataList[id];
        return factionData;
    }
    
    public void AddHeroToAllHeroesList(PopulationData populationData)
    {
        // We need to double-check that the hero isn't already in the dictionary.
        if (!allHeroesDictionary.ContainsKey(populationData.populationId))
        {
            FactionData factionData = GetFactionDataFromId(populationData.factionId);
            factionData.allHeroesDictionary[populationData.populationId] = populationData.location;
        }


        GD.Print($"{populationData.firstName} has been added to {factionName} all heroes list.");
    }

    // This isn't used yet, but when heroes die...Can heroes starve to death?
    public void RemoveHeroFromAllHeroesList(PopulationData populationData)
    {
        allHeroesDictionary.Remove(populationData.populationId);
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
            //GD.Print("Yesterday's Influence: "+ yesterdaysFactionResources[AllEnums.FactionResourceType.Influence].amount);
        }
    }

    public void AddCountyImprovementToAllCountyImprovements(CountyImprovementData countyImprovementData)
    {
        // Generates the stockpile good dictionary.
        Haulmaster.GenerateStockpileGoodsDictionary(countyImprovementData);

        allFactionKnownCountyImprovements.Add(CountyImprovementData.NewCopy(countyImprovementData));
        //GD.PrintRich($"[rainbow][tornado]{factionName} {countyImprovementData.improvementName} has been added.");
        // Alphabetize the list by improvementName
        allFactionKnownCountyImprovements
            = [.. allFactionKnownCountyImprovements.OrderBy(improvement => Tr(improvement.improvementName))];
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