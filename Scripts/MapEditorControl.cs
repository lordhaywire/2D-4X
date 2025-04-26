using Godot;

namespace PlayerSpace;

public partial class MapEditorControl : Control
{
    [Export] private PackedScene mapEditorScene;
    private void ButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/MapEditor.tscn");
    }
}