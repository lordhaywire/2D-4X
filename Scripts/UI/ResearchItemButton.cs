using Godot;

namespace PlayerSpace
{
    public partial class ResearchItemButton : PanelContainer
    {
        [Export] public ResearchItemData researchItemData;
        [Export] private Button researchButton;
        [Export] private ProgressBar researchProgressBar;
        [Export] private CheckBox researchCheckBox;

        public override void _Ready()
        {
            CallDeferred(nameof(SubscribeToResearchEvent));
            InitialResearchInfoUpdate();
        }

        private void SubscribeToResearchEvent()
        {
            ResearchControl.Instance.ResearchVisible += CheckIfResearchIsDone;
        }

        private void InitialResearchInfoUpdate()
        {
            researchButton.Text = researchItemData.researchName;
            researchButton.Icon = researchItemData.researchTexture;
        }

        /*
        private void UpdateIfResearched()
        {
            GD.Print("Research Panel has opened and this item has updated: " + researchItemData.researchName);
            CheckIfResearchIsDone();
        }
        */

        private void CheckIfResearchIsDone()
        {
            researchProgressBar.MaxValue = researchItemData.costOfResearch;
            researchProgressBar.Value = researchItemData.AmountOfResearchDone;
        }
        private void OnButtonPressed()
        {
            ResearchDescriptionPanel.Instance.researchItemData = researchItemData;
            ResearchDescriptionPanel.Instance.Show();
            //ResearchControl.Instance.researchItemParent.Hide();
        }
    }
}