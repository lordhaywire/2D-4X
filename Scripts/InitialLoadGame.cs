using Godot;

namespace PlayerSpace;
public partial class InitialLoadGame : Node
{
    public override void _Ready()
    {
        SaveManager.Instance.LoadGame();
    }
}
