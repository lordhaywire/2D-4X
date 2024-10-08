using Godot;
using System;

namespace PlayerSpace
{
    public partial class PopulationListUIElement : MarginContainer
    {
        [Export] private VBoxContainer populationListParent;
        [Export] private PackedScene populationRowButtonPackedScene;
        [Export] private Label populationListTitle;

        private void OnVisibilityChanged()
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
                    string populationString = "WORD_POPULATION";
                    populationListTitle.Text = $"{Globals.Instance.SelectedLeftClickCounty.countyData.countyName} {Tr(populationString)}";
                        
                    
                    
                    GeneratePopulationRows(Globals.Instance.SelectedLeftClickCounty.countyData.herosInCountyList);
                    GeneratePopulationRows(Globals.Instance.SelectedLeftClickCounty.countyData.countyPopulationList);
                }
                else
                {
                    populationListTitle.Text = $"{Globals.Instance.SelectedLeftClickCounty.countyData.countyName} " +
                        AllText.Titles.VISITORLIST;
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

        private void GeneratePopulationRows(Globals.ListWithNotify<CountyPopulation> countyPopulation)
        {
            foreach (CountyPopulation person in countyPopulation)
            {
                PopulationRowButton populationRow = (PopulationRowButton)populationRowButtonPackedScene.Instantiate();

                populationRow.populationNameLabel.Text = $"{person.firstName} {person.lastName}";
                populationRow.ageLabel.Text = person.age.ToString();
                if (person.isMale == true)
                {
                    populationRow.sexLabel.Text = "Male";
                }
                else
                {
                    populationRow.sexLabel.Text = "Female";
                }
                UpdateUnHelpfulPerk(populationRow, person);
                UpdateAttributes(populationRow, person);
                UpdateSkills(populationRow, person);
                UpdateActivities(populationRow, person);

                populationListParent.AddChild(populationRow);
                PopulationRowButton populationRowButton = populationRow;
                populationRowButton.countyPopulation = person;
            }
        }


        // This are all broken.
        // This is where we would us a method to return the name.
        private static void UpdateActivities(PopulationRowButton populationRow, CountyPopulation countyPopulation)
        {
            // This sets their current activity then checks to see if Building, Work, or Research is null and if it isn't
            // then it adds where to the end of the label.  If they are all null then it puts nothing.
            populationRow.currentActivityLabel.Text = countyPopulation.GetActivityName();
            string currentWhere = countyPopulation.currentCountyImprovement?.improvementName
                ?? countyPopulation.currentResearchItemData?.researchName
                ?? string.Empty;
            populationRow.currentActivityLabel.Text += $" {currentWhere}";
        }

        // This will not be shown in to the player eventually.  It is just shown right now for testing.
        // We might need a second testing bar to show these things later.
        // This should be not equal to null.
        private static void UpdateUnHelpfulPerk(PopulationRowButton populationRow, CountyPopulation countyPopulation)
        {
            if (countyPopulation.CheckForPerk(AllEnums.Perks.Unhelpful) == true)
            {
                populationRow.UnhelpfulLabel.Text = $"{countyPopulation.perks[AllEnums.Perks.Unhelpful].perkName}";

            }
            else
            {
                populationRow.UnhelpfulLabel.Text = "Helpful";
            }
        }


        private static void UpdateAttributes(PopulationRowButton populationRow, CountyPopulation person)
        {
            populationRow.loyaltyAttributeLabel.Text = $"{person.LoyaltyAdjusted}";
        }
        private static void UpdateSkills(PopulationRowButton populationRow, CountyPopulation person)
        {
            for (int i = 0; i < person.skills.Count; i++)
            {
                AllEnums.Skills skillNumber = (AllEnums.Skills)i;
                populationRow.skillLabels[i].Text = $"{person.skills[skillNumber].skillLevel}";
            }
        }
    }
}