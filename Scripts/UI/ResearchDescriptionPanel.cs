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
        [Export] private Label researchPrerequisitesTitleLabel;
        [Export] private PackedScene researchPrerequisiteLabelPackedScene;
        [Export] private VBoxContainer prerequisitesParent;
        [Export] private Label costOfResearchLabel;

        [Export] private VBoxContainer countyImprovementsInResearchParent;
        [Export] private PackedScene countyImprovementResearchPackedScene;
        [Export] private MenuButton assignResearcherMenuButton;

        public ResearchItemData researchItemData;
        private readonly List<CountyPopulation> assignableResearchers = [];
        public override void _Ready()
        {
            Instance = this;
        }
        private void OnResearchDescriptionPanelVisibilityChanged()
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
                $" {Tr("PHRASE_IS_NOW_RESEARCHING")} {Tr(researchItemData.researchName)}");
            //populationListTitle.Text = $"{Globals.Instance.SelectedLeftClickCounty.countyData.countyName} {Tr("WORD_POPULATION")}";

            AssignResearcherMenuButton(); // This clears the list.
        }

        private void AssignResearcherMenuButton()
        {
            if (researchItemData.CheckIfResearchDone() == true || researchItemData.CheckIfPrerequisitesAreDone() == false)
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
                        foreach (CountyPopulation countyPopulation in countyImprovementData.populationAtImprovement)
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



        private void AssignCountyImprovements()
        {
            if (researchItemData.countyImprovementDatas.Length > 0)
            {
                foreach (CountyImprovementData countyImprovementData in researchItemData.countyImprovementDatas)
                {
                    CountryImprovementPanelContainer countyImprovementInResearchControl
                        = (CountryImprovementPanelContainer)countyImprovementResearchPackedScene.Instantiate();
                    countyImprovementInResearchControl.countyImprovementData = countyImprovementData;
                    countyImprovementInResearchControl.countyImprovementData
                        .SetCountyImprovementStatus(AllEnums.CountyImprovementStatus.InResearchPanel);
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
            researchPrerequisitesTitleLabel.Show();
            // This whole thing is almost a duplicate of the code in ResearchItemButton.
            ClearPrerequisites();
            if (researchItemData.researchPrerequisites.Count > 0)
            {
                //GD.Print($"{researchItemData.researchName} is not null");
                UpdatePrerequisites();
            }
            else
            {
                researchPrerequisitesTitleLabel.Hide();
            }
        }

        private void ClearPrerequisites()
        {
            foreach(Label label in prerequisitesParent.GetChildren().Cast<Label>())
            {
                label.QueueFree();
            }
        }

        private void UpdatePrerequisites()
        {
            foreach (EnumsResearch.All enumResearch in researchItemData.researchPrerequisites)
            {
                Label researchPrerequisiteLabel = (Label)researchPrerequisiteLabelPackedScene.Instantiate();
                researchPrerequisiteLabel.Text 
                    = Globals.Instance.playerFactionData.researchItems[(int)enumResearch].researchName;
                prerequisitesParent.AddChild(researchPrerequisiteLabel);
            }
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
