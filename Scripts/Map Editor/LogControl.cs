using Godot;

namespace MapEditorSpace
{
	public partial class LogControl : Control
	{
		public static LogControl Instance { get; set; }

		[Export] public Label centerLogLabel;
		public override void _Ready()
		{
			Instance = this;
		}

		public void UpdateLabel(string text)
		{
			centerLogLabel.Text = text;
            GD.Print(text);
        }
    }
}