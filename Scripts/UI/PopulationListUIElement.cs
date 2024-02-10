using Godot;
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
                PlayerControls.Instance.AdjustPlayerControls(false);
                Clock.Instance.PauseTime();
                
                if(Globals.Instance.isVisitorList == false)
                {
                    populationListTitle.Text = AllText.Titles.POPLIST;
                    GeneratePopulationRows(Globals.Instance.SelectedCountyData.heroCountyPopulation);
                    GeneratePopulationRows(Globals.Instance.SelectedCountyData.countyPopulation);
                }
                else
                {
                    populationListTitle.Text = AllText.Titles.VISITORLIST;
                    GeneratePopulationRows(Globals.Instance.SelectedCountyData.visitingPopulation);
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

        private void GeneratePopulationRows(List<CountyPopulation> countyPopulation)
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
                PopulationRowButton populationRowButton = (PopulationRowButton)populationRow;
                populationRowButton.countyPopulation = person;
            }
        }
    }
}