using Godot;

namespace PlayerSpace;
public partial class SaveGameData : Resource
{
    public int currentPopulationId;
    [Export] public Godot.Collections.Array<FactionData> allFactionDataList = [];
}
