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

        public override void _Ready()
        {
            Instance = this;
            AddPlayerResearchToUI();
        }
        private void OnVisibilityChange()
        {
            if (Visible)
            {
                PlayerControls.Instance.AdjustPlayerControls(false);
                Clock.Instance.PauseTime();
                ResearchVisible?.Invoke();

                GenerateAssignedResearchers();
                CheckForResearchers();
                // For testing only
                /*
                Globals.Instance.playerFactionData.factionLeader.attributes[AllEnums.Attributes.Intelligence].attributeLevel++;
                */
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
            ClearResearcherHBoxContainers();
            foreach (CountyPopulation countyPopulation in assignedResearchers)
            {
                //GD.Print("Generate Assigned Researchers: " + countyPopulation.firstName);
                GD.Print($"{countyPopulation.CurrentResearchItemData.researchName}");
                AssignedResearcherHboxContainer researcherButton = (AssignedResearcherHboxContainer)assignedResearchersButton.Instantiate();
                researcherButton.assignedResearcherButton.Text
                    = $"{countyPopulation.firstName} {countyPopulation.lastName}: {countyPopulation.CurrentResearchItemData.researchName}";
                researcherButton.countyPopulation = countyPopulation;
                // If the county population is working at an research office, then their button is disabled, so they can't be
                // removed from the research.
                if(researcherButton.countyPopulation.isHero == false)
                {
                    researcherButton.assignedResearcherCheckbox.Disabled = true;
                }
                assignedResearchersParent.AddChild(researcherButton);
            }
        }

        private void ClearResearcherHBoxContainers()
        {
            foreach (AssignedResearcherHboxContainer assignedResearcherHboxContainer in
                assignedResearchersParent.GetChildren().Cast<AssignedResearcherHboxContainer>())
            {
                assignedResearcherHboxContainer.QueueFree();
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
