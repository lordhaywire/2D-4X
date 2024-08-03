using Godot;

namespace PlayerSpace
{
    public partial class AssignedResearcherHboxContainer : HBoxContainer
    {
        [Export] public Button assignedResearcherButton;
        [Export] public CheckBox assignedResearcherCheckbox;

        public CountyPopulation countyPopulation;

        
        private void RemoveResearcherCheckBoxToggled(bool toggled)
        {
            if(toggled == false)
            {
                //GD.Print("Research Checkbox has been unchecked.");
                Research research = new();
                research.RemoveResearcher(countyPopulation);
                QueueFree();
            }
        }

        private void AssignedResearcherButton()
        {
            ResearchDescriptionPanel.Instance.researchItemData = countyPopulation.CurrentResearchItemData;
            ResearchDescriptionPanel.Instance.Show();
        }
    }
}