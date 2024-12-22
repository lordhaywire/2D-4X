using Godot;

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
        [Export] public CheckBox researchCheckbox;
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

        private void SpawnHeroCheckBox(bool toggleOn)
        {
            if (toggleOn == true && populationData.token == null)
            {
                // Assign to Currently Selected Hero so it is ready to be moved.
                Globals.Instance.SelectedCountyPopulation
                = Globals.Instance.playerFactionData.tokenSpawner.Spawn(Globals.Instance.SelectedLeftClickCounty, populationData);
                //GD.Print("Spawn Hero Check Box " + Globals.Instance.SelectedCountyPopulation.firstName);
            }
        }

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