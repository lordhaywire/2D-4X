using System.Collections.Generic;

namespace PlayerSpace;

public class SaveGameDto
{
    public string SaveTimestamp { get; set; } // Store in ISO-like format
    public float SaveVersion { get; set; }
    public int CurrentPopulationId { get; set; }

    //public List<PopulationDto> AllPopulationDataList { get; set; } = [];
    
    public List<CountyDto> AllCountyDataList { get; set; } = [];
    public List<FactionDto> AllFactionDataList { get; set; } = [];
}