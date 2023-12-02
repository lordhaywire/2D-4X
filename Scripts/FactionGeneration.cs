using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class FactionGeneration : Node
    {
        public static FactionGeneration Instance { get; private set; }

        private string factionDataPath = "res://Resources/Factions/";
        public List<FactionData> factions = new();
        public FactionData playerFaction;

        public override void _Ready()
        {
            Instance = this;

            GetFactionsFromDisk();
        }

        private void GetFactionsFromDisk()
        {
            using var directory = DirAccess.Open(factionDataPath);
            if (directory.DirExists(factionDataPath))
            {
                directory.ListDirBegin();
                string[] fileNames = directory.GetFiles();
                for (int i = 0; i < fileNames.Length; i++)
                {
                    var factionData = ResourceLoader.Load<FactionData>(factionDataPath + fileNames[i]);
                    factions.Add(factionData);
                    GD.Print($"Player? {factions[i].isPlayer} and Faction Name? {factions[i].factionName}");
                    if (factions[i].isPlayer == true)
                    {
                        playerFaction = factionData;
                        GD.Print("Player Faction: " + playerFaction.factionName);
                    }
                }
            }
            else
            {
                GD.Print("You are so fucked.  This directory doesn't exist: " + factionDataPath);
            }
        }
    }
}