using Godot;
using System.Linq;

namespace PlayerSpace;

public partial class SubordinatesVBoxContainer : VBoxContainer
{
    private PopulationData populationData;
    [Export] private SpinBox numberOfSubordinatesSpinbox;
    [Export] private VBoxContainer subordinatesListVBoxContainer;
    [Export] private PackedScene subordinateButtonPackedScene;

    public void UpdateNumberOfSubordinates(PopulationData populationData)
    {
        // When the population descrition control is visible it runs this method which sets the populationData
        // in this script.
        this.populationData = populationData;
        numberOfSubordinatesSpinbox.Value = populationData.numberOfSubordinates;
    }

    public void NumberOfSubordinatesValueChanged(float value)
    {
        populationData.numberOfSubordinates = (int)value;
        GD.Print("Number of Subordinates: " + populationData.numberOfSubordinates);
    }

    public void UpdateSubordinates(Godot.Collections.Array<PopulationData> heroSubordinates)
    {
        ClearSubordinateButtons();
        foreach (PopulationData populationData in heroSubordinates) 
        {
            Button subordinateButton = (Button)subordinateButtonPackedScene.Instantiate();
            subordinatesListVBoxContainer.AddChild(subordinateButton);
            subordinateButton.Text = populationData.GetFullName();
        }
    }

    private void ClearSubordinateButtons()
    {
        foreach (Button subordinateButton in subordinatesListVBoxContainer.GetChildren().Cast<Button>())
        {
            subordinateButton.QueueFree();
        }
    }
}
