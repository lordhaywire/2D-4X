using Godot;

namespace PlayerSpace
{
	public partial class BattleLogControl : Control
	{
		[Export] private PackedScene combatLogScene;
		[Export] private VBoxContainer leftVboxContainer;
		[Export] private VBoxContainer rightVboxContainer;

		private void CloseButton()
		{
			Hide();
		}
	}
}