using Godot;

namespace PlayerSpace
{
    public partial class AssignedResearcherHboxContainer : HBoxContainer
    {
        [Export] public Button assignedResearcherButton;
        [Export] public CheckBox assignedResearcherCheckbox;

        public PopulationData populationData;

        
        private void RemoveResearcherCheckBoxToggled(bool toggled)
        {
            if(toggled == false)
            {
                //GD.Print("Research Checkbox has been unchecked.");
                Research research = new();
                research.RemoveResearcher(populationData);
                QueueFree();
            }
        }

        private void AssignedResearcherButton()
        {
            ResearchDescriptionPanel.Instance.researchItemData = populationData.currentResearchItemData;
            ResearchDescriptionPanel.Instance.Show();
        }
    }
}