using Godot;

namespace PlayerSpace
{
    public partial class PopulationRowButton : Button
    {
        [Export] public CountyPopulation countyPopulation;
        [Export] public Label populationNameLabel;
        [Export] public Label ageLabel;
        [Export] public Label sexLabel;
        [Export] public Label currentActivityLabel;
        [Export] public Label nextActivityLabel;
        private void OnButtonClick()
        {
            CountyInfoControl.Instance.populationDescriptionMarginContainer.Show();
            CountyInfoControl.Instance.populationListMarginContainer.Hide();
            Globals.Instance.selectedCountyPopulation = countyPopulation;
        }
    }
}