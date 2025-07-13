using Godot;

namespace PlayerSpace;
public partial class SaveGeneratedWorldToDisk : Node
{
    public override void _Ready()
    {
        SaveManager.Instance.SaveGame();
    }
}
