using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class PopulationListUIElement : MarginContainer
    {
        [Export] private VBoxContainer populationListParent;
        [Export] private PackedScene populationRowButtonPrefab;
        [Export] private Label populationListTitle;
        
        private void OnVisibilityChange()
        {
            DestroyPopulationRows(); // Clears out the population, so it doesn't duplicate.
            if (Visible == true)
            {
                CountyInfoControl.Instance.DisableSpawnHeroCheckButton(true);
                CountyInfoControl.Instance.populationDescriptionMarginContainer.Hide();
                Globals.Instance.playerControlsEnabled = false;
                Clock.Instance.PauseTime();
                
                populationListTitle.Text = AllText.Titles.POPLIST;
                GeneratePopulationRows(Globals.Instance.selectedCountyData.heroCountyPopulation);
                GeneratePopulationRows(Globals.Instance.selectedCountyData.countyPopulation);
            }
            else
            {
                CountyInfoControl.Instance.DisableSpawnHeroCheckButton(false);
                Globals.Instance.playerControlsEnabled = true;
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
                PopulationRowButton populationRow = (PopulationRowButton)populationRowButtonPrefab.Instantiate();

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
                // This is only going to work while there are two Enums on the list.
                if(person.currentActivity == AllText.Jobs.BUILDING)
                {
                    populationRow.currentActivityLabel.Text = $"{person.currentActivity} {person.currentImprovement.improvementName}";
                }
                else
                {
                    populationRow.currentActivityLabel.Text = person.currentActivity;
                }
                if(person.nextActivity == AllText.Jobs.BUILDING)
                {
                    populationRow.nextActivityLabel.Text = $"{person.nextActivity} {person.currentImprovement.improvementName}";
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