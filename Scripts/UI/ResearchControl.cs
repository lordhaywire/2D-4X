using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class ResearchControl : Control
    {
        public static ResearchControl Instance { get; private set; }
        public readonly List<ResearchItem> researchItemsList = new();

        string uIResearchItemButtonPath = "res://UIScenes/UIResearchItemButton.tscn";

        [Export] public VBoxContainer researchItemParent;
        [Export] public PanelContainer researchDescriptionPanel;

        public void ShowResearchPanel()
        {
            //GD.Print("Show the research panel!");
            Show();
            Globals.Instance.playerControlsEnabled = false;
        }

        public void CloseButton()
        {
            if(researchDescriptionPanel.Visible == true)
            {
                researchDescriptionPanel.Hide();
                researchItemParent.Show();
            }
            else
            {
                Hide();
                Globals.Instance.playerControlsEnabled = true;
            }  
        }
        public override void _Ready()
        {
            Instance = this;

            researchItemsList.Add(new ResearchItem(AllText.BuildingName.FISHERSSHACK, AllText.Descriptions.FISHERSSHACK, true, true));
            researchItemsList.Add(new ResearchItem(AllText.BuildingName.FORESTERSSHACK, AllText.Descriptions.FORESTERSSHACK, true, true));
            researchItemsList.Add(new ResearchItem(AllText.BuildingName.GARDENERSSHACK, AllText.Descriptions.GARDENERSSHACK, true, true));
        }

        private void CreateResearchItemButtons()
        {
            for (int i = 0; i < researchItemsList.Count; i++)
            {
                PackedScene researchItemScene = (PackedScene)GD.Load(uIResearchItemButtonPath);
                PanelContainer researchItem = (PanelContainer)researchItemScene.Instantiate();
                researchItem.Name = i.ToString();
                researchItemParent.AddChild(researchItem);
                researchItem.GetNode<Button>("Button").Text = researchItemsList[i].researchName;
                if (researchItemsList[i].isResearchDone == true)
                {
                    researchItem.GetNode<CheckBox>("CheckBox").ButtonPressed = true;
                }
            }
            GD.Print("Research Parent Count: " + researchItemParent.GetChildCount());

        }

        private void DestroyResearchItemButtons()
        {
            foreach (Node researchItem in researchItemParent.GetChildren())
            {
                researchItem.QueueFree();
            }
        }
        private void OnVisibilityChange()
        {
            if (Visible == true)
            {
                CreateResearchItemButtons();
                Globals.Instance.playerControlsEnabled = false;
                Clock.Instance.numberOfControlsShown++;
                Clock.Instance.PauseTime();
            }
            else
            {
                DestroyResearchItemButtons();
                Globals.Instance.playerControlsEnabled = true;
                Clock.Instance.numberOfControlsShown--;
                Clock.Instance.UnpauseTime();
            }
        }


    }
}
// Reading a file from the project folders.
/*

//private string researchItemsPath = "res://ResearchItems/ResearchItems.txt"; // For reading from Json or Text

using var file = FileAccess.Open(researchItemsPath, FileAccess.ModeFlags.Read);
string content = file.GetAsText();
GD.PrintRich("Conmtent: " + content);


// Writing it, partial.
if(FileAccess.FileExists(researchItemsPath) == true)
{
    using var file = FileAccess.Open(researchItemsPath, FileAccess.ModeFlags.Write);
    var testJson = Json
    file.StoreVar(researchItems, true);
    //GD.PrintRich("[rainbow]Whatevs");
}
*/
