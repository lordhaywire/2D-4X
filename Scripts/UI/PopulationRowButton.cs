using Godot;

namespace PlayerSpace
{
    public partial class PopulationRowButton : Button
    {
        public CountyPopulation countyPopulation;
        [Export] public Label populationNameLabel;
        [Export] public Label ageLabel;
        [Export] public Label sexLabel;
        [Export] public Label constructionSkillLabel;
        [Export] public Label rifleSkillLabel;
        [Export] public Label currentActivityLabel;
        [Export] public Label nextActivityLabel;
        private void OnButtonClick()
        {
            CountyInfoControl.Instance.populationDescriptionControl.Show();
            CountyInfoControl.Instance.populationListMarginContainer.Hide();
            PopulationDescriptionControl.Instance.countyPopulation = countyPopulation;
        }
    }
}