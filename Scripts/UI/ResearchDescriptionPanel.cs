using Godot;
using System;

namespace PlayerSpace
{
    public partial class ResearchDescriptionPanel : PanelContainer
    {
        [Export] private Label researchName;
        [Export] private Label researchDescription;
        private async void OnVisibilityChanged()
        {
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            GD.Print("Research Description Panel!");
            researchName.Text = Globals.Instance.playerFactionData.researchItems[Globals.Instance.researchClicked].researchName;
            researchDescription.Text = Globals.Instance.playerFactionData.researchItems[Globals.Instance.researchClicked].description;
        }
    }
}
