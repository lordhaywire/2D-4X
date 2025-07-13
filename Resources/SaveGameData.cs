using Godot;

namespace PlayerSpace;
public partial class SaveGameData : Resource
{
    [Export] public Godot.Collections.Array<FactionData> allFactionDataList = [];
}
