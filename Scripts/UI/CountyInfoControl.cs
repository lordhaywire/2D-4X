using Godot;
using System;

namespace PlayerSpace
{


    public partial class CountyInfoControl : Control
    {
        public static CountyInfoControl Instance { get; private set; }

        [Export] private Label countyPopulationLabel;
        [Export] private Label countyIdleWorkersLabel;

        public override void _Ready()
        {
            Instance = this;
        }
        public void UpdateCountyPopulationLabel(int population)
        {
            countyPopulationLabel.Text = population.ToString();
        }

        public void UpdateIdleWorkersLabel(int idleWorkers)
        {
            countyIdleWorkersLabel.Text = idleWorkers.ToString();
        }
    }
}