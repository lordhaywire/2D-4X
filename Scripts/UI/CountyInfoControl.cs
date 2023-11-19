using Godot;
using System;

namespace PlayerSpace
{
    public partial class CountyInfoControl : Control
    {
        public static CountyInfoControl Instance { get; private set; }

        [ExportGroup("Public Shit")]
        [Export] public MarginContainer populationListMarginContainer;
        [Export] public MarginContainer populationDescriptionMarginContainer;

        [ExportGroup("Private Variables")]
        [Export] private VBoxContainer heroListParent;
        [Export] private VBoxContainer heroSpawnCheckButtonParent;
        [Export] private PackedScene heroListPrefab;
        [Export] private Label countyPopulationLabel;
        [Export] private Label countyIdleWorkersLabel;

        public override void _Ready()
        {
            Instance = this;
        }

        public void DisableSpawnHeroCheckButton(bool value)
        {
            foreach (Node node in heroSpawnCheckButtonParent.GetChildren())
            {
                CheckButton checkbutton = (CheckButton)node.GetChild(2);
                checkbutton.Disabled = value;
            }
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
                PanelContainer heroPrefab = (PanelContainer)heroListPrefab.Instantiate();
                Label heroNameLabel = (Label)heroPrefab.GetChild(0);
                //GD.Print($"County Hero Name: {hero.firstName}");
                heroNameLabel.Text = $"{countyPopulation.firstName} {countyPopulation.lastName}";
                heroListParent.AddChild(heroPrefab);
                HeroListButton heroListButton = (HeroListButton)heroPrefab;
                heroListButton.countyPopulation = countyPopulation;
                //GD.Print("Hero Token: " + countyPopulation.token);
                if(countyPopulation.token == null)
                {
                    heroListButton.GetNode<CheckButton>("CheckButton").ButtonPressed = false;
                    continue;
                }
                else
                {
                    heroListButton.GetNode<CheckButton>("CheckButton").ButtonPressed = true;
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
            countyIdleWorkersLabel.Text = countyPopulationLabel.Text;
        }
    }
}