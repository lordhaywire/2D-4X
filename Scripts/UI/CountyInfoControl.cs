using Godot;


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
        [Export] private Label countyPopulationLabel;
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
                TopBarControl.Instance.UpdateResourceLabels();
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
            countyData = Globals.Instance.SelectedLeftClickCounty.countyData;
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
            visitorsLabel.Text = Globals.Instance.SelectedLeftClickCounty.countyData.visitingHeroList.Count().ToString();
            if (Globals.Instance.SelectedLeftClickCounty.countyData.visitingHeroList.Count() == 0)
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
                if (heroPanelContainer.countyPopulation.factionData == Globals.Instance.playerFactionData)
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
            GenerateHeroes(countyData.herosInCountyList);
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

        private void GenerateHeroes(Globals.ListWithNotify<CountyPopulation> heroCountyPopulationList)
        {
            foreach (CountyPopulation countyPopulation in heroCountyPopulationList)
            {
                HeroPanelContainer heroPrefab = (HeroPanelContainer)heroListPrefab.Instantiate();

                UpdateHeroInfo(heroPrefab, countyPopulation);

                heroListParent.AddChild(heroPrefab);
                heroPrefab.countyPopulation = countyPopulation;

                // Change color of panel to the faction color.
                heroPrefab.SelfModulate = countyPopulation.factionData.factionColor;

                if (heroPrefab.countyPopulation.factionData != Globals.Instance.playerFactionData)
                {
                    heroPrefab.heroListButton.Disabled = true;
                    heroPrefab.spawnHeroButton.Disabled = true;
                }
                else
                {
                    heroPrefab.heroListButton.Disabled = false;
                    heroPrefab.spawnHeroButton.Disabled = false;
                }
                //GD.Print("Hero Token: " + countyPopulation.token);
                if (countyPopulation.token == null)
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

        public void UpdateHeroInfo(HeroPanelContainer heroPrefab, CountyPopulation countyPopulation)
        {
            heroPrefab.heroNameLabel.Text = $"{countyPopulation.firstName} {countyPopulation.lastName}";

            // Check for hero activities
            if (heroPrefab.researchCheckbox != null)
            {
                heroPrefab.researchCheckbox.ButtonPressed = false;
            }
            //GD.Print("Researching?" + countyPopulation.currentResearchItemData.researchName);
            if (countyPopulation.currentResearchItemData != null)
            {
                //GD.Print("Research CheckBox!?");
                heroPrefab.researchCheckbox.ButtonPressed = true;
            }
            else
            {
                //GD.Print($"{countyPopulation.firstName} research is null.");
            }

            switch (countyPopulation)
            {
                case { isFactionLeader: true, isAide: false, IsArmyLeader: false }:
                    heroPrefab.factionLeaderTextureRect.Show();
                    heroPrefab.aideTextureRect.Hide();
                    heroPrefab.armyLeaderTextureRect.Hide();
                    break;

                case { isFactionLeader: true, isAide: false, IsArmyLeader: true }:
                    heroPrefab.factionLeaderTextureRect.Show();
                    heroPrefab.aideTextureRect.Hide();
                    heroPrefab.armyLeaderTextureRect.Show();
                    break;

                case { isFactionLeader: false, isAide: true, IsArmyLeader: false }:
                    heroPrefab.factionLeaderTextureRect.Hide();
                    heroPrefab.aideTextureRect.Show();
                    heroPrefab.armyLeaderTextureRect.Hide();
                    break;

                case { isFactionLeader: false, isAide: false, IsArmyLeader: true }:
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

            int population = Globals.Instance.SelectedLeftClickCounty.countyData.countyPopulationList.Count()
                + Globals.Instance.SelectedLeftClickCounty.countyData.herosInCountyList.Count();
            countyPopulationLabel.Text = population.ToString();
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