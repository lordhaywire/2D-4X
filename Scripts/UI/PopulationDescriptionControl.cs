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
        [Export] private Label nextActivityTitle;
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

        // I should probably rewrite this so it is less of a mess.
        public void UpdateDescriptionInfo()
        {
            CountyInfoControl.Instance.DisableSpawnHeroCheckButton(true);
            PlayerControls.Instance.AdjustPlayerControls(false); // This was probably happening too fast which is why it is here.
            CountyPopulation countyPopulation = Globals.Instance.selectedCountyPopulation;
            SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(countyPopulation.location);
            GD.Print("Select County Location: " + countyPopulation.location);

            //GD.Print("It goes to the update description: " + person.firstName);
            populationName.Text = $"{countyPopulation.firstName} {countyPopulation.lastName}";

            DisableUIElements();

            if(countyPopulation.token == null)
            {
                nextActivityTitle.Show();
                nextActivity.Show();
            }

            if(selectCounty.countyData.factionData == Globals.Instance.playerFactionData)
            {
                if (countyPopulation.isArmyLeader == true)
                {
                    armyLeaderRecruitButton.Disabled = true;
                }
                else
                {
                    armyLeaderRecruitButton.Disabled = false;
                }
            }

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
        }

        private void DisableUIElements()
        {
            leaderCheckbox.Disabled = true;
            aideCheckbox.Disabled = true;
            armyLeaderCheckbox.Disabled = true;
            armyLeaderRecruitButton.Disabled = true;
            nextActivityTitle.Hide();
            nextActivity.Hide();
        }

        private void CloseButton()
        {
            Hide();
        }
    }
}