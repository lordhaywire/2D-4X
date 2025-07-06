using Godot;

namespace StartMenu;

public partial class StartMenuCanvasLayer : CanvasLayer
{
    [Export] private Button startButton;
    [Export] private Button loadButton;
    [Export] private Button exitButton;

    private string gameGenerationSceneDirectory = "res://Scenes/GameGeneration.tscn";

    public override void _Ready()
    {
        startButton.Pressed += StartGame;
        loadButton.Pressed += LoadGame;
        exitButton.Pressed += ExitGame;
    }

    private void StartGame()
    {
        GD.Print("Start Game");
        GetTree().ChangeSceneToFile(gameGenerationSceneDirectory);
    }
    
    private void LoadGame()
    {
        throw new System.NotImplementedException();
    }
    private void ExitGame()
    {
        GD.Print("Exit Game");
        GetTree().Quit();
    }
}
