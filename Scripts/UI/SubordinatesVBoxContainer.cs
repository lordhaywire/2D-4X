using Godot;

namespace PlayerSpace;

public partial class SubordinatesVBoxContainer : VBoxContainer
{
    [Export] private SpinBox numberOfSubordinatesSpinbox;

    private PopulationData populationData;
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
}
