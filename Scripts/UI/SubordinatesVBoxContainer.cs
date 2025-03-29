using Godot;
using System;
using System.Linq;

namespace PlayerSpace;

public partial class SubordinatesVBoxContainer : VBoxContainer
{
    private PopulationData populationData;
    [Export] private Label numberOfSubordinatesLabel;
    [Export] private SpinBox numberOfSubordinatesSpinbox;
    [Export] private VBoxContainer subordinatesListVBoxContainer;
    [Export] private PackedScene subordinateButtonPackedScene;

    public void UpdateNumberOfSubordinates(PopulationData populationData)
    {
        // When the population descrition control is visible it runs this method which sets the populationData
        // in this script.
        this.populationData = populationData;
        numberOfSubordinatesLabel.Text = $"{Tr("PHRASE_NUMBER_OF_SUBORDINATES")}: {populationData.heroSubordinates.Count} /";
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
            SubordinateButton subordinateButton = (SubordinateButton)subordinateButtonPackedScene.Instantiate();
            subordinatesListVBoxContainer.AddChild(subordinateButton);
            subordinateButton.Text = populationData.GetFullName();
            subordinateButton.populationData = populationData;
            subordinateButton.Pressed += () => OpenSubordinateDescriptionPanel(populationData);
        }
    }

    private void OpenSubordinateDescriptionPanel(PopulationData populationData)
    {
        PopulationDescriptionControl.Instance.populationData = populationData;
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
