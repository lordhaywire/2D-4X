using Godot;
using PlayerSpace;
using System.Collections.Generic;

public partial class ResearchControl : Control
{
	private List<ResearchItem> researchItems = new();
	private string researchItemsPath = "res://ResearchItems/ResearchItems.txt";
	public void ShowResearchPanel()
	{
		GD.Print("Show the research panel!");
		Show();
        Globals.Instance.playerControlsEnabled = false;
    }

	public void CloseButton()
	{
		Hide();
		Globals.Instance.playerControlsEnabled = true;
	}
	public override void _Ready()
	{
		researchItems.Add(new ResearchItem("Test", "This is a test item", false, false));

		// Reading a file from the project folders.
		/*
		using var file = FileAccess.Open(researchItemsPath, FileAccess.ModeFlags.Read);
		string content = file.GetAsText();
		GD.PrintRich("Conmtent: " + content);
		

		// Writing it, partial.
		if(FileAccess.FileExists(researchItemsPath) == true)
		{
			using var file = FileAccess.Open(researchItemsPath, FileAccess.ModeFlags.Write);
			var testJson = Json
			file.StoreVar(researchItems, true);
			//GD.PrintRich("[rainbow]Whatevs");
			

		}
		*/
	}


}
