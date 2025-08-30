using System;
using System.Collections.Generic;
using Godot;

namespace PlayerSpace;
public partial class SaveGameData : Resource
{
    // Store when the save was created (runtime)
    public DateTime saveTimestamp;
    private float saveVersion = 0.01f;
    public int currentPopulationId;

    //public List<PopulationData> allPopulationDataList = []; 
    [Export] public Godot.Collections.Array<CountyData> allCountyDataList = [];
    [Export] public Godot.Collections.Array<FactionData> allFactionDataList = [];

    public FactionData ConvertFactionIdToFactionData(int factionId)
    {
        return allFactionDataList[factionId];
    }
    
    public SaveGameDto ToDto()
    {
        SaveGameDto saveGameDto = new SaveGameDto
        {
            SaveTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Store timestamp as string
            SaveVersion = saveVersion,
            CurrentPopulationId = currentPopulationId,
            //AllPopulationDataList = allPopulationDataList.ConvertAll(p => p.ToDto()),
            AllCountyDataList = [],
            AllFactionDataList = []
        };

        foreach (CountyData countyData in allCountyDataList)
        {
            saveGameDto.AllCountyDataList.Add(countyData.ToDto());
        }
        foreach (FactionData faction in allFactionDataList)
        {
            saveGameDto.AllFactionDataList.Add(faction.ToDto());
        }
        return saveGameDto;
    }
    
    public static void FromDto(SaveGameDto saveGameDto)
    {
        SaveManager.Instance.saveGameData = new SaveGameData
        {
            saveTimestamp = DateTime.TryParse(saveGameDto.SaveTimestamp, out var parsed) ? parsed : DateTime.MinValue,
            saveVersion = saveGameDto.SaveVersion,
            currentPopulationId = saveGameDto.CurrentPopulationId,
            allCountyDataList = [],
            allFactionDataList = []
        };
        
        /*
        foreach (PopulationDto popDto in saveGameDto.AllPopulationDataList)
        {
            SaveManager.Instance.saveGameData.allPopulationDataList.Add(PopulationData.FromDto(popDto));
        }
        */

        foreach (CountyDto countyDto in saveGameDto.AllCountyDataList)
        {
            GD.PrintRich($"[rainbow]SaveGameData.cs: FromDto: {countyDto.CountyName}");
            SaveManager.Instance.saveGameData.allCountyDataList.Add(CountyData.FromDto(countyDto));
        }
        
        foreach (FactionDto factionDto in saveGameDto.AllFactionDataList)
        {
            //GD.PrintRich($"[rainbow]Count of All Population Data List: {SaveManager.Instance.saveGameData.allPopulationDataList.Count}");
            SaveManager.Instance.saveGameData.allFactionDataList.Add(FactionData.FromDto(factionDto));
        }
    }
}
