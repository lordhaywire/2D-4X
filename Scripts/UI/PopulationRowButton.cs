using Godot;

namespace PlayerSpace
{
    public partial class PopulationRowButton : Button
    {
        [Export] public CountyPopulation countyPopulation;
        private void OnButtonClick()
        {
            CountyInfoControl.Instance.populationDescriptionMarginContainer.Show();
            CountyInfoControl.Instance.populationListMarginContainer.Hide();
            Globals.Instance.selectedCountyPopulation = countyPopulation;
        }
    }
}