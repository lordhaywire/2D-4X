using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace
{
    public partial class ResearchControl : Control
    {
        public static ResearchControl Instance { get; private set; }

        //string uIResearchItemButtonPath = "res://UIScenes/UIResearchItemButton.tscn";

        [Export] public Label assignedResearchersTitleLabel;
        [Export] public VBoxContainer researchItemParent;
        [Export] private PackedScene assignedResearchersButton;
        [Export] private GridContainer assignedResearchersParent;
        //[Export] public PanelContainer researchDescriptionPanel;

        public ResearchItemData researchItemData;

        public event Action ResearchVisible;

        public List<CountyPopulation> assignedResearchers = [];

        private void OnVisibilityChange()
        {
            if (Visible == true)
            {
                //CreateResearchItemButtons();
                PlayerControls.Instance.AdjustPlayerControls(false);
                Clock.Instance.PauseTime();
                ResearchVisible?.Invoke();
                GenerateAssignedResearchers();
                CheckForResearchers();
            }
            else
            {
                //DestroyResearchItemButtons();
                PlayerControls.Instance.AdjustPlayerControls(true);
                Clock.Instance.UnpauseTime();
            }
        }

        public void CheckForResearchers()
        {
            if(assignedResearchers.Count == 0)
            {
                assignedResearchersTitleLabel.Hide();
            }
            else
            {
                assignedResearchersTitleLabel.Show();
            }
        }

        public void GenerateAssignedResearchers()
        {
            ClearResearchers();
            foreach (CountyPopulation countyPopulation in assignedResearchers)
            {
                Button researcherButton = (Button)assignedResearchersButton.Instantiate();
                researcherButton.Text = $"{countyPopulation.firstName} {countyPopulation.lastName}: " +
                        $"{countyPopulation.currentResearchItemData.researchName}";
                assignedResearchersParent.AddChild(researcherButton);
            }
        }

        private void ClearResearchers()
        {
            foreach(Button researcher in assignedResearchersParent.GetChildren().Cast<Button>())
            {
                researcher.QueueFree();
            }
        }

        public void ShowResearchPanel()
        {
            //GD.Print("Show the research panel!");
            Show();
            PlayerControls.Instance.AdjustPlayerControls(false);
        }

        public void CloseButton()
        {
            if (ResearchDescriptionPanel.Instance.Visible == true)
            {
                ResearchDescriptionPanel.Instance.Hide();
                researchItemParent.Show();
            }
            else
            {
                Hide();
                PlayerControls.Instance.AdjustPlayerControls(true);
            }
        }
        public override void _Ready()
        {
            Instance = this;
        }

        /*
        private void CreateResearchItemButtons()
        {
            List<ResearchItemData> researchItems = Globals.Instance.playerFactionData.researchItems;
            for (int i = 0; i < researchItems.Count; i++)
            {
                PackedScene researchItemScene = (PackedScene)GD.Load(uIResearchItemButtonPath);
                PanelContainer researchItem = (PanelContainer)researchItemScene.Instantiate();
                researchItem.Name = i.ToString();
                researchItemParent.AddChild(researchItem);
                researchItem.GetNode<Button>("Button").Text = researchItems[i].researchName;
                if (researchItems[i].isResearchDone == true)
                {
                    researchItem.GetNode<CheckBox>("CheckBox").ButtonPressed = true;
                }
            }
            //GD.Print("Research Parent Count: " + researchItemParent.GetChildCount());
        }
        */
        /*
        private void DestroyResearchItemButtons()
        {
            foreach (Node researchItem in researchItemParent.GetChildren())
            {
                researchItem.QueueFree();
            }
        }
        */
    }
}
