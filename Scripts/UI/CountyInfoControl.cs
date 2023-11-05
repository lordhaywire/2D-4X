using Godot;
using System;

namespace PlayerSpace
{
    public partial class CountyInfoControl : Control
    {
        public static CountyInfoControl Instance { get; private set; }

        [Export] private VBoxContainer heroListParent;
        [Export] private PackedScene heroListPrefab;
        [Export] private Label countyPopulationLabel;
        [Export] private Label countyIdleWorkersLabel;
        [Export] public MarginContainer populationListMarginContainer;
        [Export] public MarginContainer populationDescriptionMarginContainer;

        public override void _Ready()
        {
            Instance = this;
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

            foreach (CountyPopulation countyPopulation in countyData.heroCountyPopulation)
            {
                PanelContainer heroPrefab = (PanelContainer)heroListPrefab.Instantiate();
                Label heroNameLabel = (Label)heroPrefab.GetChild(0);
                //GD.Print($"County Hero Name: {hero.firstName}");
                heroNameLabel.Text = $"{countyPopulation.firstName} {countyPopulation.lastName}";
                heroListParent.AddChild(heroPrefab);
                HeroListButton heroListButton = (HeroListButton)heroPrefab;
                heroListButton.countyPopulation = countyPopulation;
                GD.Print("Hero Token: " + countyPopulation.token);
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
        public void UpdateCountyPopulationLabel(int population)
        {
            countyPopulationLabel.Text = population.ToString();
        }

        public void UpdateIdleWorkersLabel(int idleWorkersNumbers)
        {
            countyIdleWorkersLabel.Text = idleWorkersNumbers.ToString();
        }
    }
}