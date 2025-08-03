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
        SaveGameDto saveGameDto = new SaveGameDto
        {
            SaveVersion = saveVersion,
            CurrentPopulationId = currentPopulationId,
            AllPopulationDataList = allPopulationDataList.ConvertAll(p => p.ToDto()),
            AllFactionDataList = []
        };

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
            saveVersion = saveGameDto.SaveVersion,
            currentPopulationId = saveGameDto.CurrentPopulationId,
            allPopulationDataList = [],
            allFactionDataList = []
        };
        
        foreach (PopulationDto popDto in saveGameDto.AllPopulationDataList)
        {
            SaveManager.Instance.saveGameData.allPopulationDataList.Add(PopulationData.FromDto(popDto));
        }

        foreach (FactionDto factionDto in saveGameDto.AllFactionDataList)
        {
            GD.PrintRich($"[rainbow]Count of All Population Data List: {SaveManager.Instance.saveGameData.allPopulationDataList.Count}");
            SaveManager.Instance.saveGameData.allFactionDataList.Add(FactionData.FromDto(factionDto));
        }
    }
}
