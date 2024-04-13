using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace
{
    public partial class ResearchDescriptionPanel : PanelContainer
    {
        public static ResearchDescriptionPanel Instance { get; private set; }

        [Export] private Label researchName;
        [Export] private TextureRect researchTextureRect;
        [Export] private Label researchDescription;
        [Export] private Label amountOfResearchDoneLabel;
        [Export] private Label costOfResearchLabel;
        [Export] private HBoxContainer countyImprovementsInResearchParent;
        [Export] private PackedScene countyImprovementResearchPackedScene;
        [Export] private MenuButton assignResearcherMenuButton;

        public ResearchItemData researchItemData;
        private List<CountyPopulation> assignableResearchers = [];
        public override void _Ready()
        {
            Instance = this;
        }

        private void SelectResearcher(long id)
        {
            GD.Print("Assigned Researcher ID: " + id);
            GD.Print($"Ass Researcher: {assignableResearchers[(int)id].firstName} {assignableResearchers[(int)id].lastName}");
            assignableResearchers[(int)id].currentResearchItemData = researchItemData;
            assignableResearchers[(int)id].currentActivity = AllText.Activities.RESEARCHING;
            ResearchControl.Instance.assignedResearchers.Add(assignableResearchers[(int)id]);
            ResearchControl.Instance.GenerateAssignedResearchers();
            ResearchControl.Instance.CheckForResearchers();
            CountyInfoControl.Instance.GenerateHeroesPanelList();
            EventLog.Instance.AddLog($"{assignableResearchers[(int)id].firstName} {assignableResearchers[(int)id].lastName}" +
                $" is now researching {researchItemData.researchName}");
            AssignResearcherMenuButton(); // This clears the list.
        }

        private void AssignResearcherMenuButton()
        {
            if (researchItemData.isResearchDone == true)
            {
                assignResearcherMenuButton.Hide();
                return;
            }
            assignResearcherMenuButton.Show();
            assignResearcherMenuButton.GetPopup().Clear();
            assignableResearchers.Clear();
            foreach (CountyData countyData in Globals.Instance.playerFactionData.countiesFactionOwns)
            {
                foreach (CountyPopulation countyPopulation in countyData.herosInCountyList)
                {
                    if (countyPopulation.currentResearchItemData == null)
                    {
                        assignResearcherMenuButton.GetPopup().AddItem($"{countyPopulation.firstName} " +
                            $"{countyPopulation.lastName}: {countyPopulation.currentActivity}");
                        assignableResearchers.Add(countyPopulation);
                    }
                }
            }
            if (assignableResearchers.Count == 0)
            {
                assignResearcherMenuButton.Hide();
            }
            else
            {
                assignResearcherMenuButton.Show();
            }

        }

        private void OnVisibilityChanged()
        {
            if (Visible == true)
            {
                //GD.Print("Research Description Panel!");
                assignResearcherMenuButton.GetPopup().IdPressed += SelectResearcher;
                RemoveCountyImprovements();
                UpdateDescriptionLabels();
                AssignResearcherMenuButton();
                if (researchItemData.countyImprovementDatas.Length > 0)
                {
                    foreach (CountyImprovementData countyImprovementData in researchItemData.countyImprovementDatas)
                    {
                        CountryImprovementDescriptionButton countyImprovementInResearchControl
                            = (CountryImprovementDescriptionButton)countyImprovementResearchPackedScene.Instantiate();
                        countyImprovementInResearchControl.countyImprovementData = countyImprovementData;
                        countyImprovementsInResearchParent.AddChild(countyImprovementInResearchControl);
                    }
                }
            }
            else
            {
                assignResearcherMenuButton.GetPopup().IdPressed -= SelectResearcher;
            }
        }

        private void UpdateDescriptionLabels()
        {
            researchName.Text = researchItemData.researchName;
            researchTextureRect.Texture = researchItemData.researchTexture;
            researchDescription.Text = researchItemData.researchDescription;
            amountOfResearchDoneLabel.Text = researchItemData.AmountOfResearchDone.ToString();
            costOfResearchLabel.Text = researchItemData.costOfResearch.ToString();
        }

        private void RemoveCountyImprovements()
        {
            foreach (Control control in countyImprovementsInResearchParent.GetChildren().Cast<Control>())
            {
                control.QueueFree();
            }
        }
    }
}
