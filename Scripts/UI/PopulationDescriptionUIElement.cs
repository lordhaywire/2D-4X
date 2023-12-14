using Godot;

namespace PlayerSpace
{
    public partial class PopulationDescriptionUIElement : MarginContainer
    {
        public static PopulationDescriptionUIElement Instance { get; private set; }

        [Export] private Label populationName;
        [Export] private Label age;
        [Export] private Label sex;
        [Export] private Label constructionSkill;
        [Export] private Label currentActivity;
        [Export] private Label nextActivity;

        [Export] private PanelContainer heroRecruitmentConfirmPanel;
        [Export] private Button recruitButton;
        
        public override void _Ready()
        {
            Instance = this;
        }

        private void OnVisibilityChange()
        {
            if(Visible == true)
            {
                CallDeferred("UpdateDescriptionInfo");
                Clock.Instance.PauseTime();
                CountyInfoControl.Instance.countyImprovementsPanelControl.Hide();
                CountyInfoControl.Instance.populationListMarginContainer.Hide();
            }
            else
            {
                CountyInfoControl.Instance.DisableSpawnHeroCheckButton(false);
                heroRecruitmentConfirmPanel.Hide();
                Globals.Instance.selectedCountyPopulation = null;
                PlayerControls.Instance.AdjustPlayerControls(true);
                Clock.Instance.UnpauseTime();
            }
        }

        public void UpdateDescriptionInfo()
        {
            CountyInfoControl.Instance.DisableSpawnHeroCheckButton(true);
            PlayerControls.Instance.AdjustPlayerControls(false); // This was probably happening too fast which is why it is here.
            CountyPopulation person = Globals.Instance.selectedCountyPopulation;

            if (Globals.Instance.playerFactionData.Influence < Globals.Instance.costOfHero || person.isHero == true)
            {
                recruitButton.Disabled = true;
            }
            else
            {
                recruitButton.Disabled = false;
            }

            //GD.Print("It goes to the update description: " + person.firstName);
            populationName.Text = $"{person.firstName} {person.lastName}";
            age.Text = person.age.ToString();
            constructionSkill.Text = person.constructionSkill.ToString();
            currentActivity.Text = person.currentActivity.ToString();
            nextActivity.Text = person.nextActivity.ToString();
            if(person.isMale)
            {
                sex.Text = "Male";
            }
            else
            {
                sex.Text = "Female";
            }
        }
        private void CloseButton()
        {
            Hide();
        }
    }
}