using Godot;

namespace PlayerSpace
{
    public partial class FactionGeneration : Node
    {
        private string factionDataPath = "res://Resources/Factions/";
        [Export] private PackedScene factionNodePackedScene;
        

        [Export] private CountyImprovementData[] countyImprovementData;

        public override void _Ready()
        {
            //Instance = this;

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
                    
                    Globals.Instance.factions.Add(factionData);
                    //GD.Print($"Player? {Globals.Instance.factions[i].isPlayer} and Faction Name? {Globals.Instance.factions[i].factionName}");
                    if (Globals.Instance.factions[i].isPlayer == true)
                    {
                        Globals.Instance.playerFactionData = factionData;
                        //GD.Print("Player Faction: " + Globals.Instance.playerFactionData.factionName);
                    }

                    AddStartingResearch(factionData);
                    CreateFactionNode(factionData);
                }
            }
            else
            {
                GD.Print("You are so fucked.  This directory doesn't exist: " + factionDataPath);
            }
        }

        private void CreateFactionNode(FactionData factionData)
        {
            FactionNode factionNode = (FactionNode)factionNodePackedScene.Instantiate();
            factionNode.factionData = factionData;
            factionNode.Name = factionNode.factionData.factionName;
            AddChild(factionNode);
        }

        private void AddStartingResearch(FactionData factionData)
        {
            // Let's turn this into an array or some shit at some point so we don't have manually add everything, we could just do a foreach loop.
            factionData.researchItems.Add(new ResearchItemData(AllText.BuildingName.FISHERSSHACK, AllText.Descriptions.FISHERSSHACK, countyImprovementData[0], true));
            //GD.Print("Add Starting Research: " + factionData.researchItems[0].researchName);
            factionData.researchItems.Add(new ResearchItemData(AllText.BuildingName.FORESTERSSHACK, AllText.Descriptions.FORESTERSSHACK, countyImprovementData[1], true));
            //GD.Print("Add Starting Research: " + factionData.researchItems[1].researchName);
            factionData.researchItems.Add(new ResearchItemData(AllText.BuildingName.GARDENERSSHACK, AllText.Descriptions.GARDENERSSHACK, countyImprovementData[2], true));
            //GD.Print("Add Starting Research: " + factionData.researchItems[2].researchName);
        }
    }
}