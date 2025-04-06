using Godot;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable SuggestVarOrType_SimpleTypes

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
        
        [Export] private HBoxContainer perksParent;
        [Export] private PackedScene perkLabel;
        [Export] private GridContainer skillsGridContainer;
        
        [Export] private Label interestLabel;
        [Export] private Label preferredWorkLabel;
        [Export] private Label currentActivityLabel;

        [Export] public VBoxContainer InventoryAndSubordinatesInventoryVBoxContainer;
        [Export] public InventoryVBoxContainer inventoryVBoxContainer;
        [Export] public SubordinatesVBoxContainer subordinatesVBoxContainer;
        [Export] private Button aideRecruitButton;
        [Export] private Button armyLeaderRecruitButton;
        [Export] private RecruitHeroConfirmationPanelContainer heroRecruitmentConfirmPanel;

        private readonly List<Label> skillLabelsList = [];
        public PopulationData populationData;

        public bool heroButtonClicked; // If the player has clicked a hero from the list below the countyinfo panel.

        public override void _Ready()
        {
            Instance = this;
            GetSkillLabels();
            ConnectRecruitmentButtonsSignals();
        }

        private void ConnectRecruitmentButtonsSignals()
        {
            aideRecruitButton.Pressed += () => OpenConfirmationPanel(false); 
            armyLeaderRecruitButton.Pressed += () => OpenConfirmationPanel(true); 
        }
        
        private void OpenConfirmationPanel(bool armyLeaderRecruited)
        {
            heroRecruitmentConfirmPanel.Show();
            heroRecruitmentConfirmPanel.armyLeaderRecruited = armyLeaderRecruited;
        }

        private void GetSkillLabels()
        {
            foreach (Label label in skillsGridContainer.GetChildren().Cast<Label>())
            {
                skillLabelsList.Add(label);
            }
        }

        private void OnPopulationDescriptionControlVisibilityChanged()
        {
            if (Visible)
            {
                CallDeferred(nameof(UpdateDescriptionInfo));
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
                    PlayerControls.Instance.AdjustPlayerControls(true);
                    Clock.Instance.UnpauseTime();
                }
                else
                {
                    CountyInfoControl.Instance.DisableSpawnHeroCheckButton(false);
                    heroRecruitmentConfirmPanel.Hide();
                    PlayerUICanvas.Instance.populationListUIElement.Show();
                    Clock.Instance.UnpauseTime();
                }
                heroButtonClicked = false;
            }
            CountyInfoControl.Instance.UpdateEverything();
        }

        public void UpdateDescriptionInfo()
        {
            inventoryVBoxContainer.PopulateEquipment(populationData);
            subordinatesVBoxContainer.UpdateNumberOfSubordinates(populationData);
            subordinatesVBoxContainer.UpdateSubordinates(populationData.heroSubordinates);

            CountyInfoControl.Instance.DisableSpawnHeroCheckButton(true);
            PlayerControls.Instance.AdjustPlayerControls(false); // This was probably happening too fast which is why it is here.
            County county = (County)Globals.Instance.countiesParent.GetChild(populationData.location);
            //GD.Print("Select County Location: " + populationData.location);

            //GD.Print("It goes to the update description: " + person.firstName);
            populationName.Text = $"{populationData.firstName} {populationData.lastName}";

            DisableUiElements();

            // If the token is moving and doesn't belong to the player's faction disable the ability to turn
            // it into an Army.
            CheckForArmyRecruitmentButton(county);
            CheckForAideRecruitmentButton();

            CheckForTitles();

            UpdateAttributes(populationData);

            ageLabel.Text = populationData.age.ToString();

            if (populationData.isMale)
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

            if (populationData.currentCountyImprovement != null)
            {
                currentActivityLabel.Text = $"{Tr(populationData.GetActivityName())} " +
                    $"{Tr(populationData.currentCountyImprovement.GetCountyImprovementName())}";
            }
            else
            {
                currentActivityLabel.Text = $"{Tr(populationData.GetActivityName())}";
            }
        }

        // This was a simplification written by ChatGPT.
        /// <summary>
        /// Check to show if the Army Recruitment Button is active.  It will be disabled if it is in counties that the
        /// player doesn't own.
        /// </summary>
        /// <param name="county"></param>
        private void CheckForArmyRecruitmentButton(County county)
        {
            if (county.countyData.factionData == Globals.Instance.playerFactionData)
            {
                // If this is just a normal population
                if (populationData.HeroType == AllEnums.HeroType.None 
                    && Globals.Instance.playerFactionData.factionGoods[AllEnums.FactionGoodType.Influence].Amount
                        < Globals.Instance.costOfHero)
                {
                    armyLeaderRecruitButton.Disabled = false;
                    return;
                }

                // If the token is moving then it can't become an army.
                if (populationData.heroToken?.tokenMovement.MoveToken == true)
                {
                    return;
                }

                // If the population is already an Aide the player has already paid for the aide
                // and they can make it an army leader.
                if (populationData.IsThisAnArmy() == false)
                {
                    armyLeaderRecruitButton.Disabled = false;
                    return;
                }
            }
        }

        /// <summary>
        /// If the the player's faction doesn't have enough influence or the population is already a hero
        /// the button is disabled.
        /// </summary>
        private void CheckForAideRecruitmentButton()
        {
            if (Globals.Instance.playerFactionData.factionGoods[AllEnums.FactionGoodType.Influence].Amount
                < Globals.Instance.costOfHero || populationData.isHero == true)
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
            interestLabel.Text = Tr(populationData.interestData.name);
        }

        private void UpdatePreferredWork()
        {
            preferredWorkLabel.Text = Tr(populationData.preferredSkill.skillName);
        }

        // All perks are known for now, but eventually we want the player not to know all of their population's perks.
        private void UpdatePerks()
        {
            ClearPerks();
            if (populationData.perks.Count < 1)
            {
                Label perksLabel = (Label)perkLabel.Instantiate();
                perksParent.AddChild(perksLabel);
            }
            else
            {
                foreach (KeyValuePair<AllEnums.Perks, PerkData> keyValuePair in populationData.perks)
                {
                    Label perksLabel = (Label)perkLabel.Instantiate();
                    perksLabel.Text = Tr(keyValuePair.Value.perkName);
                    perksParent.AddChild(perksLabel);
                }
            }
        }

        private void ClearPerks()
        {
            foreach (Label label in perksParent.GetChildren().Cast<Label>().Skip(0))
            {
                label.QueueFree();
            }
        }

        private void CheckForTitles()
        {
            //GD.Print("Check for Titles! " + populationData.IsArmyLeader);
            switch (populationData.HeroType)
            {
                case AllEnums.HeroType.FactionLeader:
                    leaderTitleButton.Disabled = false;
                    return;
                case AllEnums.HeroType.Aide:
                    aideTitleButton.Disabled = false;
                    return;
                case AllEnums.HeroType.FactionLeaderArmyLeader:
                case AllEnums.HeroType.ArmyLeader:
                    armyLeaderTitleButton.Disabled = false;
                    return;
            }
        }


        private void UpdateAttributes(PopulationData population)
        {
            physicalStrengthLabel.Text = population.attributes[AllEnums.Attributes.PhysicalStrength].attributeLevel.ToString();
            agilityLabel.Text = population.attributes[AllEnums.Attributes.Agility].attributeLevel.ToString();
            enduranceLabel.Text = population.attributes[AllEnums.Attributes.Endurance].attributeLevel.ToString();
            intelligenceLabel.Text = population.attributes[AllEnums.Attributes.Intelligence].attributeLevel.ToString();
            mentalStrengthLabel.Text = population.attributes[AllEnums.Attributes.MentalStrength].attributeLevel.ToString();
            awarenessLabel.Text = population.attributes[AllEnums.Attributes.Awareness].attributeLevel.ToString();
            charismaLabel.Text = population.attributes[AllEnums.Attributes.Charisma].attributeLevel.ToString();
            looksLabel.Text = population.attributes[AllEnums.Attributes.Looks].attributeLevel.ToString();

            loyaltyAttributeLabel.Text = population.LoyaltyAdjusted.ToString();
        }

        private void DisableUiElements()
        {
            leaderTitleButton.Disabled = true;
            aideTitleButton.Disabled = true;
            armyLeaderTitleButton.Disabled = true;
            armyLeaderRecruitButton.Disabled = true;
        }

        private void UpdateSkills()
        {
            for (int i = 0; i < populationData.skills.Count; i++)
            {
                AllEnums.Skills skillNumber = (AllEnums.Skills)i;
                skillLabelsList[i].Text = $"{Tr(populationData.skills[skillNumber].skillName)} {populationData.skills[skillNumber].skillLevel}";
            }
        }

        private void CloseButton()
        {
            Hide();
        }
    }
}