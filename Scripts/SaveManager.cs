using Godot;

namespace PlayerSpace;

public partial class SaveManager : Node
{
    public static SaveManager Instance { get; private set; }

    [Export] public SaveGameData saveGameData = new();

    private string saveFolderPath = "user://saves";
    private string saveFileName = "user://saves/savegame.tres";

    public override void _Ready()
    {
        Instance = this;
    }

    public void SaveGame()
    {
        CheckForFolderAndCreate();
        SaveFileToDisk();
    }

    private void SaveFileToDisk()
    {
        ResourceSaver.Save(saveGameData, saveFileName);
    }

    private void CheckForFolderAndCreate()
    {
        DirAccess directory = DirAccess.Open("user://");

        if (!directory.DirExists(saveFolderPath))
        {
            Error error = directory.MakeDir(saveFolderPath);
            if (error == Error.Ok)
            {
                GD.Print($"Folder created at: {saveFolderPath}");
            }
            else
            {
                GD.PrintErr("Failed to create folder.");
            }
        }
        else
        {
            GD.Print("Folder already exists.");
        }
    }
}