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
        [Export] private ProgressBar researchProgressBar;
        [Export] private Label costOfResearchLabel;

        [Export] private HBoxContainer countyImprovementsInResearchParent;
        [Export] private PackedScene countyImprovementResearchPackedScene;
        [Export] private MenuButton assignResearcherMenuButton;

        public ResearchItemData researchItemData;
        private readonly List<CountyPopulation> assignableResearchers = [];
        public override void _Ready()
        {
            Instance = this;
        }

        private void SelectResearcher(long id)
        {
            //GD.Print("Assigned Researcher ID: " + id);
            //GD.Print($"Ass Researcher: {assignableResearchers[(int)id].firstName} {assignableResearchers[(int)id].lastName}");
            ResearchControl.Instance.assignedResearchers.Add(assignableResearchers[(int)id]);
            assignableResearchers[(int)id].UpdateCurrentResearch(researchItemData);
            //GD.Print("Assigned Researcher in Select Reseacher Count: " + ResearchControl.Instance.assignedResearchers.Count);
            ResearchControl.Instance.GenerateAssignedResearchers();
            if (CountyInfoControl.Instance.Visible == true)
            {
                CountyInfoControl.Instance.GenerateHeroesPanelList();
            }
            EventLog.Instance.AddLog($"{assignableResearchers[(int)id].firstName} {assignableResearchers[(int)id].lastName}" +
                $" is now researching {researchItemData.researchName}");
            AssignResearcherMenuButton(); // This clears the list.
        }

        private void AssignResearcherMenuButton()
        {
            if (researchItemData.CheckIfResearchDone() == true)
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
                    if (countyPopulation.currentResearchItemData == null && countyPopulation.activity
                        != AllEnums.Activities.Move)
                    {
                        AddResearcherToMenu(countyData, countyPopulation);
                    }
                }
                // Go through every county data that is built and if it produces research then add it
                // to the assignable researcher list.
                foreach (CountyImprovementData countyImprovementData in countyData.completedCountyImprovements)
                {
                    if (countyImprovementData.factionResourceType == AllEnums.FactionResourceType.Research)
                    {
                        foreach (CountyPopulation countyPopulation in countyImprovementData.countyPopulationAtImprovement)
                        {
                            if (countyPopulation.activity != AllEnums.Activities.Research)
                            {
                                AddResearcherToMenu(countyData, countyPopulation);
                            }
                        }

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

        private void AddResearcherToMenu(CountyData countyData, CountyPopulation countyPopulation)
        {
            assignResearcherMenuButton.GetPopup().AddItem($"{countyPopulation.firstName} {countyPopulation.lastName}" +
                $" - {countyData.countyName}");
            assignableResearchers.Add(countyPopulation);
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
                AssignCountyImprovements();
            }
            else
            {
                assignResearcherMenuButton.GetPopup().IdPressed -= SelectResearcher;
            }
        }

        private void AssignCountyImprovements()
        {
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

        private void UpdateDescriptionLabels()
        {
            researchName.Text = researchItemData.researchName;
            researchTextureRect.Texture = researchItemData.researchTexture;
            researchDescription.Text = researchItemData.researchDescription;
            costOfResearchLabel.Text = $"{researchItemData.AmountOfResearchDone} / {researchItemData.costOfResearch}";
            researchProgressBar.MaxValue = researchItemData.costOfResearch;
            researchProgressBar.Value = researchItemData.AmountOfResearchDone;
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
