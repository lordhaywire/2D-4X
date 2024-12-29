using Godot;
namespace PlayerSpace;

public partial class GameStart : Node
{
	public override void _Ready()
	{
        // This is so that getter setters will start getting effected after the game has started.
        // I don't think this works.
        AutoLoad.Instance.gameStarted = true;

    }
}
