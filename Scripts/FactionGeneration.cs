using Godot;
using System;

namespace PlayerSpace
{
    public partial class FactionGeneration : Node
    {
        public static FactionGeneration Instance { get; private set; }

        private string factionDataPath = "res://Resources/Factions/";
        [Export] private PackedScene factionNodePackedScene;
        [Export] private CountyImprovementData[] countyImprovementData;
        [Export] private Node factions;

        public override void _Ready()
        {
            Instance = this;

            GetFactionsFromDisk();
            AddFactionsToDiplomacyWar();
        }

        private static void AddFactionsToDiplomacyWar()
        {
            foreach (FactionData factionData in Globals.Instance.factionDatas)
            {
                //GD.Print("Faction Name: " + factionData.factionName);
                foreach (FactionData warFactionData in Globals.Instance.factionDatas)
                {
                    // Add warFactionData to factionWarDictionary with a default value of false
                    factionData.factionWarDictionary[warFactionData.factionName] = false;
                }
            }
        }

        private void GetFactionsFromDisk()
        {
            DirAccess directory = DirAccess.Open("res://");//(factionDataPath);
            if (directory.DirExists("res://Resources/Factions/")) //(factionDataPath))
            {
                directory = DirAccess.Open("res://Resources/Factions/");
                GD.Print("Faction Resource Directory Found.");
                directory.ListDirBegin();
                string[] fileNames = directory.GetFiles();
                for (int i = 0; i < fileNames.Length; i++)
                {
                    GD.Print("Files in Faction Resources: " + fileNames[i]);
                    FactionData newFactionData
                        = (FactionData)ResourceLoader.Load<FactionData>(factionDataPath + fileNames[i]).Duplicate();
                    Globals.Instance.factionDatas.Add(newFactionData);
                    newFactionData.factionID = i;

                    if (Globals.Instance.factionDatas[i].isPlayer == true)
                    {
                        Globals.Instance.playerFactionData = newFactionData;
                    }
                    AddStartingResearch(newFactionData);
                    CreateFactionNode(newFactionData);
                }
            }
            else
            {
                GD.Print("You are so fucked.  This directory doesn't exist: " + factionDataPath);
            }
        }

        private void CreateFactionNode(FactionData newFactionData)
        {
            Faction faction = (Faction)factionNodePackedScene.Instantiate();
            faction.factionData = newFactionData;
            faction.Name = faction.factionData.factionName;
            factions.AddChild(faction);
        }

        private static void AddStartingResearch(FactionData factionData)
        {
            foreach (ResearchItemData researchItemData in AllResearch.Instance.allTierOneResearchData)
            {
                if (researchItemData.researchedAtStart == true)
                {
                    // We need to add some randomness to the starting factions starting research, except
                    // for the player factions.
                    researchItemData.AmountOfResearchDone = researchItemData.costOfResearch;
                }
                factionData.researchItems.Add((ResearchItemData)researchItemData.Duplicate());
            }
        }
    }
}