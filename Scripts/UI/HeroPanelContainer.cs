using Godot;

namespace PlayerSpace
{
    public partial class HeroPanelContainer : PanelContainer
    {
        public CountyPopulation countyPopulation;

        [Export] public TextureRect factionLeaderTextureRect;
        [Export] public TextureRect aideTextureRect;
        [Export] public TextureRect armyLeaderTextureRect;
        [Export] public Label heroNameLabel;
        [Export] public Button heroListButton;
        [Export] public CheckButton spawnHeroButton;
        private void HeroButton()
        {
            PopulationDescriptionControl.Instance.countyPopulation = countyPopulation;
            CountyInfoControl.Instance.populationDescriptionControl.Show();
            if (CountyInfoControl.Instance.populationDescriptionControl.Visible == true)
            {
                PopulationDescriptionControl.Instance.UpdateDescriptionInfo();
            }
            CountyInfoControl.Instance.populationListMarginContainer.Hide();
            //spawnHeroButton.Disabled = true;
        }

        private void SpawnHeroCheckBox(bool toggleOn)
        {
            if (toggleOn == true && countyPopulation.token == null)
            {
                // Assign to Currently Selected Hero so it is ready to be moved.
                Globals.Instance.SelectedCountyPopulation 
                = Globals.Instance.playerFactionData.tokenSpawner.Spawn(Globals.Instance.selectedLeftClickCounty, countyPopulation);
                GD.Print("Spawn Hero Check Box " + Globals.Instance.SelectedCountyPopulation.firstName);
            }
        }

        public void OnMouseEnteredUI()
        {
            PlayerControls.Instance.stopClickThrough = true;
            //GD.Print("Mouse Over UI: " + PlayerControls.Instance.stopClickThrough);
        }

        public void OnMouseExitedUI()
        {
            PlayerControls.Instance.stopClickThrough = false;
            //GD.Print("Mouse Over UI: " + PlayerControls.Instance.stopClickThrough);
        }
    }
}