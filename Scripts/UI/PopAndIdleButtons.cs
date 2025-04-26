using Godot;

namespace PlayerSpace;

public partial class PopAndIdleButtons : VBoxContainer
{
    [Export] private MarginContainer populationListMarginContainer;
    [Export] private Button populationListButton;
    [Export] private Button visitorListButton;

    public override void _Ready()
    {
        populationListButton.Pressed += () => PopAndIdleOnButtonPressed(false);
        visitorListButton.Pressed += () => PopAndIdleOnButtonPressed(true);
    }

    private void PopAndIdleOnButtonPressed(bool isVisitorList)
    {
        Globals.Instance.isVisitorList = isVisitorList;
        populationListMarginContainer.Show();
    }

    private void CloseButton()
    {
        populationListMarginContainer.Hide();
    }
}