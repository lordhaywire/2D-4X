using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace;
public partial class RandomlyAssignColorsToFactions : Node
{
    private readonly Random random = new();

    public override void _Ready()
    {
        RandomFactionColors();
    }

    private void RandomFactionColors()
    {
        
        if (Arrays.Colors.Length < SaveManager.Instance.saveGameData.allFactionDataList.Count)
        {
            GD.Print("Not enough color options for all Sprite Renderers!");
            return;
        }

        // Create a list of available color32 options
        List<Color> availableColors = new(Arrays.Colors);

        // Loop through each factionNameAndColors and assign a random color32 from available options
        foreach (FactionData factionData in SaveManager.Instance.saveGameData.allFactionDataList)
        {
            int randomIndex = random.Next(0, availableColors.Count);
            factionData.factionColor = availableColors[randomIndex];
            GD.Print("Faction Data attempting to get color: " + factionData.factionName + factionData.factionColor);
            availableColors.RemoveAt(randomIndex);
        }
    }
}
