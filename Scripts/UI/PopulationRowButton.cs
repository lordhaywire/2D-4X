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
        /*
        [Export] public Label constructionSkillLabel;
        [Export] public Label coolSkillLabel;
        [Export] public Label farmSkillLabel;
        [Export] public Label fishSkillLabel;
        [Export] public Label lumberjackSkillLabel;
        [Export] public Label researchSkillLabel;
        [Export] public Label rifleSkillLabel;
        [Export] public Label scavengeSkillLabel;
        */
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