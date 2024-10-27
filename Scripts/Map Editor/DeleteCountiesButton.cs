using Godot;
using System;
using System.Threading.Tasks;

namespace MapEditorSpace
{
    public partial class DeleteCountiesButton : Node
    {
        private async void ButtonPressed()
        {
            MapEditorControls.Instance.controlsEnabled = false;
            await DeleteCountiesFromDisk();
            await DeleteCountyNodes();
        }

        private async Task DeleteCountyNodes()
        {
            foreach (Node2D county in MapEditorGlobals.Instance.countiesParent.GetChildren())
            {
                await RootNode.Instance.WaitFrames(1);
                LogControl.Instance.UpdateLabel("Removing county node: " + county.Name);
                county.QueueFree();
            }
            LogControl.Instance.UpdateLabel("All county nodes have been deleted.");
        }

        private async Task DeleteCountiesFromDisk()
        {
            DirAccess directory = DirAccess.Open("res://");
            if (directory != null)
            {
                DirAccess countiesDirectory = DirAccess.Open("res://Counties");
                string[] directoryArray = countiesDirectory.GetFiles();
                foreach(string file in directoryArray)
                {
                    //GD.Print("Before Deleted Files: " + file);
                }
                if (directoryArray.Length > 0)
                {
                    foreach (string file in directoryArray)
                    {
                        await RootNode.Instance.WaitFrames(1);
                        LogControl.Instance.UpdateLabel("Deleting " + file);
                        countiesDirectory.Remove(file);
                    }
                    LogControl.Instance.UpdateLabel("All counties have been deleted.");
                    // This is just for testing to see if the directory is really empty.
                    directoryArray = countiesDirectory.GetFiles();
                    foreach (string file in directoryArray)
                    {
                        //GD.Print("After Deleted Files: " + file);
                    }
                }
                else
                {
                    LogControl.Instance.UpdateLabel("There are no counties to be deleted.  Generate and save the counties first.");
                }
            }
        }
    }
}