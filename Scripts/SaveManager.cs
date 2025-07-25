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
        //UpdateSaveGameData();
        SaveFileToDisk();
    }

    /*
    private void UpdateSaveGameData()
    {
        saveGameData.allFactionDataList = Autoload.Instance.allFactionDataList;
    }
    */

    public void LoadGame()
    {
        if (CheckForSaveFolder())
        { 
            saveGameData = (SaveGameData)ResourceLoader.Load(saveFilePath);
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
    
    private void SaveGameToJson()
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        List<FactionDto> factionDtos = [];

        foreach (FactionData faction in saveGameData.allFactionDataList)
        {
            factionDtos.Add(faction.ToDto());
        }


        string json = JsonSerializer.Serialize(factionDtos, options);

        FileAccess file = FileAccess.Open(saveFilePathJson, FileAccess.ModeFlags.Write);
        file.StoreString(json);
        file.Close();

        //GD.Print("Save Game saved as JSON:\n" + json);
    }

    private bool CheckForSaveFolder()
    {
        DirAccess directory = DirAccess.Open("user://");

        return directory.DirExists(saveFolderPath);
    }

    private void SaveFileToDisk()
    {
        SaveGameToJson();
        ResourceSaver.Save(saveGameData, saveFilePath);
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