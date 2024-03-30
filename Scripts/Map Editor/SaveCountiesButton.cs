using Godot;
using System;
using System.Threading.Tasks;


namespace MapEditorSpace
{
    public partial class SaveCountiesButton : Node
    {
        private async void ButtonUp()
        {
            await SaveCounties();
        }

        private async Task SaveCounties()
        {
            DirAccess directory = DirAccess.Open(MapEditorGlobals.Instance.pathToCounties);
            if (directory != null)
            {
                directory.ListDirBegin();
                string[] files = directory.GetFiles();
                if (files.Length == 0)
                {
                    if (MapEditorGlobals.Instance.countiesParent.GetChildCount() > 0)
                    {
                        foreach (Node node in MapEditorGlobals.Instance.countiesParent.GetChildren())
                        {
                            await RootNode.Instance.WaitFrames(1);
                            Node2D node2d = (Node2D)node;
                            LogControl.Instance.UpdateLabel("Saving County " + node2d.Name);
                            PackedScene packedScene = new();
                            packedScene.Pack(node2d);
                            ResourceSaver.Save(packedScene, $"{MapEditorGlobals.Instance.pathToCounties}{node2d.Name}.tscn");
                        }
                        LogControl.Instance.UpdateLabel("All counties have been saved.");
                    }
                    else
                    {
                        LogControl.Instance.UpdateLabel("There are no counties generated.  Generate the counties first.");
                    }
                }
                else
                {
                    LogControl.Instance.UpdateLabel("Counties already saved.  Delete them first.");
                }
            }
        }
    }
}