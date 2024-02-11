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
        [Export] private Label physicalStrengthLabel;
        [Export] private Label agilityLabel;
        [Export] private Label enduranceLabel;
        [Export] private Label intelligenceLabel;
        [Export] private Label mentalStrengthLabel;
        [Export] private Label awarenessLabel;
        [Export] private Label charismaLabel;
        [Export] private Label looksLabel;
        [Export] private Label coolLabel;
        [Export] private Label ageLabel;
        [Export] private Label sexLabel;
        [Export] private Label firstPerkLabel;
        [Export] private Label noPerksLabel;
        [Export] private Label constructionSkillLabel;
        [Export] private Label rifleSkillLabel;
        [Export] private Label currentActivityLabel;
        [Export] private Label nextActivityTitle;
        [Export] private Label nextActivityLabel;

        [Export] private Button aideRecruitButton;
        [Export] private Button armyLeaderRecruitButton;
        [Export] private PanelContainer heroRecruitmentConfirmPanel;

        public CountyPopulation countyPopulation;

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
                PlayerControls.Instance.AdjustPlayerControls(true);
                Clock.Instance.UnpauseTime();
            }
        }

        // I should probably rewrite this so it is less of a mess.
        public void UpdateDescriptionInfo()
        {
            CountyInfoControl.Instance.DisableSpawnHeroCheckButton(true);
            PlayerControls.Instance.AdjustPlayerControls(false); // This was probably happening too fast which is why it is here.
            SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(countyPopulation.location);
            GD.Print("Select County Location: " + countyPopulation.location);

            //GD.Print("It goes to the update description: " + person.firstName);
            populationName.Text = $"{countyPopulation.firstName} {countyPopulation.lastName}";

            DisableUIElements();

            if(countyPopulation.token == null)
            {
                nextActivityTitle.Show();
                nextActivityLabel.Show();
            }

            if(selectCounty.countyData.factionData == Globals.Instance.playerFactionData)
            {
                if (countyPopulation.IsArmyLeader == true)
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
            if (countyPopulation.IsArmyLeader == true)
            {
                armyLeaderCheckbox.Disabled = false;
            }

            UpdateAttributes(countyPopulation);

            ageLabel.Text = countyPopulation.age.ToString();

            if (countyPopulation.isMale)
            {
                sexLabel.Text = "Male";
            }
            else
            {
                sexLabel.Text = "Female";
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

            UpdateSkills(countyPopulation);

            if (countyPopulation.currentImprovement != null)
            {
                currentActivityLabel.Text = $"{countyPopulation.currentActivity} {countyPopulation.currentImprovement.improvementName}";
            }
            else
            {
                currentActivityLabel.Text = $"{countyPopulation.currentActivity}";
            }
            if (countyPopulation.nextImprovement != null)
            {
                nextActivityLabel.Text = $"{countyPopulation.nextActivity} {countyPopulation.nextImprovement.improvementName}";

            }
            else
            {
                nextActivityLabel.Text = $"{countyPopulation.nextActivity}";
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

        private void UpdateAttributes(CountyPopulation countyPopulation)
        {
            coolLabel.Text = countyPopulation.coolAttribute.ToString();
        }

        private void DisableUIElements()
        {
            leaderCheckbox.Disabled = true;
            aideCheckbox.Disabled = true;
            armyLeaderCheckbox.Disabled = true;
            armyLeaderRecruitButton.Disabled = true;
            nextActivityTitle.Hide();
            nextActivityLabel.Hide();
        }

        private void CloseButton()
        {
            Hide();
        }

        private void UpdateSkills(CountyPopulation countyPopulation)
        {
            // Skills
            constructionSkillLabel.Text = $"Contruction: {countyPopulation.constructionSkill}";
            rifleSkillLabel.Text = $"Rifle: {countyPopulation.rifleSkill}";
        }
    }
}