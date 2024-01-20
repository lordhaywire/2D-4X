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

        [ExportGroup("Buttons")]
        [Export] private Button populationListButton;
        [Export] private Button countyImprovementsButton;

        [Export] private PackedScene heroListPrefab;

        public override void _Ready()
        {
            Instance = this;
        }

        private void OnVisibilityChanged()
        {
            // Idle workers changes if we change who is building stuff etc.
            if(Visible == true)
            {
                Globals.Instance.selectedCountyData.IdleWorkersChanged += UpdateIdleWorkersLabel;             
            }
            else
            {
                Globals.Instance.selectedCountyData.IdleWorkersChanged -= UpdateIdleWorkersLabel;
            }
        }

        public void UpdateEverything()
        {
            CheckForOwnership();
            UpdateNameLabels();
            UpdateCountyPopulationLabel();
            UpdateIdleWorkersLabel();
            GenerateHeroesPanelList();
        }
        public void DisableSpawnHeroCheckButton(bool value)
        {
            foreach (Node node in heroSpawnCheckButtonParent.GetChildren())
            {
                HeroPanelContainer heroPanelContainer = (HeroPanelContainer)node;
                if(heroPanelContainer.countyPopulation.factionData == Globals.Instance.playerFactionData)
                {
                    heroPanelContainer.spawnHeroButton.Disabled = value;
                }
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
                HeroPanelContainer heroPrefab = (HeroPanelContainer)heroListPrefab.Instantiate();

                heroPrefab.heroNameLabel.Text = $"{countyPopulation.firstName} {countyPopulation.lastName}";
                /*
                GD.Print($"Faction Leader: {countyPopulation.isLeader}");
                GD.Print($"Aide: {countyPopulation.isAide}");
                GD.Print($"Army Leader: {countyPopulation.isArmyLeader}");
                */
                countyPopulation.location = countyData.countyId;
                
                switch (countyPopulation)
                {
                    case { isLeader: true, isAide: false, isArmyLeader: false }:
                        heroPrefab.factionLeaderTextureRect.Show();
                        heroPrefab.aideTextureRect.Hide();
                        heroPrefab.armyLeaderTextureRect.Hide();
                        break;

                    case { isLeader: true, isAide: false, isArmyLeader: true }:
                        heroPrefab.factionLeaderTextureRect.Show();
                        heroPrefab.aideTextureRect.Hide();
                        heroPrefab.armyLeaderTextureRect.Show();
                        break;

                    case { isLeader: false, isAide: true, isArmyLeader: false }:
                        heroPrefab.factionLeaderTextureRect.Hide();
                        heroPrefab.aideTextureRect.Show();
                        heroPrefab.armyLeaderTextureRect.Hide();
                        break;

                    case { isLeader: false, isAide: false, isArmyLeader: true }:
                        heroPrefab.factionLeaderTextureRect.Hide();
                        heroPrefab.aideTextureRect.Hide();
                        heroPrefab.armyLeaderTextureRect.Show();
                        break;

                    default:
                        // Handle any other cases if needed
                        break;
                }

                heroListParent.AddChild(heroPrefab);
                heroPrefab.countyPopulation = countyPopulation;

                // Change color of panel to the faction color.
                heroPrefab.SelfModulate = countyPopulation.factionData.factionColor;

                GD.Print("Hero faction: " + heroPrefab.countyPopulation.factionData.factionName);
                GD.Print("Player faction: " + Globals.Instance.playerFactionData.factionName);
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

        private void CheckForOwnership()
        {
            if (Globals.Instance.playerFactionData != Globals.Instance.selectedCountyData.factionData)
            {
                populationListButton.Disabled = true;
                countyImprovementsButton.Disabled = true;
            }
            else
            {
                populationListButton.Disabled = false;
                countyImprovementsButton.Disabled = false;
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
            //GD.Print("Update Idle Workers !!");
            countyIdleWorkersLabel.Text = Globals.Instance.selectedCountyData.IdleWorkers.ToString();
        }

        private void CountyImprovementsButton()
        {
            //GD.Print("Buildings Button has been pressed.");
            countyImprovementsPanelControl.Show();
        }
    }
}

/*
if (countyPopulation.isLeader)
{
    heroPrefab.factionLeaderTextureRect.Show();
    heroPrefab.aideTextureRect.Hide();
    heroPrefab.armyLeaderTextureRect.Hide();

    if (countyPopulation.isArmyLeader)
    {
        heroPrefab.armyLeaderTextureRect.Show();
    }
}
else if (countyPopulation.isAide)
{
    heroPrefab.factionLeaderTextureRect.Hide();
    heroPrefab.aideTextureRect.Show();
    heroPrefab.armyLeaderTextureRect.Hide();
}
else if (countyPopulation.isArmyLeader)
{
    heroPrefab.factionLeaderTextureRect.Hide();
    heroPrefab.aideTextureRect.Hide();
    heroPrefab.armyLeaderTextureRect.Show();
}
*/
/*
if (countyPopulation.isLeader == true && countyPopulation.isArmyLeader == false)
{
    heroPrefab.factionLeaderTextureRect.Show();
    heroPrefab.aideTextureRect.Hide();
    heroPrefab.armyLeaderTextureRect.Hide();
}
if (countyPopulation.isLeader == true && countyPopulation.isArmyLeader == true)
{
    heroPrefab.factionLeaderTextureRect.Show();
    heroPrefab.aideTextureRect.Hide();
    heroPrefab.armyLeaderTextureRect.Show();
}
if (countyPopulation.isAide == true)
{
    heroPrefab.factionLeaderTextureRect.Hide();
    heroPrefab.aideTextureRect.Show();
    heroPrefab.armyLeaderTextureRect.Hide();
}
if(countyPopulation.isArmyLeader == true && countyPopulation.isLeader == false)
{
    heroPrefab.factionLeaderTextureRect.Hide();
    heroPrefab.aideTextureRect.Hide();
    heroPrefab.armyLeaderTextureRect.Show();
}
*/