using System.Collections.Generic;
using Godot;

namespace PlayerSpace;
public partial class SaveGameData : Resource
{
    private float saveVersion = 0.01f;
    public int currentPopulationId;
    // Since we don't really need to see this in the inspector when the game is running
    // and since the list below doesn't even show the correct data it is a C# list.
    public List<PopulationData> allPopulationDataList = []; 
    [Export] public Godot.Collections.Array<FactionData> allFactionDataList = [];
    
    public SaveGameDto ToDto()
    {
        SaveGameDto dto = new SaveGameDto
        {
            SaveVersion = saveVersion,
            CurrentPopulationId = currentPopulationId,
            AllPopulationDataList = allPopulationDataList.ConvertAll(p => p.ToDto()),
            AllFactionDataList = []
        };

        foreach (FactionData faction in allFactionDataList)
        {
            dto.AllFactionDataList.Add(faction.ToDto());
        }

        return dto;
    }
    
    public static SaveGameData FromDto(SaveGameDto saveGameDto)
    {
        SaveGameData saveGameData = new SaveGameData
        {
            saveVersion = saveGameDto.SaveVersion,
            currentPopulationId = saveGameDto.CurrentPopulationId,
            allPopulationDataList = [],
            allFactionDataList = []
        };

        foreach (PopulationDto popDto in saveGameDto.AllPopulationDataList)
        {
            saveGameData.allPopulationDataList.Add(PopulationData.FromDto(popDto));
        }

        foreach (FactionDto factionDto in saveGameDto.AllFactionDataList)
        {
            saveGameData.allFactionDataList.Add(FactionData.FromDto(factionDto));
        }

        return saveGameData;
    }
}
