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
        [Export] private Label countyPopulationLabel;
        [Export] private Label countyIdleWorkersLabel;

        [ExportGroup("Containers and shit")]
        [Export] public MarginContainer populationListMarginContainer;
        [Export] public Control populationDescriptionControl;
        [Export] public Control countyImprovementsPanelControl;
        [Export] private VBoxContainer heroListParent;
        [Export] private VBoxContainer heroSpawnCheckButtonParent;

        [Export] private PackedScene heroListPrefab;

        public override void _Ready()
        {
            Instance = this;
        }

        private void OnVisibilityChanged()
        {
            if(Visible == true)
            {
                Globals.Instance.selectedCountyData.IdleWorkersChanged += UpdateIdleWorkersLabel;
            }
            else
            {
                Globals.Instance.selectedCountyData.IdleWorkersChanged -= UpdateIdleWorkersLabel;
            }
        }

        public void DisableSpawnHeroCheckButton(bool value)
        {
            foreach (Node node in heroSpawnCheckButtonParent.GetChildren())
            {
                HeroListButton heroListButton = (HeroListButton)node;
                heroListButton.spawnHeroButton.Disabled = value;
            }
        }

        public void UpdateNameLabels()
        {
            factionNamelabel.Text = Globals.Instance.selectedCountyData.factionData.factionName;
            countyNameLabel.Text = Globals.Instance.selectedCountyData.countyName;
        }
        public void GenerateHeroesPanelList()
        {
            CountyData countyData = Globals.Instance.selectedCountyData;

            if (heroListParent.GetChildCount() != 0)
            {
                foreach (Node hero in heroListParent.GetChildren())
                {
                    hero.QueueFree();
                }
            }
            //GD.Print($"{countyData.countyName} has {countyData.heroCountyPopulation.Count} heroes in it.");
            foreach (CountyPopulation countyPopulation in countyData.heroCountyPopulation)
            {
                HeroListButton heroPrefab = (HeroListButton)heroListPrefab.Instantiate();

                heroPrefab.heroNameLabel.Text = $"{countyPopulation.firstName} {countyPopulation.lastName}";
                heroListParent.AddChild(heroPrefab);
                heroPrefab.countyPopulation = countyPopulation;
                //GD.Print("Hero Token: " + countyPopulation.token);
                if(countyPopulation.token == null)
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
        public void UpdateCountyPopulationLabel()
        {
            int population = Globals.Instance.selectedCountyData.countyPopulation.Count + Globals.Instance.selectedCountyData.heroCountyPopulation.Count;
            countyPopulationLabel.Text = population.ToString();
        }

        // This is going to break once we put people to work.
        public void UpdateIdleWorkersLabel()
        {
            GD.Print("Update Idle Workers !!");
            countyIdleWorkersLabel.Text = Globals.Instance.selectedCountyData.IdleWorkers.ToString();
        }

        private void BuildingsButton()
        {
            GD.Print("Buildings Button has been pressed.");
            countyImprovementsPanelControl.Show();
        }
    }
}