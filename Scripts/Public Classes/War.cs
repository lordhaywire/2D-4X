using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;

public class War
{
    public FactionData aggressorFactionData;
    public FactionData defenderFactionData;
    //public List<Battle> battles = new();
    
    public WarDto ToDto()
    {
        return new WarDto
        {
            AggressorFactionId = aggressorFactionData.factionId,
            DefenderFactionId = defenderFactionData.factionId
        };
    }

    // ToDo: This needs to be converted in the save.
    public static War FromDto(WarDto dto, Godot.Collections.Array<FactionData> allFactions)
    {
        return new War
        {
            aggressorFactionData = allFactions.FirstOrDefault(f => f.factionId == dto.AggressorFactionId),
            defenderFactionData = allFactions.FirstOrDefault(f => f.factionId == dto.DefenderFactionId)
        };
    }

}