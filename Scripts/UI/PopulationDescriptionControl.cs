using Godot;
using System;

namespace PlayerSpace
{
    public partial class PopulationDescriptionControl : Control
    {
        public static PopulationDescriptionControl Instance { get; private set; }

        [Export] private Label populationName;
        [Export] private CheckBox leaderCheckbox;
        [Export] private CheckBox aideCheckbox;
        [Export] private CheckBox armyLeaderCheckbox;
        [Export] private Label age;
        [Export] private Label sex;
        [Export] private Label firstPerkLabel;
        [Export] private Label noPerksLabel;
        [Export] private Label constructionSkill;
        [Export] private Label currentActivity;
        [Export] private Label nextActivity;

        [Export] private Button aideRecruitButton;
        [Export] private Button armyLeaderRecruitButton;
        [Export] private PanelContainer heroRecruitmentConfirmPanel;

        public override void _Ready()
        {
            Instance = this;
        }

        private void OnVisibilityChanged()
        {
            if (Visible == true)
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
            CountyPopulation countyPopulation = Globals.Instance.selectedCountyPopulation;

            //GD.Print("It goes to the update description: " + person.firstName);
            populationName.Text = $"{countyPopulation.firstName} {countyPopulation.lastName}";

            DisableAllTitles();
            // Titles
            if (countyPopulation.isLeader == true)
            {
                leaderCheckbox.Disabled = false;

            }
            if (countyPopulation.isAide == true)
            {
                aideCheckbox.Disabled = false;
            }
            if (countyPopulation.isArmyLeader == true)
            {
                armyLeaderCheckbox.Disabled = false;
            }

            age.Text = countyPopulation.age.ToString();

            if (countyPopulation.isMale)
            {
                sex.Text = "Male";
            }
            else
            {
                sex.Text = "Female";
            }

            if (countyPopulation.leaderOfPeoplePerk == true)
            {
                firstPerkLabel.Text = "Leader of People";
                firstPerkLabel.Show();
                noPerksLabel.Hide();
            }
            else
            {
                firstPerkLabel.Hide();
                noPerksLabel.Show();
            }

            constructionSkill.Text = $"Contruction: {countyPopulation.constructionSkill}";

            if (countyPopulation.currentImprovement != null)
            {
                currentActivity.Text = $"{countyPopulation.currentActivity} {countyPopulation.currentImprovement.improvementName}";
            }
            else
            {
                currentActivity.Text = $"{countyPopulation.currentActivity}";
            }
            if (countyPopulation.nextImprovement != null)
            {
                nextActivity.Text = $"{countyPopulation.nextActivity} {countyPopulation.nextImprovement.improvementName}";

            }
            else
            {
                nextActivity.Text = $"{countyPopulation.nextActivity}";
            }

            if (Globals.Instance.playerFactionData.Influence < Globals.Instance.costOfHero || countyPopulation.isHero == true)
            {
                aideRecruitButton.Disabled = true;
            }
            else
            {
                aideRecruitButton.Disabled = false;
            }

            if (countyPopulation.isArmyLeader == true)
            {
                armyLeaderRecruitButton.Disabled = true;
            }
            else
            {
                armyLeaderRecruitButton.Disabled = false;
            }
        }

        private void DisableAllTitles()
        {
            leaderCheckbox.Disabled = true;
            aideCheckbox.Disabled = true;
            armyLeaderCheckbox.Disabled = true;
        }

        private void CloseButton()
        {
            Hide();
        }
    }
}