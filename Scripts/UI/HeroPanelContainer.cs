using Godot;
using System.Linq;

namespace PlayerSpace
{
    public partial class HeroPanelContainer : PanelContainer
    {
        public PopulationData populationData;

        [Export] public TextureRect factionLeaderTextureRect;
        [Export] public TextureRect aideTextureRect;
        [Export] public TextureRect armyLeaderTextureRect;
        [Export] public Label heroNameLabel;
        [Export] public Button heroListButton;
        [Export] public CheckButton spawnHeroButton;
        [Export] public HBoxContainer aideActivitiesHboxContainer;
        [Export] public HBoxContainer armyActivitiesHboxContainer;
        [Export] public CheckBox[] heroCheckBoxes;
        //[Export] public CheckBox researchCheckbox;
        private void HeroButtonOnPressed()
        {
            PopulationDescriptionControl.Instance.populationData = populationData;
            CountyInfoControl.Instance.populationDescriptionControl.Show();
            if (CountyInfoControl.Instance.populationDescriptionControl.Visible == true)
            {
                PopulationDescriptionControl.Instance.UpdateDescriptionInfo();
            }
            CountyInfoControl.Instance.populationListMarginContainer.Hide();
            PopulationDescriptionControl.Instance.heroButtonClicked = true;
        }

        private void DeselectAllOtherCheckBoxes(int numberOfCheckBox)
        {
            foreach (CheckBox checkBox in heroCheckBoxes)
            {
                if (checkBox == heroCheckBoxes[numberOfCheckBox])
                {
                    continue;
                }

                checkBox.ButtonPressed = false;
            }
        }

        // I wonder if the populationData.IsHeroSpawned is needed anymore.
        private void SpawnHeroCheckButton()
        {
            GD.Print($"{populationData.firstName} token is: {populationData.token}");
            
            if (spawnHeroButton.ButtonPressed)// == true && populationData.IsHeroSpawned() == false)
            {
                
                // Assign to Currently Selected Hero so it is ready to be moved.
                Globals.Instance.SelectedCountyPopulation
                = TokenSpawner.Spawn(Globals.Instance.SelectedLeftClickCounty, populationData);
                GD.Print("Spawn Hero Check Box " + Globals.Instance.SelectedCountyPopulation.firstName);
                CountyInfoControl.Instance.UpdateEverything();
                
                GD.Print("Toggled: " + spawnHeroButton.ButtonPressed);
            }
            else
            {
                TokenSpawner.Unspawn(Globals.Instance.SelectedLeftClickCounty, populationData);
                GD.Print("Else Toggled: " + spawnHeroButton.ButtonPressed);
            }
        }

        private void HeroActivitiesCheckBoxPressed(int numberOfCheckBox)
        {
            DeselectAllOtherCheckBoxes(numberOfCheckBox);

            switch (numberOfCheckBox)
            {
                case 0:
                    populationData.UpdateActivity(AllEnums.Activities.Scavenge);
                    return;
                case 1:
                    populationData.UpdateActivity(AllEnums.Activities.Work);
                    return;
                case 2:
                    populationData.UpdateActivity(AllEnums.Activities.Research);
                    return;
                case 3:
                    populationData.UpdateActivity(AllEnums.Activities.Explore);
                    return;
                default:
                    GD.Print("Some hero activity doesn't exist.");
                    return;
            }
        }

        // This needs to be moved into the switch above.
        private void ResearchCheckBoxToggled(bool toggled)
        {
            //GD.Print("Research Checkbox Toggled: " + toggled);
            if (toggled == false)
            {
                Research research = new();
                research.RemoveResearcher(populationData);
            }
        }

        // This needs to be on this script because this is also instantiated.
        public static void OnMouseEnteredUI()
        {
            PlayerControls.Instance.stopClickThrough = true;
            //GD.Print("Mouse Over UI: " + PlayerControls.Instance.stopClickThrough);
        }

        public static void OnMouseExitedUI()
        {
            PlayerControls.Instance.stopClickThrough = false;
            //GD.Print("Mouse Over UI: " + PlayerControls.Instance.stopClickThrough);
        }
    }
}