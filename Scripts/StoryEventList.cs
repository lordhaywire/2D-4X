using System.Collections.Generic;
using Godot;

namespace PlayerSpace
{
    public partial class StoryEventList : Node
    {
        public static StoryEventList Instance { get; private set; }

        [Export] public StoryEventData[] storyEventData;
        private string ruinExplorationDirectory = "res://Resources/Story Events/Exploration Events/Ruin/";

        public List<StoryEventData> allRuinExplorationStoryEvents = [];

        public override void _Ready()
        {
            Instance = this;
        }
        public void GetExplorationEventsFromDisk()
        {
            DirAccess dirAccess = DirAccess.Open(ruinExplorationDirectory);

            if (dirAccess != null && dirAccess.ListDirBegin() == Error.Ok)
            {
                string fileName;

                while ((fileName = dirAccess.GetNext()) != "")
                {
                    if (dirAccess.CurrentIsDir() || (!fileName.EndsWith(".tres") && !fileName.EndsWith(".res")))
                        continue;

                    string filePath = ruinExplorationDirectory + fileName;
                    Resource resource = ResourceLoader.Load(filePath);
                    GD.Print($"Load Ruin Exploration Story Event: {filePath}");
                    allRuinExplorationStoryEvents.Add((StoryEventData)resource);
                }
                dirAccess.ListDirEnd(); // Always close the directory listing
            }
            else
            {
                GD.PrintErr("Failed to open directory: " + allRuinExplorationStoryEvents);
            }
        }
    }
}