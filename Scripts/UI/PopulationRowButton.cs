using Godot;

namespace PlayerSpace
{
    public partial class PopulationRowButton : Button
    {
        public CountyPopulation countyPopulation;
        [Export] public Label populationNameLabel;
        [Export] public Label ageLabel;
        [Export] public Label sexLabel;
        [Export] public Label UnhelpfulLabel;
        [Export] public Label loyaltyAttributeLabel;
        [Export] public Label[] skillLabels;

        [Export] public Label currentActivityLabel;
        [Export] public Label currentWhereLabel;
        [Export] public Label nextActivityLabel;
        [Export] public Label nextWhereLabel;
        private void OnButtonClick()
        {
            CountyInfoControl.Instance.populationDescriptionControl.Show();
            CountyInfoControl.Instance.populationListMarginContainer.Hide();
            PopulationDescriptionControl.Instance.countyPopulation = countyPopulation;
        }
    }
}