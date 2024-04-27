using Godot;
using System;

namespace PlayerSpace
{

    public partial class ResourcesPanelContainer : PanelContainer
    {
        [Export] private Label maxPerishableAmountLabel;
        [Export] private Label maxNonperisableAmountLabel;

        [Export] public HBoxContainer[] resourceHboxContainers;
        private void OnVisibilityChanged()
        {
            if(Visible)
            {
                PlayerControls.Instance.playerControlsEnabled = false;
                UpdateMaxAmountLabels();
                UpdateResourceLabels();
            }
            else
            {
                PlayerControls.Instance.playerControlsEnabled = true;
            }
        }

        private void UpdateResourceLabels()
        {
            for(int i=0; i < Globals.Instance.selectedLeftClickCounty.countyData.resources.Length; i++) 
            {
                resourceHboxContainers[i]
            }
        }

        private void UpdateMaxAmountLabels()
        {
            maxPerishableAmountLabel.Text = Globals.Instance.selectedLeftClickCounty.countyData.perishableStorage.ToString();
            maxNonperisableAmountLabel.Text = Globals.Instance.selectedLeftClickCounty.countyData.nonperishableStorage.ToString();
        }

        private void CloseButtonPressed()
        {
            Hide();
        }
    }
}