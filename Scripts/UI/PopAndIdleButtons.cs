using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class PopAndIdleButtons : Node
    {
        [Export] private MarginContainer populationListMarginContainer;
        [Export] private VBoxContainer populationListParent;
        [Export] private PackedScene populationRowButton;
        [Export] private Label populationListTitle;
        private void OnButtonClick(bool idleWorkersButton)
        {
            populationListMarginContainer.Show();
            if (idleWorkersButton == false)
            {
                populationListTitle.Text = AllText.Titles.POPLIST;
                GeneratePopulationRows(Globals.Instance.selectedCountyData.heroCountyPopulation);
                GeneratePopulationRows(Globals.Instance.selectedCountyData.countyPopulation);
            }
            else
            {
                populationListTitle.Text = AllText.Titles.IDLELIST;
            }
        }

        private void GeneratePopulationRows(List<CountyPopulation> countyPopulation)
        {
            foreach (CountyPopulation person in countyPopulation)
            {
                Button populationRow = (Button)populationRowButton.Instantiate();                
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
            }
        }

        private void CloseButton()
        {
            //GD.Print("Population List Parent Child Count:" + populationListParent.GetChildCount());
            for (int i = 2; i < populationListParent.GetChildCount(); i++)
            {
                populationListParent.GetChild(i).QueueFree();
            }
            populationListMarginContainer.Hide();
        }
    }
}