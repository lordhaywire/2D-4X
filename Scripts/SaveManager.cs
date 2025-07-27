using System.Collections.Generic;
using System.Text.Json;
using AutoloadSpace;
using Godot;

namespace PlayerSpace;

public partial class SaveManager : Node
{
    public static SaveManager Instance { get; private set; }

    [Export] public SaveGameData saveGameData = new();

    private string saveFolderPath = "user://saves";
    private string saveFilePath = "user://saves/savegame.tres";
    private string saveFilePathJson = "user://saves/savegame.json";


    public override void _Ready()
    {
        Instance = this;
    }

    public void SaveGame()
    {
        CheckForFolderAndCreate();
        SaveGameToJson();
    }


    private void SaveGameToJson()
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        // Convert entire saveGameData into DTO
        SaveGameDto saveDto = saveGameData.ToDto();

        // Serialize everything
        string json = JsonSerializer.Serialize(saveDto, options);

        // Write JSON to disk
        using FileAccess file = FileAccess.Open(saveFilePathJson, FileAccess.ModeFlags.Write);
        file.StoreString(json);

        GD.Print("Full game saved as JSON.");
    }

    public void LoadGameFromJson()
    {
        if (!CheckForSaveFolder())
        {
            GD.PrintErr("Save file not found!");
            return;
        }

        string json = FileAccess.GetFileAsString(saveFilePathJson);
        SaveGameDto saveDto = JsonSerializer.Deserialize<SaveGameDto>(json);

        // Convert DTO back to runtime SaveGameData
        saveGameData = SaveGameData.FromDto(saveDto);

        GD.Print("Full game loaded from JSON.");
    }

    /*
    public void LoadGame()
    {
        if (CheckForSaveFolder())
        {
            // We may want to move this to just be saved inside the saveGameData on its own.
            foreach (FactionData factionData in saveGameData.allFactionDataList)
            {
                if (factionData.isPlayer)
                {
                    Autoload.Instance.playerFactionData = factionData;
                }
            }
        }
        else
        {
            GD.Print("Save game folder is missing, you are so fucked.");
        }
    }
    */
    private bool CheckForSaveFolder()
    {
        DirAccess directory = DirAccess.Open("user://");

        return directory.DirExists(saveFolderPath);
    }

    private void CheckForFolderAndCreate()
    {
        DirAccess directory = DirAccess.Open("user://");

        if (!CheckForSaveFolder())
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