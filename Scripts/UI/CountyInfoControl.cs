using Godot;
using System;
using System.Linq;


namespace PlayerSpace
{
    public partial class CountyInfoControl : Control
    {
        public static CountyInfoControl Instance { get; private set; }

        [ExportGroup("County Info")]
        [Export] public Control countyInfoControl;
        [Export] public Label factionNamelabel;
        [Export] public Label countyNameLabel;
        [Export] private Label countyFoodLabel;
        [Export] private Label countyScrapLabel;
        [Export] private Label populationDataNumberLabel;
        [Export] private Label countyIdleWorkersLabel;
        [Export] private Label visitorsLabel;
        private CountyData countyData;

        [ExportGroup("Containers and shit")]
        [Export] public MarginContainer populationListMarginContainer;
        [Export] public Control populationDescriptionControl;
        [Export] public Control countyImprovementsPanelControl;
        [Export] private VBoxContainer heroListParent;
        [Export] private VBoxContainer heroSpawnCheckButtonParent;

        [ExportGroup("Buttons")]
        [Export] private Button populationListButton;
        [Export] private Button visitorsListButton;
        [Export] private Button countyImprovementsButton;
        [Export] private Button resourcesButton;

        [Export] private PackedScene heroListPrefab;

        public override void _Ready()
        {
            Instance = this;
        }

        private void OnVisibilityChanged()
        {
            // Idle workers changes if we change who is building stuff etc.
            if (Visible)
            {
                Globals.Instance.SelectedLeftClickCounty.countyData.CountIdleWorkers();
                //TopBarControl.Instance.UpdateResourceLabels();
            }
            else
            {
                // Update the faction resources, which is all the county resources.
                TopBarControl.Instance.UpdateTopBarGoodLabels();
            }
        }

        public static void OnMouseEntered()
        {
            PlayerControls.Instance.stopClickThrough = true;
        }

        public static void OnMouseExited()
        {
            PlayerControls.Instance.stopClickThrough = false;
        }

        // This update everything needs to be looked at.
        public void UpdateEverything()
        {
            countyData = Globals.Instance.SelectedLeftClickCounty?.countyData;
            if(countyData == null)
            {
                return;
            }
            CheckForOwnership();
            UpdateNameLabels();
            UpdateCountyAvailableResources();
            UpdateCountyPopulationLabel();
            UpdateVisitorsPopulationLabel();
            UpdateIdleWorkersLabel();
            GenerateHeroesPanelList();
        }

        public void UpdateCountyAvailableResources()
        {
            countyFoodLabel.Text = $"Food: {countyData.scavengableCannedFood}";
            countyScrapLabel.Text = $"Remnants: {countyData.scavengableRemnants}";
        }

        private void UpdateVisitorsPopulationLabel()
        {
            visitorsLabel.Text
                = Globals.Instance.SelectedLeftClickCounty.countyData.visitingHeroList.Count.ToString();
            if (Globals.Instance.SelectedLeftClickCounty.countyData.visitingHeroList.Count == 0)
            {
                visitorsListButton.Disabled = true;
            }
            else
            {
                visitorsListButton.Disabled = false;
            }
        }

        public void DisableSpawnHeroCheckButton(bool value)
        {
            foreach (Node node in heroSpawnCheckButtonParent.GetChildren())
            {
                HeroPanelContainer heroPanelContainer = (HeroPanelContainer)node;
                if (heroPanelContainer.populationData.factionData == Globals.Instance.playerFactionData)
                {
                    heroPanelContainer.spawnHeroButton.Disabled = value;
                }
            }
        }

        public void UpdateNameLabels()
        {
            factionNamelabel.Text = Globals.Instance.SelectedLeftClickCounty.countyData.factionData.factionName;
            countyNameLabel.Text = Globals.Instance.SelectedLeftClickCounty.countyData.countyName;
        }
        public void GenerateHeroesPanelList()
        {
            ClearHeroList();
            GenerateHeroes(countyData.heroesInCountyList);
            GenerateHeroes(countyData.armiesInCountyList);
            GenerateHeroes(countyData.visitingHeroList);
            GenerateHeroes(countyData.visitingArmyList);

        }

        private void ClearHeroList()
        {
            if (heroListParent.GetChildCount() > 0)
            {
                foreach (Node hero in heroListParent.GetChildren())
                {
                    hero.QueueFree();
                }
            }
        }

        private void GenerateHeroes(Godot.Collections.Array<PopulationData> heroCountyPopulationList)
        {
            foreach (PopulationData populationData in heroCountyPopulationList)
            {
                HeroPanelContainer heroPrefab = (HeroPanelContainer)heroListPrefab.Instantiate();
                heroListParent.AddChild(heroPrefab);

                heroPrefab.populationData = populationData;

                UpdateHeroInfo(heroPrefab);

                // Change color of panel to the faction color.
                heroPrefab.SelfModulate = populationData.factionData.factionColor;

                CheckForAvailableActivities(heroPrefab);

                // Check to see if the hero is part of the player's faction to determine what to show.
                // Once we add the ability for heroes to do things in enemy faction counties we will change this.
                // Currently we are just making it so that the heroes Activities boxes are hidden.
                CountyData locationCountyData = Globals.Instance.GetCountyDataFromLocationID(populationData.location);

                PopulateActivityHboxes(populationData, heroPrefab);
                if (Globals.Instance.CheckIfPlayerFaction(populationData.factionData) == false
                    || Globals.Instance.CheckIfPlayerFaction(locationCountyData.factionData) == false)
                {
                    heroPrefab.heroListButton.Disabled = true;
                    heroPrefab.spawnHeroButton.Hide();
                    heroPrefab.aideActivitiesHboxContainer.Hide();
                    heroPrefab.armyActivitiesHboxContainer.Hide();
                    continue;
                }

                if (populationData.IsThisAnArmy())
                {
                    heroPrefab.armyActivitiesHboxContainer.Show();
                }
                else
                {
                    heroPrefab.aideActivitiesHboxContainer.Show();
                }
                heroPrefab.heroListButton.Disabled = false;
                heroPrefab.spawnHeroButton.Show();

                //GD.Print("Hero Token: " + populationData.token);
                // This is only for the players tokens.
                if (populationData.heroToken == null)
                {
                    heroPrefab.spawnHeroButton.ButtonPressed = false;
                    continue;
                }
                else
                {
                    heroPrefab.spawnHeroButton.ButtonPressed = true;
                }
            }
        }

        private void CheckForAvailableActivities(HeroPanelContainer heroPrefab)
        {
            DisableMostActivityCheckboxes(heroPrefab);
            foreach (CountyImprovementData countyImprovementData in countyData.completedCountyImprovementList)
            {
                // Work
                if (countyImprovementData.maxWorkers > 0
                    && countyImprovementData.factionResourceType != AllEnums.FactionGoodType.Research)
                {
                    heroPrefab.heroCheckBoxes[1].Disabled = false;
                }
            }
            // Build
            if (countyData.underConstructionCountyImprovementList.Count > 0)
            {
                heroPrefab.heroCheckBoxes[2].Disabled = false;
            }
        }

        /// <summary>
        /// Disables all checkboxes except scavenge.
        /// </summary>
        /// <param name="heroPrefab"></param>
        private void DisableMostActivityCheckboxes(HeroPanelContainer heroPrefab)
        {
            foreach (CheckBox checkBox in heroPrefab.heroCheckBoxes)
            {
                if (checkBox != heroPrefab.heroCheckBoxes[0]
                    && checkBox != heroPrefab.heroCheckBoxes[3]
                    && checkBox != heroPrefab.heroCheckBoxes[4])
                {
                    checkBox.Disabled = true;
                }
            }
        }

        private void PopulateActivityHboxes(PopulationData populationData, HeroPanelContainer heroPrefab)
        {
            switch (populationData.activity)
            {
                case AllEnums.Activities.Scavenge:
                    heroPrefab.heroCheckBoxes[0].ButtonPressed = true;
                    return;
                case AllEnums.Activities.Work:
                    heroPrefab.heroCheckBoxes[1].ButtonPressed = true;
                    return;
                case AllEnums.Activities.Build:
                    heroPrefab.heroCheckBoxes[2].ButtonPressed = true;
                    return;
                case AllEnums.Activities.Research:
                    heroPrefab.heroCheckBoxes[3].ButtonPressed = true;
                    return;
                case AllEnums.Activities.Explore:
                    heroPrefab.heroCheckBoxes[4].ButtonPressed = true;
                    return;
            }
        }

        public static void UpdateHeroInfo(HeroPanelContainer heroPrefab)
        {
            heroPrefab.heroNameLabel.Text = $"{heroPrefab.populationData.firstName} {heroPrefab.populationData.lastName}";

            // Check for hero activities

            switch (heroPrefab.populationData)
            {
                case { HeroType: AllEnums.HeroType.FactionLeader }: // FactionLeader
                    heroPrefab.factionLeaderTextureRect.Show();
                    heroPrefab.aideTextureRect.Hide();
                    heroPrefab.armyLeaderTextureRect.Hide();
                    break;

                case { HeroType: AllEnums.HeroType.FactionLeaderArmyLeader }: // FactionArmyLeader
                    heroPrefab.factionLeaderTextureRect.Show();
                    heroPrefab.aideTextureRect.Hide();
                    heroPrefab.armyLeaderTextureRect.Show();
                    break;

                case { HeroType: AllEnums.HeroType.Aide }: // Aide
                    heroPrefab.factionLeaderTextureRect.Hide();
                    heroPrefab.aideTextureRect.Show();
                    heroPrefab.armyLeaderTextureRect.Hide();
                    break;

                case { HeroType: AllEnums.HeroType.ArmyLeader }: // ArmyLeader
                    heroPrefab.factionLeaderTextureRect.Hide();
                    heroPrefab.aideTextureRect.Hide();
                    heroPrefab.armyLeaderTextureRect.Show();
                    break;

                default:
                    // Handle any other cases if needed
                    break;
            }
        }

        private void CheckForOwnership()
        {
            if (Globals.Instance.playerFactionData != Globals.Instance.SelectedLeftClickCounty.countyData.factionData)
            {
                populationListButton.Disabled = true;
                countyImprovementsButton.Disabled = true;
                resourcesButton.Disabled = true;
            }
            else
            {
                populationListButton.Disabled = false;
                countyImprovementsButton.Disabled = false;
                resourcesButton.Disabled = false;
            }
        }
        public void UpdateCountyPopulationLabel()
        {

            int population = Globals.Instance.SelectedLeftClickCounty.countyData.populationDataList.Count
                + Globals.Instance.SelectedLeftClickCounty.countyData.heroesInCountyList.Count;
            populationDataNumberLabel.Text = population.ToString();
        }

        // This is going to break once we put people to work.
        public void UpdateIdleWorkersLabel()
        {
            //GD.Print("Update Idle Workers!! " + Globals.Instance.SelectedLeftClickCounty.countyData.IdleWorkers.ToString());
            countyIdleWorkersLabel.Text = Globals.Instance.SelectedLeftClickCounty.countyData.IdleWorkers.ToString();
        }

        private static void ResourcesButtonPressed()
        {
            PlayerUICanvas.Instance.resourcesPanelContainer.Show();
        }
        private void CountyImprovementsButton()
        {
            //GD.Print("Buildings Button has been pressed.");
            countyImprovementsPanelControl.Show();
        }

        private void OnXButtonPressed()
        {
            Globals.Instance.SelectedLeftClickCounty = null;
            Hide();
        }
    }
}