using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class RandomFactionColor : Node
    {
        private readonly Random random = new();
        public override void _Ready()
        {
            RandomFactionColors();
            ApplyFactionColorsToCounties();

            //CallDeferred("RandomFactionColors");
            //CallDeferred("ApplyFactionColorsToCounties");
        }

        private void RandomFactionColors()
        {
            List<FactionData> factions = FactionGeneration.Instance.factions;
            if (Arrays.colors.Length < factions.Count)
            {
                GD.Print("Not enough color options for all Sprite Renderers!");
                return;
            }

            // Create a list of available color32 options
            List<Color> availableColors = new(Arrays.colors);

            // Loop through each factionNameAndColors and assign a random color32 from available options
            for (int i = 0; i < factions.Count; i++)
            {
                int randomIndex = random.Next(0, availableColors.Count);
                factions[i].factionColor = availableColors[randomIndex];
                availableColors.RemoveAt(randomIndex);
            }
        }

        private static void ApplyFactionColorsToCounties()
        {
            foreach(Node county in Globals.Instance.countiesParent.GetChildren())
            {
                SelectCounty selectCounty = (SelectCounty)county;
                county.GetNode<Sprite2D>("County Sprite2D").SelfModulate = selectCounty.countyData.faction.factionColor;
            }
        }
    }
}

