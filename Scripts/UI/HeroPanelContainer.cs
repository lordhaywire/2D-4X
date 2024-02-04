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
            Globals.Instance.selectedCountyPopulation = countyPopulation;
            GD.Print("Hero Panel Container.  How has any of this shit worked?" + Globals.Instance.selectedCountyPopulation.firstName);
            CountyInfoControl.Instance.populationDescriptionControl.Show();
            if (CountyInfoControl.Instance.populationDescriptionControl.Visible == true)
            {
                PopulationDescriptionControl.Instance.UpdateDescriptionInfo();
            }
            CountyInfoControl.Instance.populationListMarginContainer.Hide();
        }

        private void SpawnHeroCheckBox(bool toggleOn)
        {
            if (toggleOn == true && countyPopulation.token == null)
            {
                // Assign to Currently Selected Hero so it is ready to be moved.
                Globals.Instance.selectedCountyPopulation 
                = Globals.Instance.playerFactionData.tokenSpawner.Spawn(Globals.Instance.selectedLeftClickCounty, countyPopulation);
                GD.Print("Spawn Hero Check Box " + Globals.Instance.selectedCountyPopulation.firstName);
            }
        }

        public void OnMouseEntered()
        {
            PlayerControls.Instance.mouseOverUI = true;
            //GD.Print("Mouse Over UI: " + PlayerControls.Instance.mouseOverUI);
        }

        public void OnMouseExited()
        {
            PlayerControls.Instance.mouseOverUI = false;
            //GD.Print("Mouse Over UI: " + PlayerControls.Instance.mouseOverUI);
        }
    }
}