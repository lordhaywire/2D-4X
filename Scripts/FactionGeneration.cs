using Godot;
using System;
using System.Linq;

namespace PlayerSpace
{
    public partial class FactionGeneration : Node
    {
        public static FactionGeneration Instance { get; private set; }

        private string factionDataPath = "res://Resources/Factions/";
        [Export] private PackedScene factionNodePackedScene;
        [Export] private CountyImprovementData[] countyImprovementData;

        public override void _Ready()
        {
            Instance = this;

            CreateFactionsFromDisk();
        }

        private void CreateFactionsFromDisk()
        {
            DirAccess directory = DirAccess.Open("res://");//(factionDataPath);
            if (directory.DirExists("res://Resources/Factions/")) //(factionDataPath))
            {
                directory = DirAccess.Open("res://Resources/Factions/");
                //GD.Print("Faction Resource Directory Found.");
                directory.ListDirBegin();
                string[] fileNames = directory.GetFiles();
                for (int i = 0; i < fileNames.Length; i++)
                {
                    //GD.Print("Files in Faction Resources: " + fileNames[i]);
                    FactionData newFactionData
                        = (FactionData)ResourceLoader.Load<FactionData>(factionDataPath + fileNames[i]).Duplicate();
                    Globals.Instance.factionDatas.Add(newFactionData); // We should probably get rid of this.  We already
                    // have it in the FactioNode children.
                    newFactionData.factionID = i;

                    if (Globals.Instance.factionDatas[i].isPlayer == true)
                    {
                        Globals.Instance.playerFactionData = newFactionData;
                    }
                    //GD.Print($"{newFactionData.factionName} has been loaded from disk.");
                    // The order is important.
                    CreateFactionNode(newFactionData);
                    AddStartingResearch(newFactionData);
                    CreateFactionGoodDictionary(newFactionData);
                    AddFactionsToDiplomacyWar(newFactionData);
                }
                /*
                foreach (Faction faction in Globals.Instance.factionsParent.GetChildren().Cast<Faction>())
                {
                    foreach (ResearchItemData researchItem in faction.factionData.researchItems)
                    {
                        GD.Print("Research Item Data Faction ID: " + researchItem.factionID);
                    }
                }
                */
            }
            else
            {
                //GD.Print("You are so fucked.  This directory doesn't exist: " + factionDataPath);
            }
        }

        private static void AddStartingResearch(FactionData factionData)
        {
            foreach (ResearchItemData researchItemData in AllResearch.Instance.allResearchItemDatas)
            {
                //GD.Print("Faction ID that is getting assigned: " + factionData.factionID);
                researchItemData.factionID = factionData.factionID;
                //GD.PrintRich($"[rainbow]{FactionData.GetFactionDataFromID(researchItemData.factionID).factionName}: {researchItemData.researchName}");

                ResearchItemData researchItemDataCopy = researchItemData.NewCopy(researchItemData); //(ResearchItemData)researchItemData.Duplicate(true); //
                if (researchItemDataCopy.researchedAtStart == true)
                {
                    // We need to add some randomness to the starting factions starting research, except
                    // for the player factions.
                    researchItemDataCopy.AmountOfResearchDone = researchItemDataCopy.costOfResearch;
                }
                factionData.researchItems.Add(researchItemDataCopy);

                /* This is for testing.
                if (factionData.researchItems.Count > 0)
                {
                    GD.Print($"Faction Data Research Items Count: {factionData.researchItems.Count}");

                    GD.Print($"Test of research item faction ID: {factionData.researchItems[0].factionID}");
                }
                */
            }
        }


        private static void CreateFactionGoodDictionary(FactionData factionData)
        {
            foreach (GoodData goodData in AllGoods.Instance.allGoods)
            {
                if (goodData.goodType == AllEnums.GoodType.CountyGood)
                {
                    continue;
                }
                //GD.Print($"{goodData.goodName} has been added to {factionData.factionName}");
                factionData.factionGood.Add(goodData.factionGoodType, (GoodData)goodData.Duplicate());
                factionData.yesterdaysFactionGoods.Add(goodData.factionGoodType, (GoodData)goodData.Duplicate());
                factionData.amountUsedFactionGoods.Add(goodData.factionGoodType, (GoodData)goodData.Duplicate());
            }
            // This is for testing.  We are going to have to have a different, more random way of
            // generating starting resources for each faction.
            factionData.factionGood[AllEnums.FactionGoodType.Influence].Amount = 1500;
            factionData.factionGood[AllEnums.FactionGoodType.Money].Amount = 1500;
            //GD.Print("Faction Influence: "+ factionData.factionGood[AllEnums.FactionGoodType.Influence].Amount);
        }

        private static void AddFactionsToDiplomacyWar(FactionData factionData)
        {
            //GD.Print("Faction Name: " + factionData.factionName);
            foreach (FactionData warFactionData in Globals.Instance.factionDatas)
            {
                // Add warFactionData to factionWarDictionary with a default value of false
                factionData.factionWarDictionary[warFactionData.factionName] = false;
            }
        }

        private void CreateFactionNode(FactionData newFactionData)
        {
            Faction faction = (Faction)factionNodePackedScene.Instantiate();
            faction.factionData = newFactionData;
            faction.Name = faction.factionData.factionName;
            Globals.Instance.factionsParent.AddChild(faction);
        }
    }
}