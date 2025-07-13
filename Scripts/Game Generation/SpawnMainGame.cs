using Godot;

namespace PlayerSpace;

public partial class SpawnMainGame : Node
{
    [Export] public PackedScene mainGameScene;

    public override void _Ready()
    {
        CallDeferred(nameof(ChangeToMainGame));
    }

    private void ChangeToMainGame()
    {
        GetTree().ChangeSceneToPacked(mainGameScene);
    }
}
