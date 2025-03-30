using Godot;
using System;

namespace PlayerSpace
{
    public partial class PopulationListUIElement : MarginContainer
    {
        [Export] private VBoxContainer populationListParent;
        [Export] private PackedScene populationRowButtonPackedScene;
        [Export] private Label populationListTitle;

        private void OnPopulationListUIElementVisibilityChanged()
        {
            DestroyPopulationRows(); // Clears out the population, so it doesn't duplicate.
            if (Visible)
            {
                CountyInfoControl.Instance.populationDescriptionControl.Hide();
                CountyInfoControl.Instance.countyImprovementsPanelControl.Hide();
                PlayerUICanvas.Instance.BattleLogControl.Hide();

                PlayerControls.Instance.AdjustPlayerControls(false);
                Clock.Instance.PauseTime();

                if (Globals.Instance.isVisitorList == false)
                {
                    populationListTitle.Text = $"{Globals.Instance.SelectedLeftClickCounty.countyData.countyName} {Tr("WORD_POPULATION")}";
                        
                    GeneratePopulationRows(Globals.Instance.SelectedLeftClickCounty.countyData.heroesInCountyList);
                    GeneratePopulationRows(Globals.Instance.SelectedLeftClickCounty.countyData.populationDataList);
                }
                else
                {
                    populationListTitle.Text = $"{Globals.Instance.SelectedLeftClickCounty.countyData.countyName} " +
                        Tr("WORD_VISITORS");
                    GeneratePopulationRows(Globals.Instance.SelectedLeftClickCounty.countyData.visitingHeroList);
                }
            }
            else
            {
                //CountyInfoControl.Instance.DisableSpawnHeroCheckButton(false);
                PlayerControls.Instance.AdjustPlayerControls(true);
                Clock.Instance.UnpauseTime();
            }
        }

        private void DestroyPopulationRows()
        {
            for (int i = 2; i < populationListParent.GetChildCount(); i++)
            {
                populationListParent.GetChild(i).QueueFree();
            }
        }

        private void GeneratePopulationRows(Godot.Collections.Array<PopulationData> populationData)
        {
            foreach (PopulationData person in populationData)
            {
                PopulationRowButton populationRow = (PopulationRowButton)populationRowButtonPackedScene.Instantiate();

                populationRow.populationNameLabel.Text = $"{person.firstName} {person.lastName}";
                populationRow.ageLabel.Text = person.age.ToString();
                if (person.isMale == true)
                {
                    populationRow.sexLabel.Text = "WORD_MALE";
                }
                else
                {
                    populationRow.sexLabel.Text = "WORD_FEMALE";
                }
                UpdateUnHelpfulPerk(populationRow, person);
                UpdateAttributes(populationRow, person);
                UpdateSkills(populationRow, person);
                UpdateActivities(populationRow, person);

                populationListParent.AddChild(populationRow);
                PopulationRowButton populationRowButton = populationRow;
                populationRowButton.populationData = person;
            }
        }


        // This are all broken.  This are all seems to be working?
        // This is where we would us a method to return the name.
        private static void UpdateActivities(PopulationRowButton populationRow, PopulationData populationData)
        {
            // This sets their current activity then checks to see if Building, Work, or Research is null and if it isn't
            // then it adds where to the end of the label.  If they are all null then it puts nothing.
            populationRow.currentActivityLabel.Text = populationData.GetActivityName();
            string currentWhere = populationData.currentCountyImprovement?.GetCountyImprovementName()
                ?? TranslationServer.Translate(populationData.currentResearchItemData?.researchName)
                ?? string.Empty;
            populationRow.currentActivityLabel.Text += $" {currentWhere}";
        }

        // This will not be shown in to the player eventually.  It is just shown right now for testing.
        // We might need a second testing bar to show these things later.
        // This should be not equal to null.
        private void UpdateUnHelpfulPerk(PopulationRowButton populationRow, PopulationData populationData)
        {
            if (populationData.CheckForPerk(AllEnums.Perks.Unhelpful) == true)
            {
                populationRow.UnhelpfulLabel.Text = $"{Tr(populationData.perks[AllEnums.Perks.Unhelpful].perkName)}";

            }
            else
            {
                populationRow.UnhelpfulLabel.Text = $"{Tr("WORD_HELPFUL")}";
            }
        }


        private static void UpdateAttributes(PopulationRowButton populationRow, PopulationData person)
        {
            populationRow.loyaltyAttributeLabel.Text = $"{person.LoyaltyAdjusted}";
        }
        private static void UpdateSkills(PopulationRowButton populationRow, PopulationData person)
        {
            for (int i = 0; i < person.skills.Count; i++)
            {
                AllEnums.Skills skillNumber = (AllEnums.Skills)i;
                populationRow.skillLabels[i].Text = $"{person.skills[skillNumber].skillLevel}";
            }
        }
    }
}