using Godot;
using System.Linq;

namespace PlayerSpace;

public partial class SubordinatesVBoxContainer : VBoxContainer
{
    private PopulationData populationData;
    [Export] private Label numberOfSubordinatesLabel;
    [Export] private SpinBox numberOfSubordinatesSpinBox;
    [Export] private VBoxContainer subordinatesListVBoxContainer;
    [Export] private PackedScene subordinateButtonPackedScene;

    public void UpdateNumberOfSubordinates(PopulationData population)
    {
        // When the population description control is visible it runs this method which sets the populationData
        // in this script.
        populationData = population;
        numberOfSubordinatesLabel.Text = $"{Tr("PHRASE_NUMBER_OF_SUBORDINATES")}: {population.heroSubordinates.Count} /";
        numberOfSubordinatesSpinBox.Value = population.numberOfSubordinatesWanted;
    }

    private void NumberOfSubordinatesValueChanged(float value)
    {
        populationData.numberOfSubordinatesWanted = (int)value;
        GD.Print("Number of Subordinates: " + populationData.numberOfSubordinatesWanted);
        Recruiter.CheckForRecruitingActivity(populationData);
        PopulationDescriptionControl.Instance.UpdateDescriptionInfo();
    }

    public void UpdateSubordinates(Godot.Collections.Array<PopulationData> heroSubordinates)
    {
        ClearSubordinateButtons();
        foreach (PopulationData population in heroSubordinates) 
        {
            SubordinateButton subordinateButton = (SubordinateButton)subordinateButtonPackedScene.Instantiate();
            subordinatesListVBoxContainer.AddChild(subordinateButton);
            subordinateButton.Text = population.GetFullName();
            subordinateButton.populationData = population;
            subordinateButton.Pressed += () => OpenSubordinateDescriptionPanel(population);
        }
    }

    private void OpenSubordinateDescriptionPanel(PopulationData population)
    {
        PopulationDescriptionControl.Instance.populationData = population;
        PopulationDescriptionControl.Instance.UpdateDescriptionInfo();
    }

    private void ClearSubordinateButtons()
    {
        foreach (Button subordinateButton in subordinatesListVBoxContainer.GetChildren().Cast<Button>())
        {
            subordinateButton.QueueFree();
        }
    }
}
