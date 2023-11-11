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
                Button populationRow = (Button)populationRowButtonPrefab.Instantiate();
                Label name = (Label)populationRow.GetChild(0).GetChild(0);
                Label age = (Label)populationRow.GetChild(0).GetChild(1);
                Label sex = (Label)populationRow.GetChild(0).GetChild(2);
                name.Text = $"{person.firstName} {person.lastName}";
                age.Text = person.age.ToString();
                if (person.isMale == true)
                {
                    sex.Text = "Male";
                }
                else
                {
                    sex.Text = "Female";
                }
                populationListParent.AddChild(populationRow);
                PopulationRowButton populationRowButton = (PopulationRowButton)populationRow;
                populationRowButton.countyPopulation = person;
            }
        }
    }
}