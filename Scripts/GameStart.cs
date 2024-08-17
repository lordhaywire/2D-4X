using Godot;
namespace PlayerSpace;

public partial class GameStart : Node
{
	public override void _Ready()
	{
        // This is so that getter setters will start getting effected after the game has started.
        AutoLoad.Instance.gameStarted = true;
    }
}
