using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;

namespace PlayerSpace
{
    public partial class PopulationDescriptionControl : Control
    {
        public static PopulationDescriptionControl Instance { get; private set; }

        [Export] private HBoxContainer perksParent;
        [Export] private PackedScene perkLabel;

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
        [Export] private Label[] skillLabels;

        [Export] private Label interestLabel;
        [Export] private Label preferredWorkLabel;
        [Export] private Label currentActivityLabel;

        [Export] private Button aideRecruitButton;
        [Export] private Button armyLeaderRecruitButton;
        [Export] private PanelContainer heroRecruitmentConfirmPanel;

        public CountyPopulation countyPopulation;

        public bool heroButtonClicked; // If the player has clicked a hero from the list below the countyinfo panel.

        public override void _Ready()
        {
            Instance = this;
        }

        private void OnVisibilityChanged()
        {
            if (Visible)
            {
                CallDeferred("UpdateDescriptionInfo");
                Clock.Instance.PauseTime();
                CountyInfoControl.Instance.countyImprovementsPanelControl.Hide();
                CountyInfoControl.Instance.populationListMarginContainer.Hide();
            }
            else
            {
                if (heroButtonClicked)
                {
                    CountyInfoControl.Instance.DisableSpawnHeroCheckButton(false);
                    heroRecruitmentConfirmPanel.Hide();
                    //PlayerUICanvas.Instance.populationListUIElement.Show();
                    PlayerControls.Instance.AdjustPlayerControls(true);
                    Clock.Instance.UnpauseTime();
                }
                else
                {
                    CountyInfoControl.Instance.DisableSpawnHeroCheckButton(false);
                    heroRecruitmentConfirmPanel.Hide();
                    PlayerUICanvas.Instance.populationListUIElement.Show();
                    //PlayerControls.Instance.AdjustPlayerControls(true);
                    Clock.Instance.UnpauseTime();
                }
                heroButtonClicked = false;
            }
        }

        // I should probably rewrite this so it is less of a mess.
        public void UpdateDescriptionInfo()
        {
            CountyInfoControl.Instance.DisableSpawnHeroCheckButton(true);
            PlayerControls.Instance.AdjustPlayerControls(false); // This was probably happening too fast which is why it is here.
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(countyPopulation.location);
            //GD.Print("Select County Location: " + countyPopulation.location);

            //GD.Print("It goes to the update description: " + person.firstName);
            populationName.Text = $"{countyPopulation.firstName} {countyPopulation.lastName}";

            DisableUIElements();

            // If the token is moving and doesn't belong to the player's faction disable the ability to turn
            // it into an Army.
            CheckForArmyRecruitmentButton(selectCounty);

            CheckForTitles();

            UpdateAttributes(countyPopulation);

            ageLabel.Text = countyPopulation.age.ToString();

            if (countyPopulation.isMale)
            {
                sexLabel.Text = "WORD_MALE";
            }
            else
            {
                sexLabel.Text = "WORD_FEMALE";
            }

            UpdatePerks();
            UpdateInterest();
            UpdatePreferredWork();
            UpdateSkills();

            if (countyPopulation.currentCountyImprovement != null)
            {
                currentActivityLabel.Text = $"{Tr(countyPopulation.GetActivityName())} " +
                    $"{Tr(countyPopulation.currentCountyImprovement.improvementName)}";
            }
            else
            {
                currentActivityLabel.Text = $"{Tr(countyPopulation.GetActivityName())}";
            }

            if (Globals.Instance.playerFactionData.factionResources[AllEnums.FactionResourceType.Influence].amount 
                < Globals.Instance.costOfHero || countyPopulation.isHero == true)
            {
                aideRecruitButton.Disabled = true;
            }
            else
            {
                aideRecruitButton.Disabled = false;
            }
        }

        private void UpdateInterest()
        {
            interestLabel.Text = Tr(countyPopulation.interest.name);
        }

        private void UpdatePreferredWork()
        {
            preferredWorkLabel.Text = Tr(countyPopulation.preferredSkill.skillName);
        }

        // All perks are known for now, but eventually we want the player not to know all of their population's perks.
        private void UpdatePerks()
        {
            ClearPerks();
            if(countyPopulation.perks.Count < 1)
            {
                Label perksLabel = (Label)perkLabel.Instantiate();
                perksParent.AddChild(perksLabel);
            }
            else
            {
                foreach (KeyValuePair<AllEnums.Perks, PerkData> keyValuePair in countyPopulation.perks)
                {
                    Label perksLabel = (Label)perkLabel.Instantiate();
                    perksLabel.Text = Tr(keyValuePair.Value.perkName);
                    perksParent.AddChild(perksLabel);
                }
            }
        }

        private void ClearPerks()
        {
            foreach(Label label in perksParent.GetChildren().Cast<Label>().Skip(0))
            {
                label.QueueFree();
            }
        }

        private void CheckForTitles()
        {
            //GD.Print("Check for Titles! " + countyPopulation.IsArmyLeader);

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
                //GD.Print("Army Leader is true!");
                armyLeaderTitleButton.Disabled = false;
            }
        }

        // This means nothing to me.  This was a simplification written by ChatGPT.
        private void CheckForArmyRecruitmentButton(County selectCounty)
        {
            bool isPlayerFaction = selectCounty.countyData.factionData == Globals.Instance.playerFactionData;
            bool isTokenMoving = countyPopulation.token?.tokenMovement.MoveToken ?? false;

            armyLeaderRecruitButton.Disabled = countyPopulation.IsArmyLeader || (isPlayerFaction && isTokenMoving);
        }

        private void UpdateAttributes(CountyPopulation countyPopulation)
        {
            physicalStrengthLabel.Text = countyPopulation.attributes[AllEnums.Attributes.PhysicalStrength].attributeLevel.ToString();
            agilityLabel.Text = countyPopulation.attributes[AllEnums.Attributes.Agility].attributeLevel.ToString();
            enduranceLabel.Text = countyPopulation.attributes[AllEnums.Attributes.Endurance].attributeLevel.ToString();
            intelligenceLabel.Text = countyPopulation.attributes[AllEnums.Attributes.Intelligence].attributeLevel.ToString();
            mentalStrengthLabel.Text = countyPopulation.attributes[AllEnums.Attributes.MentalStrength].attributeLevel.ToString();
            awarenessLabel.Text = countyPopulation.attributes[AllEnums.Attributes.Awareness].attributeLevel.ToString();
            charismaLabel.Text = countyPopulation.attributes[AllEnums.Attributes.Charisma].attributeLevel.ToString();
            looksLabel.Text = countyPopulation.attributes[AllEnums.Attributes.Looks].attributeLevel.ToString();

            loyaltyAttributeLabel.Text = countyPopulation.LoyaltyAdjusted.ToString();
        }

        private void DisableUIElements()
        {
            leaderTitleButton.Disabled = true;
            aideTitleButton.Disabled = true;
            armyLeaderTitleButton.Disabled = true;
            armyLeaderRecruitButton.Disabled = true;
        }

        private void UpdateSkills()
        {
            for (int i = 0; i < countyPopulation.skills.Count; i++)
            {
                AllEnums.Skills skillNumber = (AllEnums.Skills)i;
                skillLabels[i].Text = $"{Tr(countyPopulation.skills[skillNumber].skillName)} {countyPopulation.skills[skillNumber].skillLevel}";
            }
        }

        private void CloseButton()
        {
            Hide();
        }
    }
}