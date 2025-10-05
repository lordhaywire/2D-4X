using Godot;
using System.Collections.Generic;
using System.Linq;
using AutoloadSpace;

namespace PlayerSpace;

public partial class FactionControl : Control
{
    public static FactionControl Instance { get; private set; }

    [Export] private PackedScene deadPersonHBoxContainer;
    [Export] private VBoxContainer deadPeopleParentVBoxContainer;
    [Export] private Button closeButton;
    public override void _Ready()
    {
        Instance = this;
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        VisibilityChanged += OnFactionControlVisibilityChanged;
        closeButton.Pressed += OnCloseButtonPressed;
    }

    private void OnFactionControlVisibilityChanged()
    {
        if (Visible)
        {
            PlayerControls.Instance.AdjustPlayerControls(false);
            Clock.Instance.PauseTime();
            GenerateDeadPeopleList();
        }
        else
        {
            PlayerControls.Instance.AdjustPlayerControls(true);
            Clock.Instance.UnpauseTime();
        }
    }

    private void GenerateDeadPeopleList()
    {
        ClearDeadPeopleList();
        foreach (PopulationData deadPopulationData in Autoload.Instance.playerFactionData.allDeadPeopleList)
        {
            CountyData countyData = Globals.Instance.GetCountyDataFromLocationId(deadPopulationData.Location);
            DeadPeopleHBoxContainer deadPeopleContainer = (DeadPeopleHBoxContainer)deadPersonHBoxContainer.Instantiate();
            deadPeopleParentVBoxContainer.AddChild(deadPeopleContainer);
            deadPeopleContainer.deadPersonNameLabel.Text = deadPopulationData.GetFullName();
            deadPeopleContainer.deadPersonLocationLabel.Text = countyData.countyName;
            deadPeopleContainer.deadPersonCauseOfDeathLabel.Text = deadPopulationData.causeOfDeath.ToString();
        }
    }

    private void ClearDeadPeopleList()
    {
        foreach (HBoxContainer hBoxContainer in deadPeopleParentVBoxContainer.GetChildren().Skip(1))
        {
            hBoxContainer.QueueFree();
        }
    }
    private void OnCloseButtonPressed()
    {
        Hide();
    }
}