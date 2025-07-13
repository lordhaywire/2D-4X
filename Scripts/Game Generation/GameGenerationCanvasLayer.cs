using Godot;

namespace GameGeneration;
public partial class GameGenerationCanvasLayer : CanvasLayer
{
    public static GameGenerationCanvasLayer Instance { get; private set; }
    
    [Export] private Label statusLabel;

    public override void _Ready()
    {
        Instance = this;
    }

    public void UpdateStatusLabelText(string statusText)
    {
        statusLabel.Text = statusText;
    }
}
