using Godot;
using System.Linq;

namespace PlayerSpace;

public partial class AssignFactionColorsToCounties : Node
{
    public override void _Ready()
    {
        ApplyFactionColorsToCounties();
    }

    private static void ApplyFactionColorsToCounties()
    {
        foreach(County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
        {
            FactionData factionData =
                SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(county.countyData.factionId);
            county.countySprite.SelfModulate = factionData.factionColor;
        }
    }
}