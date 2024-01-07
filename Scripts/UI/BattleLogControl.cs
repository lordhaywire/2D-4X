using Godot;


public partial class BattleLogControl : Control
{
	[Export] private PackedScene combatLogScene;
	[Export] private VBoxContainer leftVboxContainer;
	[Export] private VBoxContainer rightVboxContainer;

	public override void _Ready()
	{
	}

	private void CloseButton()
	{
		Hide();
	}
}
