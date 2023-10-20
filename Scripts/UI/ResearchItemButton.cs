using Godot;

namespace PlayerSpace
{
    public partial class ResearchItemButton : PanelContainer
    {
        private void OpenResearchDescriptionPanel()
        {
            //await ToSignal(GetTree(), "process_frame");
            ResearchControl.Instance.researchDescriptionPanel.Show();
            ResearchControl.Instance.researchItemParent.Hide();
            
            //await ToSignal(GetTree(), "process_frame");
            Globals.Instance.researchClicked = int.Parse(Name);
            GD.PrintRich("[rainbow]Name of Button! " + Name);
        }
    }
}