using System.Collections.Generic;
using Godot;

namespace PlayerSpace;

public partial class AllActivities : Node
{
    public static AllActivities Instance { get; private set; }

    private string activitiesDirectory = "res://Resources/Activities/";
    public readonly List<ActivityData> allActivityData = [];

    public override void _Ready()
    {
        Instance = this;

        ReadAllActivitiesFromDisk(activitiesDirectory);
    }

    private void ReadAllActivitiesFromDisk(string path)
    {
        DirAccess dirAccess = DirAccess.Open(path);

        if (dirAccess != null && dirAccess.ListDirBegin() == Error.Ok)
        {
            string fileName;

            while ((fileName = dirAccess.GetNext()) != "")
            {
                if (dirAccess.CurrentIsDir() || (!fileName.EndsWith(".tres") && !fileName.EndsWith(".res")))
                    continue;

                string filePath = path + fileName;
                ActivityData activityData = (ActivityData)ResourceLoader.Load(filePath);
                GD.Print($"Loaded Activities: {filePath}");
                allActivityData.Add(activityData);
            }

            dirAccess.ListDirEnd(); // Always close the directory listing
        }
        else
        {
            GD.PrintErr("Failed to open directory: " + path);
        }
    }
}