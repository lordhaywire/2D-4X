using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace
{
    public partial class ResearchControl : Control
    {
        public static ResearchControl Instance { get; private set; }

        [Export] PackedScene uIResearchItemButton;

        [Export] public Label assignedResearchersTitleLabel;
        [Export] public VBoxContainer researchItemParent;
        [Export] private PackedScene assignedResearchersButton;
        [Export] private GridContainer assignedResearchersParent;

        public ResearchItemData researchItemData;

        public event Action ResearchVisible;

        public List<CountyPopulation> assignedResearchers = [];

        private void OnVisibilityChange()
        {
            if (Visible)
            {
                PlayerControls.Instance.AdjustPlayerControls(false);
                Clock.Instance.PauseTime();
                ResearchVisible?.Invoke();

                GenerateAssignedResearchers();
                CheckForResearchers();
            }
            else
            {
                PlayerControls.Instance.AdjustPlayerControls(true);
                Clock.Instance.UnpauseTime();
            }
        }

        public void CheckForResearchers()
        {
            //GD.PrintRich($"[rainbow]Assigned Researchers Count: " + assignedResearchers.Count);
            if (assignedResearchers.Count == 0)
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
                GD.Print("Generate Assigned Researchers: " + countyPopulation.firstName);
                AssignedResearcherHboxContainer researcherButton = (AssignedResearcherHboxContainer)assignedResearchersButton.Instantiate();
                researcherButton.assignedResearcherButton.Text
                    = $"{countyPopulation.firstName} {countyPopulation.lastName}: {countyPopulation.CurrentResearchItemData.researchName}";
                researcherButton.countyPopulation = countyPopulation;
                assignedResearchersParent.AddChild(researcherButton);
            }
        }

        private void ClearResearchers()
        {
            foreach (AssignedResearcherHboxContainer researcher in
                assignedResearchersParent.GetChildren().Cast<AssignedResearcherHboxContainer>())
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
            AddPlayerResearchToUI();
        }

        private void AddPlayerResearchToUI()
        {
            for (int i = 0; i < researchItemParent.GetChildCount(); i++)
            {
                ResearchItemButton researchItemButton = (ResearchItemButton)researchItemParent.GetChild(i);
                researchItemButton.researchItemData = Globals.Instance.playerFactionData.researchItems[i];
            }
        }
    }
}
