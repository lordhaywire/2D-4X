using Godot;
using System;
using System.Linq;

namespace PlayerSpace
{
    public partial class CountyImprovementsControl : Control
    {
        public static CountyImprovementsControl Instance { get; private set; }

        [Export] private Node possibleImprovementsScrollContainerParent;
        [Export] public Node currentImprovementsScrollContainerParent;
        [Export] public ConfirmationDialog buildConfirmationDialog;

        [Export] private PackedScene countyImprovementButtonPackedScene;

        public override void _Ready()
        {
            Instance = this;
        }
        private void OnVisibilityChanged()
        {
            if (Visible == true)
            {
                Clock.Instance.PauseTime();
                GenerateCountyImprovementButtons();
                PlayerControls.Instance.AdjustPlayerControls(false);
                CountyInfoControl.Instance.populationDescriptionControl.Hide();
                CountyInfoControl.Instance.populationListMarginContainer.Hide();
            }
            else
            {
                Clock.Instance.UnpauseTime();
                PlayerControls.Instance.AdjustPlayerControls(true);
            }
        }

        // This is still set up as if you can only generate one county improvement of each type.
        // We want people to be able to build more then 1 of each.
        public void GenerateCountyImprovementButtons()
        {
            ClearImprovements();
            foreach (CountyImprovementData countyImprovementData in Globals.Instance.SelectedLeftClickCounty.countyData.allCountyImprovements)
            {
                if (countyImprovementData.status == AllEnums.CountyImprovementStatus.None)
                {
                    CountryImprovementDescriptionButton countyImprovementButton = (CountryImprovementDescriptionButton)countyImprovementButtonPackedScene.Instantiate();
                    possibleImprovementsScrollContainerParent.AddChild(countyImprovementButton);
                    countyImprovementButton.countyImprovementData = countyImprovementData;
                }
                else
                {
                    CountryImprovementDescriptionButton countyImprovementButton = (CountryImprovementDescriptionButton)countyImprovementButtonPackedScene.Instantiate();
                    currentImprovementsScrollContainerParent.AddChild(countyImprovementButton);
                    countyImprovementButton.countyImprovementData = countyImprovementData;
                }
            }
        }

        private void ClearImprovements()
        {
            foreach (Node node in possibleImprovementsScrollContainerParent.GetChildren().Skip(1))
            {
                node.QueueFree();
            }
            foreach (Node node in currentImprovementsScrollContainerParent.GetChildren().Skip(1))
            {
                node.QueueFree();
            }
        }
        private void CloseButton()
        {
            Hide();
        }
    }
}