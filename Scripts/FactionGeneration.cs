using AutoloadSpace;
using Godot;

namespace PlayerSpace;

public partial class FactionGeneration : Node
{
    //public static FactionGeneration Instance { get; private set; }

    private string factionDataPath = "res://Resources/Factions/";
    [Export] private PackedScene factionNodePackedScene;
    [Export] private CountyImprovementData[] countyImprovementData;
    private FactionData factionData;

    public override void _Ready()
    {
        //Instance = this;

        CreateFactionsFromDisk();
    }

    private void CreateFactionsFromDisk()
    {
        factionData = null;
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
                factionData
                    = (FactionData)ResourceLoader.Load<FactionData>(factionDataPath + fileNames[i]).Duplicate();
                Globals.Instance.allFactionData.Add(factionData); // We should probably get rid of this.  We already
                // have it in the FactionNode children.
                factionData.factionId = i;

                if (Globals.Instance.allFactionData[i].isPlayer)
                {
                    Globals.Instance.playerFactionData = factionData;
                }
                GD.Print($"{factionData.factionName} has been loaded from disk.");
                // The order is important.
                CreateFactionNode(factionData);
                CreateFactionGoodDictionary(factionData);
                AddFactionsToDiplomacyWar(factionData);
                AddStartingResearch();
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
            //GD.Print("You are so fucked. This directory doesn't exist: " + factionDataPath);
        }
    }

    private void AddStartingResearch()
    {
        foreach (ResearchItemData researchItemData in Autoload.Instance.allResearchItemDatas)
        {
            //GD.Print("Faction ID that is getting assigned: " + factionData.factionID);
            researchItemData.factionId = factionData.factionId;
            //GD.PrintRich($"[rainbow]{FactionData.GetFactionDataFromID(researchItemData.factionID).factionName}: {researchItemData.researchName}");

            ResearchItemData researchItemDataCopy = researchItemData.NewCopy(researchItemData); //(ResearchItemData)researchItemData.Duplicate(true); //
            if (researchItemDataCopy.researchedAtStart)
            {
                // We need to add some randomness to the starting factions starting research, except
                // for the player factions.
                researchItemDataCopy.AmountOfResearchDone = researchItemDataCopy.costOfResearch;
            }
            factionData.researchItems.Add(researchItemDataCopy);

            /*
            // This is for testing.
            if (factionData.researchItems.Count > 0)
            {
                GD.Print($"Faction Data Research Items Count: {factionData.researchItems.Count}");

                GD.Print($"Test of research item faction ID: {factionData.researchItems[0].factionID}");
            }
            */  
        }

        foreach(ResearchItemData researchItem in factionData.researchItems)
        {
            if (researchItem.CheckIfResearchDone())
            {
                researchItem.CompleteResearch();
            }
        }
    }

    private static void CreateFactionGoodDictionary(FactionData factionData)
    {
        foreach (GoodData goodData in Autoload.Instance.allGoods)
        {
            if (goodData.goodType == AllEnums.GoodType.CountyGood)
            {
                continue;
            }
            //GD.Print($"{goodData.goodName} has been added to {factionData.factionName}");
            factionData.factionGoods.Add(goodData.factionGoodType, (GoodData)goodData.Duplicate());
            factionData.yesterdaysFactionGoods.Add(goodData.factionGoodType, (GoodData)goodData.Duplicate());
            factionData.amountUsedFactionGoods.Add(goodData.factionGoodType, (GoodData)goodData.Duplicate());
        }
        // This is for testing.  We are going to have a different, more random way of
        // generating starting resources for each faction.
        factionData.factionGoods[AllEnums.FactionGoodType.Influence].Amount = 1500;
        factionData.factionGoods[AllEnums.FactionGoodType.Money].Amount = 1500;
        //GD.Print("Faction Influence: "+ factionData.factionGood[AllEnums.FactionGoodType.Influence].Amount);
    }

    private static void AddFactionsToDiplomacyWar(FactionData factionData)
    {
        //GD.Print("Faction Name: " + factionData.factionName);
        foreach (FactionData warFactionData in Globals.Instance.allFactionData)
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