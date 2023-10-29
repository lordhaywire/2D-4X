using Godot;

namespace PlayerSpace
{
    public partial class PopulationDescriptionUIElement : MarginContainer
    {
        [Export] private Label populationName;
        [Export] private Label age;
        [Export] private Label sex;
        [Export] private Label constructionSkill;
        [Export] private Label currentActivity;
        [Export] private Label nextActivity;

        private void OnVisibilityChange()
        {
            if(Visible == true)
            {
                
                CallDeferred("UpdateDescriptionInfo");
            }
            else
            {
                Globals.Instance.selectedCountyPopulation = null;
                Globals.Instance.playerControlsEnabled = true;
                Clock.Instance.UnpauseTime();
            }
        }

        private void UpdateDescriptionInfo()
        {
            Clock.Instance.PauseTime(); // This is just in here for now, even though it sucks.
            Globals.Instance.playerControlsEnabled = false; // This too.

            CountyPopulation person = Globals.Instance.selectedCountyPopulation;
            GD.Print("It goes to the update description: " + person.firstName);
            populationName.Text = $"{person.firstName} {person.lastName}";
            age.Text = person.age.ToString();
            constructionSkill.Text = person.constructionSkill.ToString();
            currentActivity.Text = person.currentActivity.ToString();
            nextActivity.Text = person.nextActivity.ToString();
            if(person.isMale)
            {
                sex.Text = "Male";
            }
            else
            {
                sex.Text = "Female";
            }
        }
        private void CloseButton()
        {
            Hide();
        }
    }
}