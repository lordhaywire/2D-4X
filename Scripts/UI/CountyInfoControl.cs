using Godot;

namespace PlayerSpace
{
    public partial class CountyInfoControl : Control
    {
        public static CountyInfoControl Instance { get; private set; }

        [ExportGroup("County Info")] 
        [Export] public Control countyInfoControl;
        [Export] public Label factionNameLabel;
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
            }
            else
            {
                // Update the faction resources, which is all the county resources.
                TopBarControl.Instance.UpdateTopBarGoodLabels();
            }
        }

        public static void UpdateSelectedHero()
        {
            PlayerUICanvas.Instance.selectedHeroPanelContainer.populationData = Globals.Instance.SelectedCountyPopulation;
            PlayerUICanvas.Instance.selectedHeroPanelContainer.UpdateHeroNameAndIcons();
        
            if (PlayerUICanvas.Instance.selectedHeroPanelContainer.populationData.heroToken?.tokenMovement.MoveToken == true)
            {
                PlayerUICanvas.Instance.selectedHeroPanelContainer.ShowMovementActivityHBoxContainer();
            }
            else
            {
                PlayerUICanvas.Instance.selectedHeroPanelContainer.movementActivityHBoxContainer.Hide();
            }
        }
        private static void OnMouseEntered()
        {
            PlayerControls.Instance.stopClickThrough = true;
        }

        private static void OnMouseExited()
        {
            PlayerControls.Instance.stopClickThrough = false;
        }

        // This update everything needs to be looked at.
        // When you click on a county it does this method.
        public void UpdateEverything()
        {
            countyData = Globals.Instance.SelectedLeftClickCounty?.countyData;
            if (countyData == null)
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
            countyFoodLabel.Text = $"Food: {countyData.scavengeableCannedFood}";
            countyScrapLabel.Text = $"Remnants: {countyData.scavengeableRemnants}";
        }

        private void UpdateVisitorsPopulationLabel()
        {
            visitorsLabel.Text
                = Globals.Instance.SelectedLeftClickCounty.countyData.visitingHeroList.Count.ToString();
            visitorsListButton.Disabled
                = Globals.Instance.SelectedLeftClickCounty.countyData.visitingHeroList.Count == 0;
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

        private void UpdateNameLabels()
        {
            factionNameLabel.Text = Globals.Instance.SelectedLeftClickCounty.countyData.factionData.factionName;
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
                
                heroPrefab.populationData = populationData;
                
                heroListParent.AddChild(heroPrefab);
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

        private void UpdateCountyPopulationLabel()
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