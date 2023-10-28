using Godot;

namespace PlayerSpace
{
    public partial class PopulationRowButton : Button
    {
        private void OnButtonClick()
        {
            CountyInfoControl.Instance.populationDescriptionMarginContainer.Show();
            CountyInfoControl.Instance.populationListMarginContainer.Hide();
        }
    }
}