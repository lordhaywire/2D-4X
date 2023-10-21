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
            CallDeferred("RandomColors");
           
        }

        private void RandomColors()
        {
            List<FactionData> factions = Factions.Instance.factions;
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
                Sprite2D countyOwned = (Sprite2D)Globals.Instance.countyParent.GetChild(i);
                countyOwned.SelfModulate = factions[i].factionColor;
            }
        }
    }
}

/*
           // Go through each county, assign their Sprite Renderer, their color and their Build Improvements script.
           foreach (KeyValuePair<string, County> item in WorldMapLoad.Instance.counties)
           {
               //Debug.Log("Random Color Faction: " + item.Key + "   " + item.Value);
               var county = WorldMapLoad.Instance.counties[item.Key];
               county.spriteRenderer =
                   CountyListCreator.Instance.countiesList[county.countyID].gameObject.GetComponent<SpriteRenderer>();
               county.buildImprovements
                   = CountyListCreator.Instance.countiesList[county.countyID].gameObject.GetComponent<BuildImprovements>();
               //Debug.Log(county.gameObject.name + " building improvements: " + county.buildImprovements);

               county.spriteRenderer.color = county.faction.factionNameAndColor.color32;
           }

           // Assign the faction's factionID
           for (int i = 0; i < factions.Count; i++)
           {
               factions[i].factionID = i;
               //Debug.Log("Faction ID: " + factionNameAndColors[i].factionID);
           }
           */

