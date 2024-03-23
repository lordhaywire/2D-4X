using Godot;

namespace PlayerSpace
{
    public partial class PopulationDescriptionControl : Control
    {
        public static PopulationDescriptionControl Instance { get; private set; }

        [Export] private Label populationName;
        [Export] private Button leaderTitleButton;
        [Export] private Button aideTitleButton;
        [Export] private Button armyLeaderTitleButton;
        [Export] private Label physicalStrengthLabel;
        [Export] private Label agilityLabel;
        [Export] private Label enduranceLabel;
        [Export] private Label intelligenceLabel;
        [Export] private Label mentalStrengthLabel;
        [Export] private Label awarenessLabel;
        [Export] private Label charismaLabel;
        [Export] private Label looksLabel;
        [Export] private Label loyaltyAttributeLabel;
        [Export] private Label ageLabel;
        [Export] private Label sexLabel;
        [Export] private Label firstPerkLabel;
        [Export] private Label noPerksLabel;
        [Export] private Label coolSkillLabel;
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
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(countyPopulation.location);
            GD.Print("Select County Location: " + countyPopulation.location);

            //GD.Print("It goes to the update description: " + person.firstName);
            populationName.Text = $"{countyPopulation.firstName} {countyPopulation.lastName}";

            DisableUIElements();

            if (countyPopulation.token == null)
            {
                nextActivityTitle.Show();
                nextActivityLabel.Show();
            }

            // If the token is moving and doesn't belong to the player's faction disable the ability to turn
            // it into an Army.
            CheckForArmyRecruitmentButton(selectCounty);

            CheckForTitles();
            
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

        private void CheckForTitles()
        {
            GD.Print("Check for Titles! " + countyPopulation.IsArmyLeader);

            if (countyPopulation.isFactionLeader)
            {
                leaderTitleButton.Disabled = false;
            }
            if (countyPopulation.isAide)
            {
                aideTitleButton.Disabled = false;
            }
            if (countyPopulation.IsArmyLeader)
            {
                GD.Print("Army Leader is true!");
                armyLeaderTitleButton.Disabled = false;
            }
        }

        /*
        private void CheckForTitles()
        {
            GD.Print("Check for Titles! " + countyPopulation.IsArmyLeader);
            switch (countyPopulation)
            {
                case { isFactionLeader: true }:
                    leaderCheckbox.Disabled = false;
                    break;

                case { isAide: true }:
                    aideCheckbox.Disabled = false;
                    break;

                case { IsArmyLeader: true }:
                    GD.Print("Army Leader is true!");
                    armyLeaderTitleButton.Disabled = false;
                    break;
            }
        }
        */

        // This means nothing to me.  This was a simplification written by ChatGPT.
        private void CheckForArmyRecruitmentButton(County selectCounty)
        {
            bool isPlayerFaction = selectCounty.countyData.factionData == Globals.Instance.playerFactionData;
            bool isTokenMoving = countyPopulation.token?.tokenMovement.MoveToken ?? false;

            armyLeaderRecruitButton.Disabled = countyPopulation.IsArmyLeader || (isPlayerFaction && isTokenMoving);
        }

        private void UpdateAttributes(CountyPopulation countyPopulation)
        {
            loyaltyAttributeLabel.Text = countyPopulation.loyaltyAttribute.ToString();
        }

        private void DisableUIElements()
        {
            leaderTitleButton.Disabled = true;
            aideTitleButton.Disabled = true;
            armyLeaderTitleButton.Disabled = true;
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
            coolSkillLabel.Text = $"Cool: {countyPopulation.coolSkill}";
            constructionSkillLabel.Text = $"Contruction: {countyPopulation.constructionSkill}";
            rifleSkillLabel.Text = $"Rifle: {countyPopulation.rifleSkill}";
        }
    }
}