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
        Godot.Collections.Array<PopulationData> availableResearchers = [];
        Godot.Collections.Array<CountyImprovementData> availableOffices = [];
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
                assignResearcherMenuButton.Hide(); // This is probably temporary.
                RemoveCountyImprovements();
                UpdateDescriptionLabels();
                CheckToShowAssignResearcherButton();
                AssignResearcherMenuButton();
                AssignCountyImprovements();
            }
            else
            {
                assignResearcherMenuButton.GetPopup().IdPressed -= SelectResearcher;
            }
        }

        /// <summary>
        /// This checks to make sure the research is done, all of the prerequisites are done,
        /// there are available offices and available heroes.  
        /// If so then it shows the Assign Researcher Button.
        /// </summary>
        private void CheckToShowAssignResearcherButton()
        {
            availableOffices.Clear();
            availableResearchers.Clear();
            availableOffices
                = Research.GetListOfAvailableResearchOffices(Globals.Instance.playerFactionData);
            availableResearchers 
                = Research.GetListOfAvailableHeroResearchers();

            foreach (CountyImprovementData availableOffice in availableOffices)
            {
                GD.Print($"Available Office: " + availableOffice.improvementName);
            }
            if (researchItemData.CheckIfResearchDone() == false 
                && researchItemData.CheckIfPrerequisitesAreDone() == true 
                && availableOffices.Count > 0 && availableResearchers.Count > 0)
            {
                assignResearcherMenuButton.Show();
            }
        }

        private void AssignResearcherMenuButton()
        {
            assignResearcherMenuButton.GetPopup().Clear();
            foreach (PopulationData populationData in availableResearchers) 
            {
                PopupMenu submenuResearchOffice = new();
                PopupMenu secondSubmenu = new();
                assignResearcherMenuButton.GetPopup().AddChild(submenuResearchOffice);
                
                submenuResearchOffice.AddChild(secondSubmenu);

                assignResearcherMenuButton.GetPopup().AddItem($"{populationData.firstName} {populationData.lastName}");
                assignResearcherMenuButton.GetPopup().AddSubmenuNodeItem("WHatgever", submenuResearchOffice);
                submenuResearchOffice.AddItem("SecondWah");
               
                // AddResearcherToMenu();

            }
                // Go through every county improvement data that is built and if it produces
                // research then add it
                // to the assignable researcher list.
                /*
                foreach (CountyImprovementData countyImprovementData in countyData.completedCountyImprovements)
                {
                    if (countyImprovementData.factionResourceType == AllEnums.FactionGoodType.Research)
                    {
                        foreach (CountyPopulation populationData in countyImprovementData.populationAtImprovement)
                        {
                            if (populationData.activity != AllEnums.Activities.Research)
                            {
                                AddResearcherToMenu(countyData, populationData);
                            }
                        }

                    }
                }
                */
            /*
            if (availableResearchers.Count == 0)
            {
                assignResearcherMenuButton.Hide();
            }
            else
            {
                assignResearcherMenuButton.Show();
            }
            */
        }

        private void SelectResearcher(long id)
        {
            //GD.Print("Assigned Researcher ID: " + id);
            //GD.Print($"Ass Researcher: {assignableResearchers[(int)id].firstName} {assignableResearchers[(int)id].lastName}");
            ResearchControl.Instance.assignedResearchers.Add(availableResearchers[(int)id]);
            availableResearchers[(int)id].UpdateCurrentResearch(researchItemData);
            //GD.Print("Assigned Researcher in Select Reseacher Count: " + ResearchControl.Instance.assignedResearchers.Count);
            ResearchControl.Instance.GenerateAssignedResearchers();
            if (CountyInfoControl.Instance.Visible == true)
            {
                CountyInfoControl.Instance.GenerateHeroesPanelList();
            }
            EventLog.Instance.AddLog($"{availableResearchers[(int)id].firstName} {availableResearchers[(int)id].lastName}" +
                $" {Tr("PHRASE_IS_NOW_RESEARCHING")} {Tr(researchItemData.researchName)}");
            //populationListTitle.Text = $"{Globals.Instance.SelectedLeftClickCounty.countyData.countyName} {Tr("WORD_POPULATION")}";

            AssignResearcherMenuButton(); // This clears the list.
        }

        private void AddResearcherToMenu(CountyData countyData, PopulationData populationData)
        {
            assignResearcherMenuButton.GetPopup().AddItem($"{populationData.firstName} {populationData.lastName}" +
                $" - {countyData.countyName}");
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
