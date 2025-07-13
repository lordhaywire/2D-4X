using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoloadSpace;

namespace PlayerSpace;

public partial class RandomFactionColor : Node
{
    private readonly Random random = new();
    public override void _Ready()
    {
        RandomFactionColors();
        ApplyFactionColorsToCounties();
    }

    private void RandomFactionColors()
    {
        if (Arrays.colors.Length < Autoload.Instance.allFactionDataList.Count)
        {
            //GD.Print("Not enough color options for all Sprite Renderers!");
            return;
        }

        // Create a list of available color32 options
        List<Color> availableColors = new(Arrays.colors);

        // Loop through each factionNameAndColors and assign a random color32 from available options
        foreach (FactionData factionData in Autoload.Instance.allFactionDataList)
        {
            int randomIndex = random.Next(0, availableColors.Count);
            factionData.factionColor = availableColors[randomIndex];
            //GD.Print("Faction Data attempting to get color: " + factionData.factionName + factionData.factionColor);
            availableColors.RemoveAt(randomIndex);
        }
    }

    private static void ApplyFactionColorsToCounties()
    {
        foreach(County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
        {
            county.countySprite.SelfModulate = county.countyData.factionData.factionColor;
        }
    }
}