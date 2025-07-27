using Godot;

namespace PlayerSpace;

public partial class InitialLoadGame : Node
{
    public override void _Ready()
    {
        if (Globals.Instance.loadGameAtStart)
        {
            SaveManager.Instance.saveGameData = null;
            SaveManager.Instance.LoadGameFromJson();
        }
    }
}