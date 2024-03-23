using Godot;
using System;
using System.Collections.Generic;

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
            if (Visible == true)
            {
                CountyInfoControl.Instance.DisableSpawnHeroCheckButton(true);
                CountyInfoControl.Instance.populationDescriptionControl.Hide();
                CountyInfoControl.Instance.countyImprovementsPanelControl.Hide();
                PlayerUICanvas.Instance.BattleLogControl.Hide();

                PlayerControls.Instance.AdjustPlayerControls(false);
                Clock.Instance.PauseTime();
                
                if(Globals.Instance.isVisitorList == false)
                {
                    populationListTitle.Text = $"{Globals.Instance.CurrentlySelectedCounty.countyData.countyName} " +
                        $"{AllText.Titles.POPLIST}";
                    GeneratePopulationRows(Globals.Instance.CurrentlySelectedCounty.countyData.herosInCountyList);
                    GeneratePopulationRows(Globals.Instance.CurrentlySelectedCounty.countyData.countyPopulationList);
                }
                else
                {
                    populationListTitle.Text = $"{Globals.Instance.CurrentlySelectedCounty.countyData.countyName} " + 
                        AllText.Titles.VISITORLIST;
                    GeneratePopulationRows(Globals.Instance.CurrentlySelectedCounty.countyData.visitingHeroList);
                }
            }
            else
            {
                CountyInfoControl.Instance.DisableSpawnHeroCheckButton(false);
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

                UpdateAttributes(populationRow, person);
                UpdateSkills(populationRow, person);

                if(person.currentImprovement != null)
                {
                    populationRow.currentActivityLabel.Text = $"{person.currentActivity} {person.currentImprovement.improvementName}";
                }
                else
                {
                    populationRow.currentActivityLabel.Text = person.currentActivity;
                }
                if(person.nextImprovement != null)
                {
                    populationRow.nextActivityLabel.Text = $"{person.nextActivity} {person.nextImprovement.improvementName}";
                }
                else
                {
                    populationRow.nextActivityLabel.Text = person.nextActivity;
                }
                populationListParent.AddChild(populationRow);
                PopulationRowButton populationRowButton = populationRow;
                populationRowButton.countyPopulation = person;
            }
        }
        private void UpdateAttributes(PopulationRowButton populationRow, CountyPopulation person)
        {
            populationRow.loyaltyAttributeLabel.Text = $"{person.loyaltyAttribute}";
        }
        private void UpdateSkills(PopulationRowButton populationRow, CountyPopulation person)
        {
            populationRow.coolSkillLabel.Text = $"{person.coolSkill}";
            populationRow.constructionSkillLabel.Text = $"{person.constructionSkill}";
            populationRow.rifleSkillLabel.Text = $"{person.rifleSkill}";
        }
    }
}