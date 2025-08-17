using AutoloadSpace;
using Godot;

namespace PlayerSpace;

public partial class InitialLoadGame : Node
{
    public override void _Ready()
    {
        SaveManager.Instance.saveGameData = new SaveGameData();
        //GD.PrintRich($"[rainbow]Save Game Data {SaveManager.Instance.saveGameData.allFactionDataList[0]?.factionName}");
        SaveManager.Instance.LoadGame();
        // Add Player Specific Stuff Here
        foreach (FactionData factionData in SaveManager.Instance.saveGameData.allFactionDataList)
        {
            if (!factionData.isPlayer) continue;
            Autoload.Instance.playerFactionData = factionData;
            break;
        }
    }
}