using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class Factions : Node
    {
        public static Factions Instance { get; private set; }
        private string factionDataPath = "res://Resources/Factions/";
        public List<FactionData> factions = new();

        public override void _Ready()
        {
            Instance = this;

            using var directory = DirAccess.Open(factionDataPath);
            if (directory.DirExists(factionDataPath))
            {
                directory.ListDirBegin();
                string[] fileNames = directory.GetFiles();
                //GD.Print()
                for (int i = 0; i < fileNames.Length; i++)
                {
                    var factionData = ResourceLoader.Load<FactionData>(factionDataPath + fileNames[i]);
                    factions.Add(factionData);
                    GD.Print($"Player? {factions[i].isPlayer} and Faction Name? {factions[i].factionName}");
                }
            }
            else
            {
                GD.Print("You are so fucked.  This directory doesn't exist: " + factionDataPath);
            }
        }
    }
}